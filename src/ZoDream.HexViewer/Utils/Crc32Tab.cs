using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.HexViewer.Utils
{
    public class Crc32Tab
    {
        const uint MASK_0_24 = 0x00ffffff;
        const uint MASK_0_26 = 0x03ffffff;
        const uint MASK_0_32 = 0xffffffff;
        const uint MASK_26_32 = 0xfc000000;
        const uint MASK_24_32 = 0xff000000;
        const uint MASK_10_32 = 0xfffffc00;
        const uint MASK_8_32 = 0xffffff00;
        const uint MASK_2_32 = 0xfffffffc;
        const uint MAXDIFF_0_24 = MASK_0_24 + 0xff;
        const uint MAXDIFF_0_26 = MASK_0_26 + 0xff;
        const uint CRCPOL = 0xedb88320;

        private readonly static Lazy<Crc32Tab> Instance = new(() => new Crc32Tab());

        public uint[] Tab { get; private set; } = new uint[256];

        public uint[] InvTab { get; private set; } = new uint[256];

        public Crc32Tab()
        {
            for (uint i = 0; i < 256; i++)
            {
                var crc = i;
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 1) != 0)
                    {
                        crc = crc >> 1 ^ CRCPOL;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
                Tab[i] = crc;
                InvTab[Msb(crc)] = crc << 8 ^ i;
            }
        }

        public static uint Crc32(uint pval, byte b)
        {
            return pval >> 8 ^ Instance.Value.Tab[Lsb(pval) ^ b];
        }

        public static uint Crc32Inv(uint crc, byte b)
        {
            return crc << 8 ^ Instance.Value.InvTab[Msb(crc)] ^ b;
        }

        public static uint GetYi_24_32(uint zi, uint zim1)
        {
            return (Crc32Inv(zi, 0) ^ zim1) << 24;
        }

        public static uint GetZim1_10_32(uint zi_2_32)
        {
            return Crc32Inv(zi_2_32, 0) & MASK_10_32;
        }

        public static byte Lsb(uint x)
        {
            return (byte)x;
        }

        public static byte Msb(uint x)
        {
            return (byte)(x >> 24);
        }
    }
}
