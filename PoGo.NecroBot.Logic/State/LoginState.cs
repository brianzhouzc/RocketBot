#region using directives

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Event.Player;
using PoGo.NecroBot.Logic.Exceptions;
using PoGo.NecroBot.Logic.Logging;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;
using POGOProtos.Enums;

#endregion

namespace PoGo.NecroBot.Logic.State
{
    public class LoginState : IState
    {
        private PokemonId pokemonToCatch;

        public LoginState(PokemonId pokemonToCatch = PokemonId.Missingno)
        {
            this.pokemonToCatch = pokemonToCatch;
        }

        public async Task<IState> Execute(ISession session, CancellationToken cancellationToken)
        {
            // cancellationToken.ThrowIfCancellationRequested();
            session.EventDispatcher.Send(new LoginEvent(
                session.Settings.AuthType, $"{session.Settings.GoogleUsername}{session.Settings.PtcUsername}"
                ));

            //session.EventDispatcher.Send(new NoticeEvent
            //{
            //    Message = session.Translation.GetTranslation(TranslationString.LoggingIn, session.Settings.AuthType)
            //});

            await CheckLogin(session, cancellationToken);
            try
            {
                if (session.Settings.AuthType == AuthType.Google || session.Settings.AuthType == AuthType.Ptc)
                {
                    session.Profile = await session.Client.Login.DoLogin();
                }
                else
                {
                    session.EventDispatcher.Send(new ErrorEvent
                    {
                        Message = session.Translation.GetTranslation(TranslationString.WrongAuthType)
                    });
                }
            }
            catch (AggregateException ae)
            {
                throw ae.Flatten().InnerException;
            }
            catch (APIBadRequestException)
            {
                session.EventDispatcher.Send(new ErrorEvent
                {
                    Message = session.Translation.GetTranslation(TranslationString.LoginInvalid)
                });

                await Task.Delay(2000, cancellationToken);
                throw new LoginFailedException();
            }
            catch (AccessTokenExpiredException)
            {
                session.EventDispatcher.Send(new ErrorEvent
                {
                    Message = session.Translation.GetTranslation(TranslationString.AccessTokenExpired)
                });
                return new LoginState();
            }
            catch (PtcOfflineException)
            {
                session.EventDispatcher.Send(new ErrorEvent
                {
                    Message = session.Translation.GetTranslation(TranslationString.PtcOffline)
                });
                session.EventDispatcher.Send(new NoticeEvent
                {
                    Message = session.Translation.GetTranslation(TranslationString.TryingAgainIn, 20)
                });
            }
            catch (AccountNotVerifiedException ex)
            {
                session.EventDispatcher.Send(new ErrorEvent
                {
                    Message = session.Translation.GetTranslation(TranslationString.AccountNotVerified)
                });
                await Task.Delay(2000, cancellationToken);
                throw ex;
            }
            catch (GoogleException e)
            {
                if (e.Message.Contains("NeedsBrowser"))
                {
                    session.EventDispatcher.Send(new ErrorEvent
                    {
                        Message = session.Translation.GetTranslation(TranslationString.GoogleTwoFactorAuth)
                    });
                    session.EventDispatcher.Send(new ErrorEvent
                    {
                        Message = session.Translation.GetTranslation(TranslationString.GoogleTwoFactorAuthExplanation)
                    });
                    await Task.Delay(7000, cancellationToken);
                    try
                    {
                        Process.Start("https://security.google.com/settings/security/apppasswords");
                    }
                    catch (Exception)
                    {
                        session.EventDispatcher.Send(new ErrorEvent
                        {
                            Message = "https://security.google.com/settings/security/apppasswords"
                        });
                        throw;
                    }
                }
                session.EventDispatcher.Send(new ErrorEvent
                {
                    Message = session.Translation.GetTranslation(TranslationString.GoogleError)
                });
                await Task.Delay(2000, cancellationToken);
                Environment.Exit(0);
            }
            catch (ActiveSwitchByRuleException)
            {
            }
            catch (OperationCanceledException)
            {
                //just continue login if this happen, most case is bot switching...
            }
            catch (InvalidProtocolBufferException ex) when (ex.Message.Contains("SkipLastField"))
            {
                session.EventDispatcher.Send(new ErrorEvent
                {
                    Message = session.Translation.GetTranslation(TranslationString.IPBannedError)
                });
                await Task.Delay(2000, cancellationToken);
                Environment.Exit(0);
            }
            catch (MinimumClientVersionException ex)
            {
                // We need to terminate the client.
                session.EventDispatcher.Send(new ErrorEvent
                {
                    Message = session.Translation
                        .GetTranslation(
                            TranslationString.MinimumClientVersionException,
                            ex.CurrentApiVersion.ToString(),
                            ex.MinimumClientVersion.ToString()
                        )
                });

                Logger.Write(session.Translation.GetTranslation(TranslationString.ExitNowAfterEnterKey, LogLevel.Error));
                Console.ReadKey();
                Environment.Exit(1);
            }
            catch (CaptchaException captcha)
            {
                throw captcha;
            }
            catch (Exception e)
            {
                Logger.Write(e.ToString());
                await Task.Delay(20000, cancellationToken);
                return this;
            }
            try
            {
                await DownloadProfile(session);
                if (session.Profile == null)
                {
                    await Task.Delay(20000, cancellationToken);
                    Logger.Write(
                        "Due to login failure your player profile could not be retrieved. Press any key to re-try login.",
                        LogLevel.Warning
                    );
                    Console.ReadKey();
                }

                if (session.LogicSettings.UseRecyclePercentsInsteadOfTotals)
                {
                    int totalPercent = session.LogicSettings.PercentOfInventoryPokeballsToKeep +
                                       session.LogicSettings.PercentOfInventoryPotionsToKeep +
                                       session.LogicSettings.PercentOfInventoryRevivesToKeep +
                                       session.LogicSettings.PercentOfInventoryBerriesToKeep;

                    if (totalPercent != 100)
                    {
                        Logger.Write(session.Translation.GetTranslation(TranslationString.TotalRecyclePercentGreaterThan100), LogLevel.Error);
                        Logger.Write("Press any key to exit, then fix your configuration and run the bot again.", LogLevel.Warning);
                        Console.ReadKey();
                        Environment.Exit(1);
                    }
                    else
                    {
                        Logger.Write(session.Translation.GetTranslation(TranslationString.UsingRecyclePercentsInsteadOfTotals, session.Profile.PlayerData.MaxItemStorage), LogLevel.Info);
                        Logger.Write(session.Translation.GetTranslation(TranslationString.PercentPokeballsToKeep, session.LogicSettings.PercentOfInventoryPokeballsToKeep, (int)Math.Floor(session.LogicSettings.PercentOfInventoryPokeballsToKeep / 100.0 * session.Profile.PlayerData.MaxItemStorage)), LogLevel.Info);
                        Logger.Write(session.Translation.GetTranslation(TranslationString.PercentPotionsToKeep, session.LogicSettings.PercentOfInventoryPotionsToKeep, (int)Math.Floor(session.LogicSettings.PercentOfInventoryPotionsToKeep / 100.0 * session.Profile.PlayerData.MaxItemStorage)), LogLevel.Info);
                        Logger.Write(session.Translation.GetTranslation(TranslationString.PercentRevivesToKeep, session.LogicSettings.PercentOfInventoryRevivesToKeep, (int)Math.Floor(session.LogicSettings.PercentOfInventoryRevivesToKeep / 100.0 * session.Profile.PlayerData.MaxItemStorage)), LogLevel.Info);
                        Logger.Write(session.Translation.GetTranslation(TranslationString.PercentBerriesToKeep, session.LogicSettings.PercentOfInventoryBerriesToKeep, (int)Math.Floor(session.LogicSettings.PercentOfInventoryBerriesToKeep / 100.0 * session.Profile.PlayerData.MaxItemStorage)), LogLevel.Info);
                    }
                }
                else
                {
                    int maxTheoreticalItems = session.LogicSettings.TotalAmountOfPokeballsToKeep +
                                              session.LogicSettings.TotalAmountOfPotionsToKeep +
                                              session.LogicSettings.TotalAmountOfRevivesToKeep +
                                              session.LogicSettings.TotalAmountOfBerriesToKeep;

                    if (maxTheoreticalItems > session.Profile.PlayerData.MaxItemStorage)
                    {
                        Logger.Write(session.Translation.GetTranslation(TranslationString.MaxItemsCombinedOverMaxItemStorage, maxTheoreticalItems, session.Profile.PlayerData.MaxItemStorage), LogLevel.Error);
                        Logger.Write("Press any key to exit, then fix your configuration and run the bot again.", LogLevel.Warning);
                        Console.ReadKey();
                        Environment.Exit(1);
                    }
                }
            }
            catch (ActiveSwitchByRuleException)
            {
            }
            catch (OperationCanceledException)
            {
                //just continue login if this happen, most case is bot switching...
            }
            catch (CaptchaException ex)
            {
                throw ex;
            }

            catch (APIBadRequestException)
            {
                throw new LoginFailedException();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            session.LoggedTime = DateTime.Now;
            session.EventDispatcher.Send(new LoggedEvent()
            {
                Profile = session.Profile
            });
            if (this.pokemonToCatch != PokemonId.Missingno)
            {
                return new BotSwitcherState(this.pokemonToCatch);
            }
            return new LoadSaveState();
        }

        private static async Task CheckLogin(ISession session, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (session.Settings.AuthType == AuthType.Google &&
                (session.Settings.GoogleUsername == null || session.Settings.GooglePassword == null))
            {
                session.EventDispatcher.Send(new ErrorEvent
                {
                    Message = session.Translation.GetTranslation(TranslationString.MissingCredentialsGoogle)
                });
                await Task.Delay(2000, cancellationToken);
                Environment.Exit(0);
            }
            else if (session.Settings.AuthType == AuthType.Ptc &&
                     (session.Settings.PtcUsername == null || session.Settings.PtcPassword == null))
            {
                session.EventDispatcher.Send(new ErrorEvent
                {
                    Message = session.Translation.GetTranslation(TranslationString.MissingCredentialsPtc)
                });
                await Task.Delay(2000, cancellationToken);
                Environment.Exit(0);
            }
        }

        public async Task DownloadProfile(ISession session)
        {
            try
            {
                //TODO : need get all data at 1 call here to save speed login.
                session.Profile = await session.Inventory.GetPlayerData();
                var stats = await session.Inventory.GetPlayerStats();

                session.EventDispatcher.Send(new ProfileEvent {Profile = session.Profile, Stats = stats});
            }
            catch (UriFormatException e)
            {
                session.EventDispatcher.Send(new ErrorEvent {Message = e.ToString()});
            }
            catch (CaptchaException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                session.EventDispatcher.Send(new ErrorEvent {Message = ex.ToString()});
            }
        }
    }
}