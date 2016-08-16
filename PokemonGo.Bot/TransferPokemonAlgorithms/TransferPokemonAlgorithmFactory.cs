using PokemonGo.Bot.Utils;
using System;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    public class TransferPokemonAlgorithmFactory
    {
        readonly Settings settings;

        public TransferPokemonAlgorithmFactory(Settings settings)
        {
            this.settings = settings;
        }

        public ITranferPokemonAlgorithm Get(TransferPokemonAlgorithm algorithm)
        {
            switch (algorithm)
            {
                case TransferPokemonAlgorithm.CPDuplicate:
                    return new TransferPokemonAlgorithmCPDuplicate();

                case TransferPokemonAlgorithm.All:
                    return new TransferPokemonAlgorithmAll();

                case TransferPokemonAlgorithm.Duplicate:
                    return new TransferPokemonAlgorithmDuplicate();

                case TransferPokemonAlgorithm.IVDuplicate:
                    return new TransferPokemonAlgorithmIVDuplicate();

                case TransferPokemonAlgorithm.CP:
                    return new TransferPokemonAlgorithmCP(settings.DoNotTransferPokemonAboveCP);

                case TransferPokemonAlgorithm.IV:
                    return new TransferPokemonAlgorithmIV(settings.DoNotTransferPokemonAboveIV);

                case TransferPokemonAlgorithm.None:
                    return new TransferPokemonAlgorithmNone();

                case TransferPokemonAlgorithm.IVDuplicateUnderCPThreshold:
                    return new TransferPokemonAlgorithmIVDuplicateUnderCPThreshold(settings.DoNotTransferPokemonAboveCP);

                default:
                    throw new ArgumentOutOfRangeException(nameof(algorithm), "Unknown transfer algorithm.");
            }
        }

        public TransferPokemonAlgorithm GetDefaultFromSettings() => settings.TransferType;
    }
}