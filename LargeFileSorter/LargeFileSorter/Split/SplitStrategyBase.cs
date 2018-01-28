using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LargeFileSorter
{
    public abstract class SplitStrategyBase
    {
        protected virtual long GetMemoryLimitForOneChunk()
        {
            ComputerInfo ci = new ComputerInfo();

            return (long)Math.Max(ci.AvailablePhysicalMemory, ci.TotalPhysicalMemory / 8);
        }

        protected void AdjustMemoryLimit(ref long bytesOfMemoryToConsume)
        {
            if (bytesOfMemoryToConsume <= 0)
            {
                bytesOfMemoryToConsume = GetMemoryLimitForOneChunk();
            }

            Console.WriteLine($"Effective memory limit: {bytesOfMemoryToConsume:n0} bytes");
        }
    }
}
