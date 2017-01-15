using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.Enums;

namespace PoGo.NecroBot.Logic.Event.Player
{
    public class LoginEvent : IEvent
    {
        public AuthType AuthType { get; set; }
        public  string Username { get; set; }

        public LoginEvent(AuthType authType, string v)
        {
            this.AuthType = authType;
            this.Username = v;
        }
    }
}
