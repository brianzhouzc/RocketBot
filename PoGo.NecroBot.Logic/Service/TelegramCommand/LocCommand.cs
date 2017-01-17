using PoGo.NecroBot.Logic.State;
using System;
using System.Device.Location;
using System.Threading.Tasks;

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

        public override async Task<bool> OnCommand(ISession session, string cmd, Action<GeoCoordinate> Callback)
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
