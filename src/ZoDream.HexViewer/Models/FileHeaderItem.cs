using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.HexViewer.Storage;

namespace ZoDream.HexViewer.Models
{
    public class FileHeaderItem
    {
        public string Extension { get; set; }

        public string Description { get; set; } = string.Empty;

        public string HeaderHex { get; set; }

        public string FooterHex { get; set; } = string.Empty;

        public async Task<bool> MatchAsync(IByteStream stream)
        {
            // [7] 00 00  [7]表示忽略的位数
            return await MatchHeaderAsync(stream) && await MatchFooterAsync(stream);
        }

        private async Task<bool> MatchHeaderAsync(IByteStream stream)
        {
            if (string.IsNullOrEmpty(HeaderHex))
            {
                return true;
            }
            var pos = 0;
            var items = HeaderHex.Split(new char[] { '[', ']' });
            for (int i = 0; i < items.Length; i++)
            {
                var val = items[i].Trim();
                if (string.IsNullOrEmpty(val))
                {
                    continue;
                }
                if (i % 2 == 1)
                {
                    pos += Convert.ToInt32(val);
                    continue;
                }
                var buffer = App.ViewModel.Get(ByteBaseMode.Hex).ToByte(val);
                if (buffer.Length < 1)
                {
                    continue;
                }
                if (!await stream.IsAsync(buffer, pos))
                {
                    return false;
                }
                pos += buffer.Length;
            }
            return true;
        }

        private async Task<bool> MatchFooterAsync(IByteStream stream)
        {
            if (string.IsNullOrEmpty(FooterHex))
            {
                return true;
            }
            var pos = stream.Length;
            var items = FooterHex.Split(new char[] { '[', ']' });
            for (int i = items.Length - 1; i >= 0; i--)
            {
                var val = items[i].Trim();
                if (string.IsNullOrEmpty(val))
                {
                    continue;
                }
                if (i % 2 == 1)
                {
                    pos -= Convert.ToInt32(val);
                    continue;
                }
                var buffer = App.ViewModel.Get(ByteBaseMode.Hex).ToByte(val);
                if (buffer.Length < 1)
                {
                    continue;
                }
                if (!await stream.IsAsync(buffer, pos - buffer.Length))
                {
                    return false;
                }
                pos -= buffer.Length;
            }
            return true;
        }

        public FileHeaderItem(string extension, string headerHex)
        {
            Extension = extension;
            HeaderHex = headerHex;
        }
    }
}
