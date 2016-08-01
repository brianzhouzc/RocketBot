using System;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    public static class TransferPokemonAlgorithms
    {
        public static ITranferPokemonAlgorithm Get(TransferPokemonAlgorithm algorithm)
        {
            switch (algorithm)
            {
                case TransferPokemonAlgorithm.LeaveStrongest:
                    throw new NotImplementedException();
                case TransferPokemonAlgorithm.All:
                    return new TransferPokemonAlgorithmAll();

                case TransferPokemonAlgorithm.Duplicate:
                    throw new NotImplementedException();
                case TransferPokemonAlgorithm.IVDuplicate:
                    throw new NotImplementedException();
                case TransferPokemonAlgorithm.CP:
                    throw new NotImplementedException();
                case TransferPokemonAlgorithm.IV:
                    throw new NotImplementedException();
                case TransferPokemonAlgorithm.None:
                    return new TransferPokemonAlgorithmAll();

                default:
                    throw new ArgumentOutOfRangeException(nameof(algorithm), "Unknown transfer algorithm.");
            }
        }
    }
}