using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            if (Prefix.Length > 1 && text.Contains(Prefix))
            {
                // 根据前缀来拆
                var temp = new List<string>();
                foreach (var item in items)
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        continue;
                    }
                    foreach (var it in item.Split(new string[] { Prefix }, StringSplitOptions.None))
                    {
                        if (string.IsNullOrEmpty(it))
                        {
                            continue;
                        }
                        temp.Add(it);
                    }
                }
                items = temp.ToArray();
            } else if (!text.Contains(' '))
            {
                // 根据长度来拆
                var temp = new List<string>();
                foreach (var item in items)
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        continue;
                    }
                    for (int i = 0; i < item.Length; i+= Length)
                    {
                        var j = Math.Min(i + Length, Length);
                        temp.Add(item.Substring(i, j - i));
                    }
                }
                items = temp.ToArray();
            }
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

        public bool IsMatch(string text)
        {
            text = text.Trim();
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }
            if (Prefix.Length > 1 && Regex.IsMatch(text, "^" + Prefix + @"[0-9a-fA-F]{" + Length + @"}$"))
            {
                return true;
            }
            if (!Regex.IsMatch(text, @"^[0-9a-fA-F\s]+$"))
            {
                return false;
            }
            var items = text.Split(new char[] { ' ' }, 2);
            if (items[0].Length == Length)
            {
                return true;
            }
            if (Regex.IsMatch(text, @"[a-fA-F]"))
            {
                return Mode == ByteBaseMode.Hex;
            }
            if (Regex.IsMatch(text, @"[89]"))
            {
                return Mode == ByteBaseMode.Decimal;
            }
            if (Regex.IsMatch(text, @"[2-7]"))
            {
                return Mode == ByteBaseMode.Octal;
            }
            return Mode == ByteBaseMode.Binary;
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
