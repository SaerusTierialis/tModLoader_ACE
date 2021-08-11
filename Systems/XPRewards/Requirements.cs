using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Systems.XPRewards
{
    public static class Requirements
    {
        public static readonly uint[] XP_PER_tLEVEL = new uint[1 + Limits.MAX_tLEVEL];

        static Requirements()
        {
            //tier 1 (predefined)
            uint[] xp_predef = new uint[] { 0, 10, 15, 20, 30, 40, 50, 60, 80, 100 }; //length+1 must be UI.UI.MAX_LEVEL[1]
            byte num_predef = (byte)(xp_predef.Length - 1);

            //tier 2+
            double adjust;
            for (uint i = 1; i < XP_PER_tLEVEL.Length; i++)
            {
                if (i <= num_predef)
                {
                    XP_PER_tLEVEL[i] = xp_predef[i];
                }
                else
                {
                    adjust = Math.Max(1.09 - ((i - 1.0 - num_predef) / 1000), 1.08);
                    XP_PER_tLEVEL[i] = (uint)Math.Round(XP_PER_tLEVEL[i - 1] * adjust, 0);
                }
            }
        }
    }
}
