#region using directives

using System;
using System.IO;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Tasks;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.Logging;
using PoGo.NecroBot.Logic.Model.Settings;
using PokemonGo.RocketAPI.Exceptions;
using PoGo.NecroBot.Logic.Exceptions;
using PoGo.NecroBot.Logic.Utils;

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
                await Task.Run(async () =>
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
                        (CatchPokemonTask._catchPokemonLimitReached || CatchPokemonTask._catchPokemonTimerReached))
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
                catch (ActiveSwitchByPokemonException rsae)
                {
                    session.EventDispatcher.Send(new WarnEvent { Message = "Encountered a good pokemon , switch another bot to catch him too." });
                    session.ResetSessionToWithNextBot(rsae.Bot,session.Client.CurrentLatitude, session.Client.CurrentLongitude, session.Client.CurrentAltitude);
                    state = new LoginState(rsae.LastEncounterPokemonId);
                }
                catch (ActiveSwitchByRuleException se)
                {
                    session.EventDispatcher.Send(new WarnEvent { Message = $"Switch bot account activated by : {se.MatchedRule.ToString()}  - {se.ReachedValue} " });
                    if (se.MatchedRule == SwitchRules.PokestopSoftban)
                    {
                        session.BlockCurrentBot();
                        session.ResetSessionToWithNextBot();
                    }
                    else 
                    if(se.MatchedRule == SwitchRules.CatchFlee)
                    {
                        session.BlockCurrentBot(60);
                        session.ResetSessionToWithNextBot();
                    }
                    else
                    {
                        if (session.LogicSettings.MultipleBotConfig.StartFromDefaultLocation)
                        {
                            session.ResetSessionToWithNextBot(null, globalSettings.LocationConfig.DefaultLatitude, globalSettings.LocationConfig.DefaultLongitude, session.Client.CurrentAltitude);
                        }
                        else
                        {
                            session.ResetSessionToWithNextBot(); //current location
                        }
                    }
                    //return to login state
                    state = new LoginState();
                }

                catch (InvalidResponseException)
                {
                    session.EventDispatcher.Send(new ErrorEvent { Message = "Niantic Servers unstable, throttling API Calls." });
                    await Task.Delay(1000);
                    apiCallFailured++;
                    if(apiCallFailured > 20)
                    {
                        apiCallFailured = 0;
                        session.BlockCurrentBot(30);
                        session.ResetSessionToWithNextBot();
                        state = new LoginState();
                    }
                }
                catch (OperationCanceledException)
                {
                    session.EventDispatcher.Send(new ErrorEvent {Message = "Current Operation was canceled."});
                    state = _initialState;
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
                catch (LoginFailedException)
                {
                    session.EventDispatcher.Send(new ErrorEvent { Message = session.Translation.GetTranslation(TranslationString.AccountBanned) });
                    session.EventDispatcher.Send(new ErrorEvent { Message = session.Translation.GetTranslation(TranslationString.ExitNowAfterEnterKey) });
                    Console.ReadKey();
                    System.Environment.Exit(1);
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
                catch (CaptchaException)
                {
                    // TODO Show the captcha.
                    session.EventDispatcher.Send(new WarnEvent { Message = session.Translation.GetTranslation(TranslationString.CaptchaShown) });
                    session.EventDispatcher.Send(new ErrorEvent { Message = session.Translation.GetTranslation(TranslationString.ExitNowAfterEnterKey) });
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    session.EventDispatcher.Send(new ErrorEvent {Message = "Pokemon Servers might be offline / unstable. Trying again..."});
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