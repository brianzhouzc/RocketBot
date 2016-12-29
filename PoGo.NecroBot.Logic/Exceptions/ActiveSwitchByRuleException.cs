using POGOProtos.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGo.NecroBot.Logic.Exceptions                          
{
    public enum SwitchRules
    {
        Pokestop,
        Pokemon,
        EXP,
        Runtime,
        PokestopSoftban,
        CatchFlee,
        CatchLimitReached,
        SpinPokestopReached,
        EmptyMap
    }
    public class ActiveSwitchByRuleException : Exception
    {
        public ActiveSwitchByRuleException() { }
        public ActiveSwitchByRuleException(SwitchRules rule, double value)
        {
            this.MatchedRule = rule;
            this.ReachedValue = value;
        }
        public SwitchRules MatchedRule { get; set; }
        public double ReachedValue { get; set; }

    }
}
