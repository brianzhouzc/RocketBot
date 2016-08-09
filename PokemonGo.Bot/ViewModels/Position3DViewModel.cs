using System.Globalization;

namespace PokemonGo.Bot.ViewModels
{
    public class Position3DViewModel : Position2DViewModel
    {
        public double Altitute { get; }

        public Position3DViewModel(double lat, double lon, double alt)
            : base(lat, lon)
        {
            Altitute = alt;
        }

        public override bool Equals(object obj) => Equals(obj as Position3DViewModel);

        public bool Equals(Position3DViewModel other)
        {
            return base.Equals(other) &&
                Altitute == other.Altitute;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = (base.GetHashCode() * 7) + Altitute.GetHashCode();
                return hash;
            }
        }

        public override string ToString() => string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", Latitude, Longitude, Altitute);

        public static Position3DViewModel operator +(Position3DViewModel left, Position3DViewModel right)
            => new Position3DViewModel(left.Latitude + right.Latitude, left.Longitude + right.Longitude, left.Altitute + right.Altitute);

        public static Position3DViewModel operator -(Position3DViewModel left, Position3DViewModel right)
            => new Position3DViewModel(left.Latitude - right.Latitude, left.Longitude - right.Longitude, left.Altitute - right.Altitute);

        public static Position3DViewModel operator *(Position3DViewModel left, double right)
            => new Position3DViewModel(left.Latitude * right, left.Longitude * right, left.Altitute * right);

        public static Position3DViewModel operator /(Position3DViewModel left, double right)
            => new Position3DViewModel(left.Latitude / right, left.Longitude / right, left.Altitute / right);
    }
}