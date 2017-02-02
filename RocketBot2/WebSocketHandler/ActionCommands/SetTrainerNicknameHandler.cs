using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.Model;
using PoGo.NecroBot.Logic.State;
using POGOProtos.Networking.Responses;
using SuperSocket.WebSocket;

namespace RocketBot2.WebSocketHandler.ActionCommands
{
    public class SetTrainerNicknameHandler : IWebSocketRequestHandler
    {
        public string Command { get; private set; }

        public SetTrainerNicknameHandler()
        {
            Command = "SetNickname";
        }

        public async Task Handle(ISession session, WebSocketSession webSocketSession, dynamic message)
        {
            string nickname = message.Data;

            if (nickname.Length > 15)
            {
                session.EventDispatcher.Send(new NoticeEvent()
                {
                    Message = "You selected too long Desired name, max length: 15!"
                });
                return;
            }
            if (nickname == session.Profile.PlayerData.Username) return;


            using (var blocker = new BlockableScope(session, BotActions.UpdateProfile))
            {
                if (!await blocker.WaitToRun()) return;

                var res = await session.Client.Misc.ClaimCodename(nickname);
                if (res.Status == ClaimCodenameResponse.Types.Status.Success)
                {
                    session.EventDispatcher.Send(new NoticeEvent()
                    {
                        Message = $"Your name is now: {res.Codename}"
                    });

                    session.EventDispatcher.Send(new NicknameUpdateEvent()
                    {
                        Nickname = res.Codename
                    });
                }
                else
                {
                    session.EventDispatcher.Send(new NoticeEvent()
                    {
                        Message = $"Couldn't change your nick name"
                    });
                }
            }
        }
    }
}