using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Utilities
{
    static class Maths
    {
        public static double Clamp(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        /// <summary>
        /// Add uint without overflow.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="modifier"></param>
        /// <param name="is_subtract"></param>
        /// <returns></returns>
        public static uint SafeUIntAdd(uint value, uint modifier)
        {
            return (uint)Clamp((double)value + modifier, uint.MinValue, uint.MaxValue);
        }

        /// <summary>
        /// Subtract uint without underflow.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="modifier"></param>
        /// <param name="is_subtract"></param>
        /// <returns></returns>
        public static uint SafeUIntSubtract(uint value, uint modifier)
        {
            return (uint)Clamp((double)value - modifier, uint.MinValue, uint.MaxValue);
        }
    }
}
