using System;
using System.IO;
using System.Threading.Tasks;

namespace BilibiliDanMuLib
{
    public static class StaticUtils
    {
        public static async Task ReadBAsync(this Stream stream, byte[] buffer, int offset, int count)
        {
            if (offset + count > buffer.Length)
                throw new ArgumentException();
            int read = 0;
            while (read < count)
            {
                var available = await stream.ReadAsync(buffer, offset, count - read);

                read += available;
                offset += available;

                if (available == 0)
                {
                    throw new ObjectDisposedException(null);
                }
            }
        }
    }
}
