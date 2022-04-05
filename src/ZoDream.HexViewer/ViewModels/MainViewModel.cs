using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.HexViewer.Storage;

namespace ZoDream.HexViewer.ViewModels
{
    public class MainViewModel: BindableBase
    {

        public MainViewModel()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

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


        ~MainViewModel()
        {
            Reader?.Dispose();
        }
    }
}
