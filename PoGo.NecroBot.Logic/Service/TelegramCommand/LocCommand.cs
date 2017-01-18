using System;
using System.Device.Location;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Common;
using PoGo.NecroBot.Logic.State;

namespace PoGo.NecroBot.Logic.Service.TelegramCommand
{
    public class LocCommand : CommandLocation
    {
        public override string Command => "/loc";
        public override bool StopProcess => true;
        public override TranslationString DescriptionI18NKey => TranslationString.TelegramCommandLocDescription;
        public override TranslationString MsgHeadI18NKey => TranslationString.TelegramCommandLocMsgHead;

        public LocCommand(TelegramUtils telegramUtils) : base(telegramUtils)
        {
        }

        #pragma warning disable 1998 // added to get rid of compiler warning. Remove this if async code is used below.
        public override async Task<bool> OnCommand(ISession session, string cmd, Action<GeoCoordinate> callback)
        #pragma warning restore 1998
        {
            if (cmd.ToLower() == Command)
            {
                // TODO can we send a text together with the location?
                callback(new GeoCoordinate(session.Client.CurrentLatitude, session.Client.CurrentLongitude));
                return true;
            }
            return false;
        }
    }
}