using PoGo.NecroBot.Logic.Tasks;

namespace PoGo.NecroBot.Logic.Event.Snipe
{
    public class SnipePokemonStarted : IEvent
    {
        public MSniperServiceTask.MSniperInfo2 Pokemon;

        public SnipePokemonStarted(MSniperServiceTask.MSniperInfo2 location)
        {
            this.Pokemon = location;
        }
    }
}