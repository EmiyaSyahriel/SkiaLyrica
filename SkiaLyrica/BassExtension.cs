using ManagedBass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiaLyrica
{
    public static partial class Extensions
    {
        public static bool IsPaused(this MediaPlayer mp) => mp.State == PlaybackState.Paused;
    }
}
