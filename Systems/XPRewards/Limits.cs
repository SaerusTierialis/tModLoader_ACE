using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Systems.XPRewards
{
    public static class Limits
    {
        public const byte MAX_TIER = 3;
        public const uint MAX_tLEVEL = 255;

        /// <summary>
        /// tier 0 is for character level
        /// </summary>
        public static readonly uint[] TIER_MAX_LEVEL = { MAX_tLEVEL, 10, 50, 100 };

        /// <summary>
        /// lookup for tLevel adjust value per tier
        /// </summary>
        public static readonly uint[] TIER_tLEVEL_ADJUST = new uint[1 + MAX_TIER];
        static Limits()
        {
            uint adjust = 0;

            for (byte tier = 1; tier <= MAX_TIER; tier++)
            {
                TIER_tLEVEL_ADJUST[tier] = adjust;
                adjust += TIER_MAX_LEVEL[tier];
            }
        }
    }
}
