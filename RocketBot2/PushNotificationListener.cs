#region using directives

using System;
using System.Diagnostics.CodeAnalysis;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.State;
using PoGo.NecroBot.Logic.Utils;
using System.Threading.Tasks;

#endregion

namespace RocketBot2
{
    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    internal class PushNotificationListener
    {
        private static async Task HandleEventAsync(ErrorEvent errorEvent, ISession session)
        {
            await PushNotificationClient.SendNotification(session, "Error occured", errorEvent.Message).ConfigureAwait(false);
        }

        public static void HandleEvent(EncounteredEvent ev, ISession session)
        {
        }

        public static void HandleEvent(IEvent evt, ISession session)
        {
        }

        internal void Listen(IEvent evt, ISession session)
        {
            dynamic eve = evt;

            try
            {
                HandleEvent(eve, session);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}