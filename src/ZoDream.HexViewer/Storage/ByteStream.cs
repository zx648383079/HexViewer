﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.ViewModel;

namespace ZoDream.HexViewer.Storage
{
    public class ByteStream : BindableBase, IByteStream
    {

        public ByteStream(string fileName)
        {
            FileName = fileName;
            BaseStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        }

        private readonly string FileName;
        public FileStream BaseStream { get; private set; }
        private volatile bool IsLoading = false;

        public long Position => BaseStream.Position;

        public long Length => BaseStream.Length;

        public Task<byte[]> ReadAsync(int count, CancellationToken cancellationToken = default)
        {
            return Lock(() =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return [];
                }
                return ReadByteAsync(count, cancellationToken).GetAwaiter().GetResult();
            });
        }

        public Task<byte[]> ReadAsync(long position, int count, CancellationToken cancellationToken = default)
        {
            return Lock(() =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return [];
                }
                Seek(position);
                if (cancellationToken.IsCancellationRequested)
                {
                    return [];
                }
                return ReadByteAsync(count, cancellationToken).GetAwaiter().GetResult();
            });
        }

        public Task SeekAsync(long position)
        {
            return Lock(() =>
            {
                Seek(position);
            });
        }


        public Task<long> FindAsync(byte[] buffer, long position = 0, bool reverse = false, CancellationToken cancellationToken = default)
        {
            return Lock(() =>
            {
                if (reverse && position < buffer.Length)
                {
                    return -1L;
                }
                if (position < 0)
                {
                    position = 0;
                }
                long i;
                if (reverse)
                {
                    for (i = position - buffer.Length; i >= 0; i--)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return -1L;
                        }
                        if (IsByte(buffer, i))
                        {
                            return i;
                        }
                    }
                } else
                {
                    for (i = position; i < Length; i++)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return -1L;
                        }
                        if (IsByte(buffer, i))
                        {
                            return i;
                        }
                    }
                }
                return -1L;
            });
        }


        public Task<bool> IsAsync(byte[] buffer, long position)
        {
            return Lock(() =>
            {
                if (position < 0)
                {
                    return false;
                }
                return IsByte(buffer, position);
            });
        }

        private bool IsByte(byte[] buffer, long postion)
        {
            int j = 0;
            while (j < buffer.Length && postion + j < Length && buffer[j] == ReadByte(postion + j))
            {
                j++;
            }
            return j == buffer.Length;
        }


        public Task CopyToAsync(Stream dist, long position, long count)
        {
            return Lock(() =>
            {
                if (count <= 0)
                {
                    return;
                }
                Seek(position);
                while (count > 0)
                {
                    var bt = BaseStream.ReadByte();
                    if (bt < 0)
                    {
                        break;
                    }
                    dist.WriteByte((byte)bt);
                    count --;
                }
            });
        }


        public Task DeleteAsync(long position, long count)
        {
            return Lock(() =>
            {
                if (count < 1)
                {
                    return;
                }
                OpenWritable();
                MoveByte(position + count, - count);
            });
        }
        public Task WriteAsync(byte[] buffer, long position, long replaceCount)
        {
            return Lock(() =>
            {
                var offset = buffer.Length - replaceCount;
                OpenWritable();
                MoveByte(position + replaceCount, offset);
                Seek(position);
                BaseStream.Write(buffer, 0, buffer.Length);
            });
        }
        public Task WriteAsync(Stream source, long position, long replaceCount)
        {
            return Lock(() =>
            {
                var offset = source.Length - replaceCount;
                OpenWritable();
                MoveByte(position + replaceCount, offset);
                Seek(position);
                source.Seek(0, SeekOrigin.Begin);
                while (true)
                {
                    var bt = source.ReadByte();
                    if (bt < 0)
                    {
                        break;
                    }
                    BaseStream.WriteByte((byte)bt);
                }
            });

        }

        private void OpenWritable()
        {
            if (BaseStream.CanWrite)
            {
                return;
            }
            var pos = BaseStream.Position;
            BaseStream.Close();
            BaseStream = new FileStream(FileName, FileMode.Open, FileAccess.ReadWrite);
            BaseStream.Seek(pos, SeekOrigin.Begin);
        }

        /// <summary>
        /// 移动子节
        /// </summary>
        /// <param name="position">要移动子节的开始位置</param>
        /// <param name="offset">移动的距离</param>
        private void MoveByte(long position, long offset)
        {
            if (offset == 0)
            {
                return;
            }
            var end = Length - 1;
            if (offset > 0)
            {
                BaseStream.SetLength(Length + offset);
            }
            long start;
            var max = 4048;
            var buffer = new byte[max];
            int bufferLen; // buffer 有效的字节数
            if (offset < 0)
            {
                start = position;
                while (start <= end)
                {
                    bufferLen = (int)Math.Min(max, end - start + 1);
                    Seek(start);
                    BaseStream.Read(buffer, 0, bufferLen);
                    Seek(start + offset);
                    for (int i = 0; i < bufferLen; i++)
                    {
                        BaseStream.WriteByte(buffer[i]);
                    }
                    start += bufferLen;
                }
            } else
            {
                start = Math.Max(position, end - max);
                while (start <= end)
                {
                    bufferLen = (int)Math.Min(max, end - start + 1);
                    Seek(start);
                    Seek(start);
                    BaseStream.Read(buffer, 0, bufferLen);
                    Seek(start + offset);
                    for (int i = 0; i < bufferLen; i++)
                    {
                        BaseStream.WriteByte(buffer[i]);
                    }
                    start += bufferLen;
                }
            }
            if (offset < 0)
            {
                BaseStream.SetLength(Length + offset);
            }
        }

        private int ReadByte(long position)
        {
            Seek(position);
            return BaseStream.ReadByte();
        }

        private async Task<byte[]> ReadByteAsync(int count, CancellationToken cancellationToken = default)
        {
            var max = Length - Position;
            if (max <= 0 || count <= 0)
            {
                return [];
            }
            if (count > max)
            {
                count = Convert.ToInt32(max);
            }
            var bytes = new byte[count];
            if (cancellationToken.IsCancellationRequested)
            {
                return bytes;
            }
            await BaseStream.ReadAsync(bytes, 0, count, cancellationToken);
            return bytes;
        }

        private void Seek(long position)
        {
            BaseStream.Seek(position, SeekOrigin.Begin);
        }

        private void WaitUnlock()
        {
            while (IsLoading)
            {
                Thread.Sleep(50);
            }
        }


        private Task<T> Lock<T>(Func<T> func)
        {
            return Task.Factory.StartNew(() =>
            {
                WaitUnlock();
                IsLoading = true;
                var res = func();
                IsLoading = false;
                return res;
            });
        }

        private Task Lock(Action func)
        {
            return Task.Factory.StartNew(() =>
            {
                WaitUnlock();
                IsLoading = true;
                func();
                IsLoading = false;
            });
        }


        public void Dispose()
        {
            BaseStream.Dispose();
        }
    }
}
