using POGOProtos.Networking.Envelopes;
using PokemonGo.RocketAPI.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokemonGo.Bot
{
    public class ApiFailureStrategy : IApiFailureStrategy
    {
        private readonly IDictionary<ulong, int> retryCounts = new Dictionary<ulong, int>();
        private const int retryMax = 5;

        public async Task<ApiOperation> HandleApiFailure(RequestEnvelope request, ResponseEnvelope response)
        {
            var retryCountForRequest = 0;
            retryCounts.TryGetValue(request.RequestId, out retryCountForRequest);
            retryCountForRequest++;
            retryCounts[request.RequestId] = retryCountForRequest;

            if (retryCountForRequest <= retryMax)
            {
                await Task.Delay(1000);
                return ApiOperation.Retry;
            }

            retryCounts.Remove(request.RequestId);
            return ApiOperation.Abort;
        }

        public void HandleApiSuccess(RequestEnvelope request, ResponseEnvelope response)
        {
            retryCounts.Remove(request.RequestId);
        }
    }
}