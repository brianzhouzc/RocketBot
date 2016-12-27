#region using directives
using System.Linq;
using PoGo.NecroBot.Logic.Event;
using PoGo.NecroBot.Logic.State;
using POGOProtos.Map.Fort;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace PoGo.NecroBot.Logic.Tasks
{
    public class SetMoveToTargetTask
    {
        public static string TARGET_ID = "NECRO2_FORT";

        private static FortDetailsResponse _fortInfo;
        private static FortData _targetStop;
        public static FortDetailsResponse FortInfo { get { return _fortInfo; } }

        public static async Task Execute(ISession session, double lat, double lng, string fortId = "")
        {
            await Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(fortId))
                {
                    var knownFort = session.Forts.FirstOrDefault(x => x.Id == fortId);
                    if (knownFort != null)
                    {
                        _targetStop = knownFort;
                        return;
                    }
                }
                //at this time only allow one target, can't be cancel
                if (_targetStop == null || _targetStop.CooldownCompleteTimestampMs == 0)
                {
                    _targetStop = new FortData()
                    {
                        Latitude = lat,
                        Longitude = lng,
                        Id = TARGET_ID,
                        Type = FortType.Checkpoint,
                        CooldownCompleteTimestampMs = DateTime.UtcNow.AddHours(1).ToUnixTime()  //make sure bot not try to spin this fake pokestop
                    };

                    _fortInfo = new FortDetailsResponse()
                    {
                        Latitude = lat,
                        Longitude = lng,
                        Name = "Your selected location"
                    };
                }
            });
        }
        public static async Task<bool> IsReachedDestination(FortData destination, ISession session, CancellationToken cancellationToken)
        {
            if (destination == _targetStop && destination.Id == TARGET_ID ) 
            {
                _targetStop = null;

                //looking for pokemon
                await CatchNearbyPokemonsTask.Execute(session, cancellationToken);
                //TODO - maybe looking for lure pokestop and try catch lure pokestop task
                return true;
            }
            return false;
        }

        internal static async Task<FortData> GetTarget(ISession session)
        {
            if (_targetStop != null &&
                !session.LogicSettings.UseGpxPathing &&
                _targetStop.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime())
            {
                if (_targetStop.Id == TARGET_ID)
                {
                    _targetStop.CooldownCompleteTimestampMs = DateTime.UtcNow.AddHours(1).ToUnixTime();
                }
                return _targetStop;
            }
            await Task.Delay(0);//to avoid waning, nothing todo.
            return null;
        }
    }
}