using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDanMuLib.Extensions
{
    internal static class StreamExtensions
    {
        internal static async Task<bool> ReadBAsync(this Stream stream, byte[] buffer, int offset, int count)
        {
            if (offset + count > buffer.Length)
                throw new ArgumentException();

            var read = 0;
            while (read < count)
            {
                var available = await stream.ReadAsync(buffer.AsMemory(offset, count - read));

                read += available;
                offset += available;

                if (available == 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
