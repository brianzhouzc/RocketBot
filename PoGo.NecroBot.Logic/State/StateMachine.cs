#region using directives

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Tasks;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.Model.Settings;
using PokemonGo.RocketAPI.Exceptions;
using PoGo.NecroBot.Logic.Exceptions;
using PoGo.NecroBot.Logic.Utils;
using PoGo.NecroBot.Logic.Captcha;

#endregion

namespace PoGo.NecroBot.Logic.State
{
    public class StateMachine
    {
        private IState _initialState;

        public Task AsyncStart(IState initialState, Session session, string subPath, bool excelConfigAllowed=false)
        {
            return Task.Run(() => Start(initialState, session, subPath, excelConfigAllowed));
        }

        public void SetFailureState(IState state)
        {
            _initialState = state;
        }

        public async Task Start(IState initialState, ISession session, string subPath, bool excelConfigAllowed = false)
        {
            GlobalSettings globalSettings = null;

            var state = initialState;
            var profilePath = Path.Combine(Directory.GetCurrentDirectory(), subPath);
            var profileConfigPath = Path.Combine(profilePath, "config");
            globalSettings = GlobalSettings.Load(subPath);
      
            FileSystemWatcher configWatcher = new FileSystemWatcher();
            configWatcher.Path = profileConfigPath;
            configWatcher.Filter = "config.json";
            configWatcher.NotifyFilter = NotifyFilters.LastWrite;
            configWatcher.EnableRaisingEvents = true;

            configWatcher.Changed += (sender, e) =>
            {
                if (e.ChangeType == WatcherChangeTypes.Changed)
                {
                    globalSettings = GlobalSettings.Load(subPath);
                    session.LogicSettings = new LogicSettings(globalSettings);
                    configWatcher.EnableRaisingEvents = !configWatcher.EnableRaisingEvents;
                    configWatcher.EnableRaisingEvents = !configWatcher.EnableRaisingEvents;
                    Logger.Write(" ##### config.json ##### ", LogLevel.Info);
                }
            };

            //watch the excel config file
            if (excelConfigAllowed)
            {
                Task.Run(async () =>
                {
                    while (true)
                    {
                        try
                        {
                            FileInfo inf = new FileInfo($"{profileConfigPath}\\config.xlsm");
                            if (inf.LastWriteTime > DateTime.Now.AddSeconds(-5))
                            {
                                globalSettings = ExcelConfigHelper.ReadExcel(globalSettings, inf.FullName);
                                session.LogicSettings = new LogicSettings(globalSettings);
                                Logger.Write(" ##### config.xlsm ##### ", LogLevel.Info);
                            }
                            await Task.Delay(5000);
                        }
                        catch (Exception)
                        {

                        }
                    }
                });
            }

            int apiCallFailured = 0;
            do
            {
                try
                {
                    state = await state.Execute(session, session.CancellationTokenSource.Token);

                    // Exit the bot if both catching and looting has reached its limits
                    if ((UseNearbyPokestopsTask._pokestopLimitReached || UseNearbyPokestopsTask._pokestopTimerReached) &&
                        session.Stats.CatchThresholdExceeds(session))
                    {
                        session.EventDispatcher.Send(new ErrorEvent
                        {
                            Message = session.Translation.GetTranslation(TranslationString.ExitDueToLimitsReached)
                        });

                        session.CancellationTokenSource.Cancel();

                        // A bit rough here; works but can be improved
                        await Task.Delay(10000);
                        state = null;
                        session.CancellationTokenSource.Dispose();
                        Environment.Exit(0);
                    }
                }
                catch (APIBadRequestException ex)
                {
                    Logger.Write("Bad Request - If you see this message please conpy error log & screenshot send back to dev asap.", level: LogLevel.Error);

                    session.EventDispatcher.Send(new ErrorEvent() { Message = ex.Message });
                    Logger.Write(ex.StackTrace, level: LogLevel.Error);

                    if (session.LogicSettings.AllowMultipleBot)
                        session.ReInitSessionWithNextBot();
                    state = new LoginState();
                }
                catch (AccountNotVerifiedException)
                {
                    if (session.LogicSettings.AllowMultipleBot)
                    {
                        session.ReInitSessionWithNextBot();
                        state = new LoginState();
                    }
                    else {
                        Console.Read();
                        Environment.Exit(0);
                    }
                }
                catch (ActiveSwitchByPokemonException rsae)
                {
                    session.EventDispatcher.Send(new WarnEvent { Message = "Encountered a good pokemon , switch another bot to catch him too." });
                    session.ReInitSessionWithNextBot(rsae.Bot, session.Client.CurrentLatitude, session.Client.CurrentLongitude, session.Client.CurrentAltitude);
                    state = new LoginState(rsae.LastEncounterPokemonId);
                }
                catch (ActiveSwitchByRuleException se)
                {
                    session.EventDispatcher.Send(new WarnEvent { Message = $"Switch bot account activated by : {se.MatchedRule.ToString()}  - {se.ReachedValue} " });
                    if (se.MatchedRule == SwitchRules.EmptyMap)
                    {
                        session.BlockCurrentBot(90);
                        session.ReInitSessionWithNextBot();
                    }
                    else
                    if (se.MatchedRule == SwitchRules.PokestopSoftban)
                    {
                        session.BlockCurrentBot();
                        session.ReInitSessionWithNextBot();
                    }
                    else
                    if (se.MatchedRule == SwitchRules.CatchFlee)
                    {
                        session.BlockCurrentBot(60);
                        session.ReInitSessionWithNextBot();
                    }
                    else
                    {
                        if (se.MatchedRule == SwitchRules.CatchLimitReached || se.MatchedRule == SwitchRules.SpinPokestopReached)
                        {
                            PushNotificationClient.SendNotification(session, $"{se.MatchedRule} - {session.Settings.GoogleUsername}{session.Settings.PtcUsername}", "This bot has reach limit, it will be blocked for 60 mins for safety.", true);
                            session.EventDispatcher.Send(new WarnEvent() { Message = "You reach limited. bot will sleep for 60 min" });

                            session.BlockCurrentBot(60);

                            if (!session.LogicSettings.AllowMultipleBot)
                            {
                                await Task.Delay(60 * 1000 * 60);
                            }
                            else
                            {
                                session.ReInitSessionWithNextBot();
                            }
                        }
                        else {
                            if (session.LogicSettings.MultipleBotConfig.StartFromDefaultLocation)
                            {
                                session.ReInitSessionWithNextBot(null, globalSettings.LocationConfig.DefaultLatitude, globalSettings.LocationConfig.DefaultLongitude, session.Client.CurrentAltitude);
                            }
                            else
                            {
                                session.ReInitSessionWithNextBot(); //current location
                            }
                        }
                    }
                    //return to login state
                    state = new LoginState();
                }

                catch (InvalidResponseException)
                {
                    session.EventDispatcher.Send(new ErrorEvent { Message = "Niantic Servers unstable, throttling API Calls." });
                    await Task.Delay(1000);
                    if (session.LogicSettings.AllowMultipleBot)
                    {
                        apiCallFailured++;
                        if (apiCallFailured > 20)
                        {
                            apiCallFailured = 0;
                            session.BlockCurrentBot(30);
                            session.ReInitSessionWithNextBot();
                        }
                    }
                    state = new LoginState();

                }
                catch (OperationCanceledException)
                {
                    session.EventDispatcher.Send(new ErrorEvent { Message = "Current Operation was canceled." });
                    if (session.LogicSettings.AllowMultipleBot)
                    {
                        session.BlockCurrentBot(30);
                        session.ReInitSessionWithNextBot();
                    }
                    state = new LoginState();

                }
                catch (LoginFailedException)
                {
                    PushNotificationClient.SendNotification(session, $"Banned!!!! {session.Settings.PtcUsername}{session.Settings.GoogleUsername}", session.Translation.GetTranslation(TranslationString.AccountBanned), true);

                    if (session.LogicSettings.AllowMultipleBot)
                    {
                        session.BlockCurrentBot(24 * 60); //need remove acc
                        session.ReInitSessionWithNextBot();
                        state = new LoginState();
                    }
                    else {
                        session.EventDispatcher.Send(new ErrorEvent { Message = session.Translation.GetTranslation(TranslationString.ExitNowAfterEnterKey) });
                        Console.ReadKey();
                        System.Environment.Exit(1);
                    }
                }
                catch (MinimumClientVersionException ex)
                {
                    // We need to terminate the client.
                    session.EventDispatcher.Send(new ErrorEvent
                    {
                        Message = session.Translation.GetTranslation(TranslationString.MinimumClientVersionException, ex.CurrentApiVersion.ToString(), ex.MinimumClientVersion.ToString())
                    });

                    session.EventDispatcher.Send(new ErrorEvent { Message = session.Translation.GetTranslation(TranslationString.ExitNowAfterEnterKey) });
                    Console.ReadKey();
                    System.Environment.Exit(1);
                }
                catch(TokenRefreshException ex)
                {
                    session.EventDispatcher.Send(new ErrorEvent() { Message = ex.Message });

                    if (session.LogicSettings.AllowMultipleBot)
                        session.ReInitSessionWithNextBot();
                    state = new LoginState();
                    
                }
                
                catch (PtcOfflineException)
                {
                    session.EventDispatcher.Send(new ErrorEvent { Message = session.Translation.GetTranslation(TranslationString.PtcOffline) });
                    session.EventDispatcher.Send(new NoticeEvent { Message = session.Translation.GetTranslation(TranslationString.TryingAgainIn, 15) });

                    await Task.Delay(15000);
                    state = _initialState;
                }
                catch (GoogleOfflineException)
                {
                    session.EventDispatcher.Send(new ErrorEvent { Message = session.Translation.GetTranslation(TranslationString.GoogleOffline) });
                    session.EventDispatcher.Send(new NoticeEvent { Message = session.Translation.GetTranslation(TranslationString.TryingAgainIn, 15) });

                    await Task.Delay(15000);
                    state = _initialState;
                }
                catch (AccessTokenExpiredException)
                {
                    session.EventDispatcher.Send(new NoticeEvent { Message = "Access Token Expired. Logging in again..." });
                    state = _initialState;
                }
                catch (CaptchaException captchaException)
                {
                    var resolved = await CaptchaManager.SolveCaptcha(session, captchaException.Url);
                    if (!resolved)
                    {
                        PushNotificationClient.SendNotification(session, $"Captcha required {session.Settings.PtcUsername}{session.Settings.GoogleUsername}", session.Translation.GetTranslation(TranslationString.CaptchaShown), true);
                        session.EventDispatcher.Send(new WarnEvent { Message = session.Translation.GetTranslation(TranslationString.CaptchaShown) });
                        if (session.LogicSettings.AllowMultipleBot)
                        {
                            session.BlockCurrentBot(15);
                            session.ReInitSessionWithNextBot();
                         
                            state = new LoginState();
                        }
                        else {
                            session.EventDispatcher.Send(new ErrorEvent { Message = session.Translation.GetTranslation(TranslationString.ExitNowAfterEnterKey) });
                            Console.ReadKey();
                            Environment.Exit(0);
                        }
                    }
                    else
                    {
                        //resolve captcha
                        state = new LoginState();
                    }
                }
                catch (HasherException ex)
                {
                    session.EventDispatcher.Send(new ErrorEvent { Message = ex.Message });
                  //  session.EventDispatcher.Send(new ErrorEvent { Message = session.Translation.GetTranslation(TranslationString.ExitNowAfterEnterKey) });
                    state = new IdleState();
                    //Console.ReadKey();
                    //System.Environment.Exit(1);
                }
                catch (Exception ex)
                {
                    session.EventDispatcher.Send(new ErrorEvent { Message = "Pokemon Servers might be offline / unstable. Trying again..." });
                    session.EventDispatcher.Send(new ErrorEvent { Message = "Error: " + ex });
                    if (state is LoginState)
                    {
                    }
                    else
                        state = _initialState;
                }
            } while (state != null);
            configWatcher.EnableRaisingEvents = false;
            configWatcher.Dispose();
        }
    }
}