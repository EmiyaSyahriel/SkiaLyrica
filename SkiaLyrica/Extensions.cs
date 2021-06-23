using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiaLyrica
{
    public static partial class Extensions
    {
        public static float ms(this TimeSpan ts) => (float)ts.TotalMilliseconds;
        public static float s(this TimeSpan ts) => (float)ts.TotalSeconds;
    }
}
