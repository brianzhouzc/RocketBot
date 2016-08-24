namespace PokemonGo.Bot.MVVMLightUtils
{
    public interface IUpdateable<T>
    {
        void UpdateWith(T other);
    }
}