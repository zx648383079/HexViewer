using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.HexViewer.Utils
{
    public static class Disk
    {
        public static string FormatSize(long size)
        {
            var len = size.ToString().Length;
            if (len < 4)
            {
                return $"{size}B";
            }
            if (len < 7)
            {
                return Math.Round(Convert.ToDouble(size / 1024d), 2) + "KB";
            }
            if (len < 10)
            {
                return Math.Round(Convert.ToDouble(size / 1024d / 1024), 2) + "MB";
            }
            if (len < 13)
            {
                return Math.Round(Convert.ToDouble(size / 1024d / 1024 / 1024), 2) + "GB";
            }
            if (len < 16)
            {
                return Math.Round(Convert.ToDouble(size / 1024d / 1024 / 1024 / 1024), 2) + "TB";
            }
            return Math.Round(Convert.ToDouble(size / 1024d / 1024 / 1024 / 1024 / 1024), 2) + "PB";
        }

        public static string GetMD5(string fileName)
        {
            using var fs = File.OpenRead(fileName);
            return GetMD5(fs);
        }

        public static string GetMD5(Stream stream)
        {
            var md5 = MD5.Create();
            var res = md5.ComputeHash(stream);
            var sb = new StringBuilder();
            foreach (var b in res)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public static string GetSha1(string fileName)
        {
            using var fs = File.OpenRead(fileName);
            return GetSha1(fs);
        }

        public static string GetSha1(Stream stream)
        {
            var sha = SHA1.Create();
            var res = sha.ComputeHash(stream);
            var sb = new StringBuilder();
            foreach (var b in res)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public static string GetCRC32(string fileName)
        {
            using var fs = File.OpenRead(fileName);
            return GetCRC32(fs);
        }

        public static string GetCRC32(Stream stream)
        {
            uint res = 0;
            while (true)
            {
                var b = stream.ReadByte();
                if (b == -1)
                {
                    break;
                }
                res = Crc32Tab.Crc32(res, (byte)b);
            }
            return res.ToString("X");
        }

    }
}
