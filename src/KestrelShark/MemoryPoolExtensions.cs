using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace KestrelShark
{
    internal static class MemoryPoolExtensions
    {
        /// <summary>
        /// Computes a minimum segment size
        /// </summary>
        /// <param name="pool"></param>
        /// <returns></returns>
        public static int GetMinimumSegmentSize(this MemoryPool<byte> pool)
        {
            if (pool == null)
            {
                return 4096;
            }

            return Math.Min(4096, pool.MaxBufferSize);
        }
    }
}

