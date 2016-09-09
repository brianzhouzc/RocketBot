using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PokemonGo.WPF.Converters
{
    class PokemonToCandyColorConverter : IValueConverter
    {
        static PokemonToImagePathConverter pokemonToImagePathConverter = new PokemonToImagePathConverter();

        // from http://gaming.stackexchange.com/questions/274588/how-to-do-you-find-and-acquire-all-the-pokemon-go-candy-colors/
        static IDictionary<int, Tuple<Color, Color>> pokemonToColorCache = new Dictionary<int, Tuple<Color, Color>>
        {
            { 1, Tuple.Create((Color)ColorConverter.ConvertFromString("#36C8A4"), (Color)ColorConverter.ConvertFromString("#A3FB83")) }, //         Bulbasaur
            { 4, Tuple.Create((Color)ColorConverter.ConvertFromString("#F09230"), (Color)ColorConverter.ConvertFromString("#FFE699")) }, //         Charmander
            { 7, Tuple.Create((Color)ColorConverter.ConvertFromString("#85C4D6"), (Color)ColorConverter.ConvertFromString("#F2E8BE")) }, //         Squirtle
            { 10, Tuple.Create((Color)ColorConverter.ConvertFromString("#A5CD87"), (Color)ColorConverter.ConvertFromString("#FAE3B1")) }, //        Caterpie
            { 13, Tuple.Create((Color)ColorConverter.ConvertFromString("#E7BC83"), (Color)ColorConverter.ConvertFromString("#DB76AD")) }, //        Weedle
            { 16, Tuple.Create((Color)ColorConverter.ConvertFromString("#E9E0B7"), (Color)ColorConverter.ConvertFromString("#D29E65")) }, //         Pidgey
            { 19, Tuple.Create((Color)ColorConverter.ConvertFromString("#A989BA"), (Color)ColorConverter.ConvertFromString("#D9D7BE")) }, //         Rattata
            { 21, Tuple.Create((Color)ColorConverter.ConvertFromString("#EBB9A0"), (Color)ColorConverter.ConvertFromString("#FE5D6C")) }, //         Spearow
            { 23, Tuple.Create((Color)ColorConverter.ConvertFromString("#CBA8C9"), (Color)ColorConverter.ConvertFromString("#F1E090")) }, //         Ekans
            { 25, Tuple.Create((Color)ColorConverter.ConvertFromString("#F5D368"), (Color)ColorConverter.ConvertFromString("#E2A65D")) }, //         Pikachu
            { 27, Tuple.Create((Color)ColorConverter.ConvertFromString("#E0D2A4"), (Color)ColorConverter.ConvertFromString("#C9B180")) }, //         Sandshrew
            { 29, Tuple.Create((Color)ColorConverter.ConvertFromString("#C5D3E4"), (Color)ColorConverter.ConvertFromString("#9697C5")) }, //         Nidoran F
            { 32, Tuple.Create((Color)ColorConverter.ConvertFromString("#D59FC1"), (Color)ColorConverter.ConvertFromString("#C37096")) }, //         Nidoran M
            { 35, Tuple.Create((Color)ColorConverter.ConvertFromString("#F1D3D1"), (Color)ColorConverter.ConvertFromString("#F1BFC0")) }, //         Clefairy
            { 37, Tuple.Create((Color)ColorConverter.ConvertFromString("#F5865E"), (Color)ColorConverter.ConvertFromString("#F6D29C")) }, //         Vulpix
            { 39, Tuple.Create((Color)ColorConverter.ConvertFromString("#F1D2E1"), (Color)ColorConverter.ConvertFromString("#EAB9CE")) }, //         Jigglypuff
            { 41, Tuple.Create((Color)ColorConverter.ConvertFromString("#478ABF"), (Color)ColorConverter.ConvertFromString("#DC8DD7")) }, //         Zubat
            { 43, Tuple.Create((Color)ColorConverter.ConvertFromString("#7095BF"), (Color)ColorConverter.ConvertFromString("#75C06B")) }, //         Oddish
            { 46, Tuple.Create((Color)ColorConverter.ConvertFromString("#F1873D"), (Color)ColorConverter.ConvertFromString("#FFD159")) }, //         Paras
            { 48, Tuple.Create((Color)ColorConverter.ConvertFromString("#998FD6"), (Color)ColorConverter.ConvertFromString("#E24379")) }, //         Venonat
            { 50, Tuple.Create((Color)ColorConverter.ConvertFromString("#B08570"), (Color)ColorConverter.ConvertFromString("#EEC5DC")) }, //         Diglett
            { 52, Tuple.Create((Color)ColorConverter.ConvertFromString("#ECE0C4"), (Color)ColorConverter.ConvertFromString("#FFE28A")) }, //         Meowth
            { 54, Tuple.Create((Color)ColorConverter.ConvertFromString("#F4C487"), (Color)ColorConverter.ConvertFromString("#EEEED8")) }, //         Psyduck
            { 56, Tuple.Create((Color)ColorConverter.ConvertFromString("#E5D6CB"), (Color)ColorConverter.ConvertFromString("#C3927F")) }, //         Mankey
            { 58, Tuple.Create((Color)ColorConverter.ConvertFromString("#F3A056"), (Color)ColorConverter.ConvertFromString("#3F3D2A")) }, //         Growlithe
            { 60, Tuple.Create((Color)ColorConverter.ConvertFromString("#849FCA"), (Color)ColorConverter.ConvertFromString("#ECECF6")) }, //         Poliwag
            { 63, Tuple.Create((Color)ColorConverter.ConvertFromString("#E5CE5C"), (Color)ColorConverter.ConvertFromString("#8E7994")) }, //         Abra
            { 66, Tuple.Create((Color)ColorConverter.ConvertFromString("#A1BBDE"), (Color)ColorConverter.ConvertFromString("#DCCEB1")) }, //         Machop
            { 69, Tuple.Create((Color)ColorConverter.ConvertFromString("#EBE16E"), (Color)ColorConverter.ConvertFromString("#AFD57E")) }, //         Bellsprout
            { 72, Tuple.Create((Color)ColorConverter.ConvertFromString("#71ACD8"), (Color)ColorConverter.ConvertFromString("#C24589")) }, //         Tentacool
            { 74, Tuple.Create((Color)ColorConverter.ConvertFromString("#ACA078"), (Color)ColorConverter.ConvertFromString("#756108")) }, //         Geodude
            { 77, Tuple.Create((Color)ColorConverter.ConvertFromString("#EDE7C7"), (Color)ColorConverter.ConvertFromString("#F59062")) }, //         Ponyta
            { 79, Tuple.Create((Color)ColorConverter.ConvertFromString("#DFA1B9"), (Color)ColorConverter.ConvertFromString("#EEE1C7")) }, //         Slowpoke
            { 81, Tuple.Create((Color)ColorConverter.ConvertFromString("#D0DAE0"), (Color)ColorConverter.ConvertFromString("#92B6C6")) }, //         Magnemite
            { 83, Tuple.Create((Color)ColorConverter.ConvertFromString("#AC9E95"), (Color)ColorConverter.ConvertFromString("#95FB97")) }, //         Farfetch'd
            { 84, Tuple.Create((Color)ColorConverter.ConvertFromString("#C89462"), (Color)ColorConverter.ConvertFromString("#AF755F")) }, //         Doduo
            { 86, Tuple.Create((Color)ColorConverter.ConvertFromString("#C7DFE8"), (Color)ColorConverter.ConvertFromString("#B6CAED")) }, //         Seel
            { 88, Tuple.Create((Color)ColorConverter.ConvertFromString("#BFA4C7"), (Color)ColorConverter.ConvertFromString("#5F5370")) }, //         Grimer
            { 90, Tuple.Create((Color)ColorConverter.ConvertFromString("#AB9CC5"), (Color)ColorConverter.ConvertFromString("#E0B5B3")) }, //         Shellder
            { 92, Tuple.Create((Color)ColorConverter.ConvertFromString("#242223"), (Color)ColorConverter.ConvertFromString("#9B7FB7")) }, //         Gastly
            { 95, Tuple.Create((Color)ColorConverter.ConvertFromString("#B5B6B8"), (Color)ColorConverter.ConvertFromString("#626264")) }, //         Onix
            { 96, Tuple.Create((Color)ColorConverter.ConvertFromString("#F8CB58"), (Color)ColorConverter.ConvertFromString("#AF7961")) }, //         Drowzee
            { 97, Tuple.Create((Color)ColorConverter.ConvertFromString("#F8CB58"), (Color)ColorConverter.ConvertFromString("#AF7961")) }, //         Hypno dunno why this has an own family
            { 98, Tuple.Create((Color)ColorConverter.ConvertFromString("#EB9063"), (Color)ColorConverter.ConvertFromString("#EDD9CE")) }, //         Krabby
            { 100, Tuple.Create((Color)ColorConverter.ConvertFromString("#B64656"), (Color)ColorConverter.ConvertFromString("#F0E5EA")) }, //         Voltorb
            { 102, Tuple.Create((Color)ColorConverter.ConvertFromString("#F4DDE7"), (Color)ColorConverter.ConvertFromString("#EFC3C1")) }, //         Exeggcute
            { 104, Tuple.Create((Color)ColorConverter.ConvertFromString("#D4D5D6"), (Color)ColorConverter.ConvertFromString("#CBB57A")) }, //         Cubone
            { 106, Tuple.Create((Color)ColorConverter.ConvertFromString("#BD9F88"), (Color)ColorConverter.ConvertFromString("#EEE1C7")) }, //         Hitmonlee
            { 107, Tuple.Create((Color)ColorConverter.ConvertFromString("#C8ABBB"), (Color)ColorConverter.ConvertFromString("#E4643B")) }, //         Hitmonchan
            { 108, Tuple.Create((Color)ColorConverter.ConvertFromString("#E3AEB9"), (Color)ColorConverter.ConvertFromString("#F0E4CA")) }, //         Lickitung
            { 109, Tuple.Create((Color)ColorConverter.ConvertFromString("#8B8FAE"), (Color)ColorConverter.ConvertFromString("#DEE0BF")) }, //         Koffing
            { 111, Tuple.Create((Color)ColorConverter.ConvertFromString("#BCBDBF"), (Color)ColorConverter.ConvertFromString("#959CA2")) }, //         Rhyhorn
            { 113, Tuple.Create((Color)ColorConverter.ConvertFromString("#E0AEB2"), (Color)ColorConverter.ConvertFromString("#C68D87")) }, //         Chansey
            { 114, Tuple.Create((Color)ColorConverter.ConvertFromString("#666C9D"), (Color)ColorConverter.ConvertFromString("#E46E8C")) }, //         Tangela
            { 115, Tuple.Create((Color)ColorConverter.ConvertFromString("#978781"), (Color)ColorConverter.ConvertFromString("#E3DDB8")) }, //         Kangaskhan
            { 116, Tuple.Create((Color)ColorConverter.ConvertFromString("#9FCFE9"), (Color)ColorConverter.ConvertFromString("#FCF7D7")) }, //         Horsea
            { 118, Tuple.Create((Color)ColorConverter.ConvertFromString("#E6E6E7"), (Color)ColorConverter.ConvertFromString("#F38469")) }, //         Goldeen
            { 120, Tuple.Create((Color)ColorConverter.ConvertFromString("#B49569"), (Color)ColorConverter.ConvertFromString("#F5E688")) }, //         Staryu
            { 122, Tuple.Create((Color)ColorConverter.ConvertFromString("#E56387"), (Color)ColorConverter.ConvertFromString("#FFCED5")) }, //         Mr_Mime
            { 123, Tuple.Create((Color)ColorConverter.ConvertFromString("#92C587"), (Color)ColorConverter.ConvertFromString("#F6F0CF")) }, //         Scyther
            { 124, Tuple.Create((Color)ColorConverter.ConvertFromString("#C44552"), (Color)ColorConverter.ConvertFromString("#643187")) }, //         Jynx
            { 125, Tuple.Create((Color)ColorConverter.ConvertFromString("#F5DB77"), (Color)ColorConverter.ConvertFromString("#14175E")) }, //         Electabuzz
            { 126, Tuple.Create((Color)ColorConverter.ConvertFromString("#F5D477"), (Color)ColorConverter.ConvertFromString("#F0664E")) }, //         Magmar
            { 127, Tuple.Create((Color)ColorConverter.ConvertFromString("#BCB1AB"), (Color)ColorConverter.ConvertFromString("#CFD4D8")) }, //         Pinsir
            { 128, Tuple.Create((Color)ColorConverter.ConvertFromString("#D8A058"), (Color)ColorConverter.ConvertFromString("#887E6F")) }, //         Tauros
            { 129, Tuple.Create((Color)ColorConverter.ConvertFromString("#E87839"), (Color)ColorConverter.ConvertFromString("#F6F0CF")) }, //         Magikarp
            { 131, Tuple.Create((Color)ColorConverter.ConvertFromString("#6BA7D4"), (Color)ColorConverter.ConvertFromString("#FFF0DA")) }, //         Lapras
            { 132, Tuple.Create((Color)ColorConverter.ConvertFromString("#AD8DBE"), (Color)ColorConverter.ConvertFromString("#DBD8BE")) }, //         Ditto
            { 133, Tuple.Create((Color)ColorConverter.ConvertFromString("#CA9761"), (Color)ColorConverter.ConvertFromString("#7E5621")) }, //         Eevee
            { 137, Tuple.Create((Color)ColorConverter.ConvertFromString("#E7757C"), (Color)ColorConverter.ConvertFromString("#6BC7C5")) }, //         Porygon
            { 138, Tuple.Create((Color)ColorConverter.ConvertFromString("#DDDCCC"), (Color)ColorConverter.ConvertFromString("#73CEE2")) }, //         Omanyte
            { 140, Tuple.Create((Color)ColorConverter.ConvertFromString("#C18335"), (Color)ColorConverter.ConvertFromString("#4E4E48")) }, //         Kabuto
            { 142, Tuple.Create((Color)ColorConverter.ConvertFromString("#D4BAD3"), (Color)ColorConverter.ConvertFromString("#B196C5")) }, //         Aerodactyl
            { 143, Tuple.Create((Color)ColorConverter.ConvertFromString("#326583"), (Color)ColorConverter.ConvertFromString("#E3DACE")) }, //         Snorlax
            { 144, Tuple.Create((Color)ColorConverter.ConvertFromString("Black"), (Color)ColorConverter.ConvertFromString("Black")) },     //         Articuno
            { 145, Tuple.Create((Color)ColorConverter.ConvertFromString("Black"), (Color)ColorConverter.ConvertFromString("Black")) },     //         Zapdos
            { 146, Tuple.Create((Color)ColorConverter.ConvertFromString("Black"), (Color)ColorConverter.ConvertFromString("Black")) },     //         Moltres
            { 147, Tuple.Create((Color)ColorConverter.ConvertFromString("#90AED4"), (Color)ColorConverter.ConvertFromString("#EFEAE6")) }, //         Dratini
            { 150, Tuple.Create((Color)ColorConverter.ConvertFromString("Black"), (Color)ColorConverter.ConvertFromString("Black")) },     //         Mewtwo
            { 151, Tuple.Create((Color)ColorConverter.ConvertFromString("Black"), (Color)ColorConverter.ConvertFromString("Black")) }      //         Mew
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Colors.Red;

            var familyId = (int)value;
            var colorIndex = System.Convert.ToInt32(parameter);
            if (familyId > 0)
            {
                //Tuple<Color, Color> colors;
                //if (!pokemonToColorCache.TryGetValue(number, out colors))
                //{
                //    var pokemonImagePath = (string)pokemonToImagePathConverter.Convert(value, typeof(string), parameter, culture);
                //    var pokemonImage = new BitmapImage(new Uri(pokemonImagePath));
                //    var candypalette = new BitmapPalette(pokemonImage, 2);
                //    colors = Tuple.Create(candypalette.Colors[0], candypalette.Colors[1]);
                //    pokemonToColorCache[number] = colors;
                //}
                var colors = pokemonToColorCache[familyId];

                return colorIndex == 0 ? colors.Item1 : colors.Item2;
            }

            return Colors.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}