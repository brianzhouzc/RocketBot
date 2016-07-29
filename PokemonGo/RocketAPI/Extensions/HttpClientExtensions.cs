#region

using System.Net.Http;
using System.Threading.Tasks;
using Google.Protobuf;
using PokemonGo.RocketAPI.GeneratedCode;
using System;

#endregion

namespace PokemonGo.RocketAPI.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<Response> PostProto<TRequest>(this HttpClient client, string url, TRequest request)
            where TRequest : IMessage<TRequest>
        {
            //Encode payload and put in envelop, then send
            var data = request.ToByteString();
            var result = await client.PostAsync(url, new ByteArrayContent(data.ToByteArray()));

            //Decode message
            var responseData = await result.Content.ReadAsByteArrayAsync();
            var codedStream = new CodedInputStream(responseData);
            var decodedResponse = new Response();
            decodedResponse.MergeFrom(codedStream);

            return decodedResponse;
        }

        private static bool waitingForResponse = false;
        public static async Task<TResponsePayload> PostProtoPayload<TRequest, TResponsePayload>(this HttpClient client,
            string url, TRequest request) where TRequest : IMessage<TRequest>
            where TResponsePayload : IMessage<TResponsePayload>, new()
        {
            ByteString payload = null;

            while (waitingForResponse)
                await Task.Delay(30);
            waitingForResponse = true;

            Response response = null;
            int count = 0;
            do
            {
                count++;
                response = await PostProto(client, url, request);
                waitingForResponse = false;
                
                //Decode payload
                //todo: multi-payload support

                await Task.Delay(30);// request every 30ms, up this value for not spam their server
            } while (response.Payload.Count < 1 && count < 30);
            payload = response.Payload[0];

            var parsedPayload = new TResponsePayload();
            parsedPayload.MergeFrom(payload);

            return parsedPayload;
        }


      //  public static async Task<TResponsePayload> PostProtoPayload<TRequest, TResponsePayload>(this HttpClient client,
      //      string url, TRequest request) where TRequest : IMessage<TRequest>
      //      where TResponsePayload : IMessage<TResponsePayload>, new()
      //  {
      ////      ColoredConsoleWrite(ConsoleColor.Red, ($"[DEBUG] [{DateTime.Now.ToString("HH:mm:ss")}] requesting {typeof(TResponsePayload).Name}"));
      //      var response = await PostProto(client, url, request);

      //      //Decode payload
      //      //todo: multi-payload support
      //      var payload = response.Payload[0];
      //      var parsedPayload = new TResponsePayload();
      //      parsedPayload.MergeFrom(payload);

      //      return parsedPayload;
      //  }
      
        public static void ColoredConsoleWrite(ConsoleColor color, string text)
        {
            ConsoleColor originalColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            System.Console.WriteLine(text);
            System.Console.ForegroundColor = originalColor;
        }

    }
}
