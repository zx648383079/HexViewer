using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.HexViewer.Models
{
    public class ByteModeItem
    {
        public ByteBaseMode Mode { get; set; }

        public string Prefix { get; set; } = string.Empty;

        public int Length { get; set; }

        public int StringFormat { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Format(long val, bool addPrefix = false, bool pad = false)
        {
            var text = Convert.ToString(val, StringFormat);
            if (pad)
            {
                text = text.PadLeft(Length, '0');
            }
            if (addPrefix && !string.IsNullOrEmpty(Prefix))
            {
                return Prefix + text;
            }
            return text;
        }

        public byte[] ToByte(string text)
        {
            var buffer = new List<byte>();
            var items = text.Split(new char[] { ' ', '\n', '\r', '\t' });
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }
                buffer.Add(Convert.ToByte(item, StringFormat));
            }
            return buffer.ToArray();
        }

        public ByteModeItem(ByteBaseMode mode, string name)
        {
            Mode = mode;
            Name = name;
        }
    }

    public enum ByteBaseMode
    {
        Binary,
        Octal,
        Decimal,
        Hex
    }
}
