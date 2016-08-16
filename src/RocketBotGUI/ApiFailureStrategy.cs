using System.Threading.Tasks;
using PokemonGo.RocketAPI.Extensions;
using POGOProtos.Networking.Envelopes;

namespace PokemonGo.RocketAPI.Window
{
    internal class ApiFailureStrategy : IApiFailureStrategy
    {
        public async Task<ApiOperation> HandleApiFailure(RequestEnvelope request, ResponseEnvelope response)
        {
            return ApiOperation.Retry;
        }

        public void HandleApiSuccess(RequestEnvelope request, ResponseEnvelope response)
        {
        }
    }
}