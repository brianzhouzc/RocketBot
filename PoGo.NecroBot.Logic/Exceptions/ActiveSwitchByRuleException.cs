using System;

namespace PoGo.NecroBot.Logic.Exceptions
{
    public enum SwitchRules
    {
        Pokestop,
        Pokemon,
        EXP,
        Runtime,
        PokestopSoftban,
        CatchFlee
    }
    public class ActiveSwitchByRuleException : Exception
    {
        public SwitchRules MatchedRule { get; set; }
        public double ReachedValue { get; set; }

    }
}
