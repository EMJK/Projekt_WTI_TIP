using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Audio
{
    static class Utils
    {
        public static WaveFormat GetRawFormat()
        {
            return new WaveFormat(8000, 16, 1);

        }
    }
}
