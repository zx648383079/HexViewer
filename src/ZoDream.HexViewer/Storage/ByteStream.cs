using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.HexViewer.ViewModels;

namespace ZoDream.HexViewer.Storage
{
    public class ByteStream : BindableBase, IByteStream
    {

        public ByteStream(string fileName)
        {
            FileName = fileName;
            Reader = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite);
        }

        private string FileName;
        private FileStream Reader;
        private volatile bool IsLoading = false;

        public long Position => Reader.Position;

        public long Length => Reader.Length;

        public Task<byte[]> ReadAsync(int count)
        {
            return Task.Factory.StartNew(() =>
            {
                WaitUnlock();
                return Read(count);
            });
        }

        public Task<byte[]> ReadAsync(long position, int count)
        {
            return Task.Factory.StartNew(() =>
            {
                WaitUnlock();
                Seek(position);
                return Read(count);
            });
        }

        public Task SeekAsync(long position)
        {
            return Task.Factory.StartNew(() =>
            {
                WaitUnlock();
                Seek(position);
            });
        }


        public Task<long> FindAsync(byte[] buffer, long position = 0)
        {
            return Task.Factory.StartNew(() =>
            {
                WaitUnlock();
                IsLoading = true;
                for (var i = position; i < Length; i++)
                {
                    int j = 0;
                    while (j < buffer.Length && i + j < Length && buffer[j] == ReadByte(i + j))
                    {
                        j++;
                    }
                    if (j == buffer.Length)
                    {
                        IsLoading = false;
                        return i;
                    }
                    i += j;
                }
                IsLoading = false;
                return -1L;
            });
        }


        public Task CopyToAsync(Stream dist, long position, long count)
        {
            return Task.Factory.StartNew(() =>
            {
                if (count <= 0)
                {
                    return;
                }
                WaitUnlock();
                IsLoading = true;
                Seek(position);
                while (count > 0)
                {
                    var bt = Reader.ReadByte();
                    if (bt < 0)
                    {
                        break;
                    }
                    dist.WriteByte((byte)bt);
                    count --;
                }
                IsLoading = false;
            });
        }

        private int ReadByte(long position)
        {
            Seek(position);
            return Reader.ReadByte();
        }

        private byte[] Read(int count)
        {
            IsLoading = true;
            var max = Length - Position;
            if (max <= 0 || count <= 0)
            {
                IsLoading = false;
                return Array.Empty<byte>();
            }
            if (count > max)
            {
                count = Convert.ToInt32(max);
            }
            var bytes = new byte[count];
            Reader.Read(bytes, 0, count);
            IsLoading = false;
            return bytes;
        }

        private void Seek(long position)
        {
            Reader.Seek(position, SeekOrigin.Begin);
        }

        private void WaitUnlock()
        {
            while (IsLoading)
            {
                Thread.Sleep(50);
            }
        }

        public void Dispose()
        {
            Reader.Dispose();
        }
    }
}
