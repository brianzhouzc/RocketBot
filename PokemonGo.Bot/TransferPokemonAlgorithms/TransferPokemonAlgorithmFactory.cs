using PokemonGo.RocketAPI;
using System;

namespace PokemonGo.Bot.TransferPokemonAlgorithms
{
    public class TransferPokemonAlgorithmFactory
    {
        readonly ISettings settings;

        public TransferPokemonAlgorithmFactory(ISettings settings)
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
                    return new TransferPokemonAlgorithmCP(settings.TransferCPThreshold);
                case TransferPokemonAlgorithm.IV:
                    return new TransferPokemonAlgorithmIV(settings.TransferIVThreshold);
                case TransferPokemonAlgorithm.None:
                    return new TransferPokemonAlgorithmNone();
                case TransferPokemonAlgorithm.IVDuplicateUnderCPThreshold:
                    return new TransferPokemonAlgorithmIVDuplicateUnderCPThreshold(settings.TransferCPThreshold);

                default:
                    throw new ArgumentOutOfRangeException(nameof(algorithm), "Unknown transfer algorithm.");
            }
        }

        public TransferPokemonAlgorithm GetDefaultFromSettings()
        {
            switch (settings.TransferType)
            {
                case "Leave Strongest":
                    return TransferPokemonAlgorithm.CPDuplicate;
                case "All":
                    return TransferPokemonAlgorithm.All;
                case "Duplicate":
                    return TransferPokemonAlgorithm.Duplicate;
                case "IV Duplicate":
                    return TransferPokemonAlgorithm.IVDuplicate;
                case "IV Duplicate under cp threshold":
                    return TransferPokemonAlgorithm.IVDuplicateUnderCPThreshold;
                case "CP":
                    return TransferPokemonAlgorithm.Duplicate;
                case "IV":
                    return TransferPokemonAlgorithm.IV;
                case "None":
                    return TransferPokemonAlgorithm.None;
                default:
                    throw new ArgumentOutOfRangeException(nameof(settings.TransferType), "Unknown transfer type from settings");
            }
        }
    }
}