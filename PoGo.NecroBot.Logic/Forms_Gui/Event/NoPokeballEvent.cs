#region using directives

using POGOProtos.Enums;

#endregion

namespace PoGo.NecroBot.Logic.Forms_Gui.Event
{
    public class NoPokeballEvent : IEvent
    {
        public int Cp;
        public PokemonId Id;
    }
}