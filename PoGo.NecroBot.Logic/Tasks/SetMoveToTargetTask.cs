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
using PoGo.NecroBot.Logic.Event.Player;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace PoGo.NecroBot.Logic.Tasks
{
    public class SetMoveToTargetTask
    {
        public static string TARGET_ID = "NECRO2_FORT";

        private static FortDetailsResponse _fortInfo;
        private static FortData _targetStop;
        public static FortDetailsResponse FortInfo { get { return _fortInfo; } }

        static Queue<FortData> queue = new Queue<FortData>();

        public static async Task Execute(ISession session, double lat, double lng, string fortId = "")
        {
            await Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(fortId))
                {
                    var knownFort = session.Forts.FirstOrDefault(x => x.Id == fortId);
                    if (knownFort != null)
                    {
                        queue.Enqueue(knownFort);
                        return;
                    }
                }
                //at this time only allow one target, can't be cancel
                //if (_targetStop == null || _targetStop.CooldownCompleteTimestampMs == 0)
                {
                    _targetStop = new FortData()
                    {
                        Latitude = lat,
                        Longitude = lng,
                        Id = TARGET_ID + DateTime.Now.Ticks.ToString(),
                        Type = FortType.Checkpoint,
                        CooldownCompleteTimestampMs = DateTime.UtcNow.AddHours(1).ToUnixTime()  //make sure bot not try to spin this fake pokestop
                    };

                    
                }
                queue.Enqueue(_targetStop);
            });

            session.EventDispatcher.Send(new TargetLocationEvent(lat, lng));
           
        }
        public static FortDetailsResponse FakeFortInfo(FortData data)
        {
            return new FortDetailsResponse()
            {
                Latitude = data.Latitude,
                Longitude = data.Longitude,
                Name = "Your selected location"
            };
        }
        public static async Task<bool> IsReachedDestination(FortData destination, ISession session, CancellationToken cancellationToken)
        {
            if (_targetStop!= null && destination.Id == _targetStop.Id ) 
            {
                _targetStop = null;
                queue.Dequeue();
                //looking for pokemon
                await CatchNearbyPokemonsTask.Execute(session, cancellationToken);
                //TODO - maybe looking for lure pokestop and try catch lure pokestop task
                return true;
            }
            return false;
        }

        internal static async Task<FortData> GetTarget(ISession session)
        {

            return await Task.Run(() =>
            {
                if (queue.Count > 0 &&
                    !session.LogicSettings.UseGpxPathing)
                {
                    _targetStop = queue.Peek();
                    return _targetStop;
                }
                return null;
            });
        }
    }
}