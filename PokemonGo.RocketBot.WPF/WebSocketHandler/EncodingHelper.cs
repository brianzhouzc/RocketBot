using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PokemonGo.RocketBot.WPF.WebSocketHandler
{
    internal class EncodingHelper
    {
        public static string Serialize(dynamic evt)
        {
            var jsonSerializerSettings = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All};

            // Add custom seriaizer to convert uong to string (ulong shoud not appear to json according to json specs)
            jsonSerializerSettings.Converters.Add(new IdToStringConverter());

            return JsonConvert.SerializeObject(evt, Formatting.None, jsonSerializerSettings);
        }

        public class IdToStringConverter : JsonConverter
        {
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                var jt = JToken.ReadFrom(reader);
                return jt.Value<long>();
            }

            public override bool CanConvert(Type objectType)
            {
                return typeof(long).Equals(objectType) || typeof(ulong).Equals(objectType);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value.ToString());
            }
        }
    }
}