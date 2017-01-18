using System;
using System.Device.Location;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.State;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class LocCommand : CommandLocation
    {
        public override string Command => "/loc";
        public override string Description => "Shows the bots current location";
        public override bool StopProcess => true;

        public LocCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        #pragma warning disable 1998 // added to get rid of compiler warning. Remove this if async code is used below.
        public override async Task<bool> OnCommand(ISession session, string cmd, Action<GeoCoordinate> Callback)
        #pragma warning restore 1998
        {
            if (cmd.ToLower() == Command)
            {
                Callback(new GeoCoordinate(session.Client.CurrentLatitude, session.Client.CurrentLongitude));
                return true;
            }
            return false;
        }
    }
}