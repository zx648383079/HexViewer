using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.HexViewer.Storage
{
    public interface IByteStream: INotifyPropertyChanged, IDisposable
    {

        public long Position { get; }

        public long Length { get; }

        public Task SeekAsync(long position);

        public Task<byte[]> ReadAsync(int count);
        public Task<byte[]> ReadAsync(long position, int count);

        public Task<long> FindAsync(byte[] buffer, long position = 0);

        public Task CopyToAsync(Stream dist, long position, long count);
    }
}
