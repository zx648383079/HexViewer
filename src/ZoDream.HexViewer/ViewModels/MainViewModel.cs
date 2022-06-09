using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.HexViewer.Models;
using ZoDream.HexViewer.Storage;
using ZoDream.Shared.ViewModels;

namespace ZoDream.HexViewer.ViewModels
{
    public class MainViewModel: BindableBase
    {

        public MainViewModel()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ByteModeItems = new ByteModeItem[] {
                new ByteModeItem(ByteBaseMode.Decimal, "十进制") { Length = 3, StringFormat = 10},
                new ByteModeItem(ByteBaseMode.Binary, "二进制") {Prefix = "0b", Length = 8, StringFormat = 2},
                new ByteModeItem(ByteBaseMode.Octal, "八进制") {Prefix = "0", Length = 3, StringFormat = 8},
                new ByteModeItem(ByteBaseMode.Hex, "十六进制") {Prefix = "0x", Length = 2, StringFormat = 16},
            };
        }

        public ByteModeItem[] ByteModeItems { get; private set; }

        private string fileName = string.Empty;

        public string FileName
        {
            get { return fileName; }
            set {
                if (fileName != value)
                {
                    Reader?.Dispose();
                    Reader = new ByteStream(value);
                }
                fileName = value;
            }
        }

        private ByteStream? reader;

        public ByteStream? Reader
        {
            get => reader;
            set => Set(ref reader, value);
        }

        public ByteModeItem Get(ByteBaseMode mode)
        {
            foreach (var item in ByteModeItems)
            {
                if (item.Mode == mode)
                {
                    return item;
                }
            }
            return ByteModeItems[0];
        }

        ~MainViewModel()
        {
            Reader?.Dispose();
        }
    }
}
