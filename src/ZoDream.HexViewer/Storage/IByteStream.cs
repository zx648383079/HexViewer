using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZoDream.HexViewer.Storage
{
    public interface IByteStream: INotifyPropertyChanged, IDisposable
    {

        public long Position { get; }

        public long Length { get; }
        /// <summary>
        /// 把流中游标移动到
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Task SeekAsync(long position);
        /// <summary>
        /// 读取指定长度的子节
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public Task<byte[]> ReadAsync(int count, CancellationToken cancellationToken = default);
        /// <summary>
        /// 从指定位置读取指定长度的子节
        /// </summary>
        /// <param name="position"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public Task<byte[]> ReadAsync(long position, int count, CancellationToken cancellationToken = default);

        /// <summary>
        /// 判断是否时这些字节
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public Task<bool> IsAsync(byte[] buffer, long position);
        /// <summary>
        /// 查找指定子节的位置
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="position"></param>
        /// <param name="reverse">是否是反向寻找</param>
        /// <returns></returns>
        public Task<long> FindAsync(byte[] buffer, long position = 0, bool reverse = false, CancellationToken cancellationToken = default);
        /// <summary>
        /// 复制指定长度的内容到目标文件中
        /// </summary>
        /// <param name="dist"></param>
        /// <param name="position"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public Task CopyToAsync(Stream dist, long position, long count);
        /// <summary>
        /// 删除指定数量的子节
        /// </summary>
        /// <param name="position"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public Task DeleteAsync(long position, long count);
        /// <summary>
        /// 写入子节
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="position">写入位置</param>
        /// <param name="replaceCount">替换原本的子节数量，实际是删除这部分再扩充</param>
        /// <returns></returns>
        public Task WriteAsync(byte[] buffer, long position, long replaceCount);
        /// <summary>
        /// 从其他流中获取子节写入
        /// </summary>
        /// <param name="source"></param>
        /// <param name="position">写入位置</param>
        /// <param name="replaceCount">替换原本的子节数量，实际是删除这部分再扩充</param>
        /// <returns></returns>
        public Task WriteAsync(Stream source, long position, long replaceCount);
    }
}
