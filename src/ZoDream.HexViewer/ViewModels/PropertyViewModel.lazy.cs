using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.HexViewer.Models;
using ZoDream.HexViewer.Storage;
using ZoDream.HexViewer.Utils;
using ZoDream.Shared.Storage;
using ZoDream.Shared.ViewModel;

namespace ZoDream.HexViewer.ViewModels
{
    public partial class PropertyViewModel
    {

        private string md5Text = "Loading...";

        public string Md5Text {
            get => md5Text;
            set => Set(ref md5Text, value);
        }

        private string crcText = "Loading...";

        public string CrcText {
            get => crcText;
            set => Set(ref crcText, value);
        }


        private string shaText = "Loading...";

        public string ShaText {
            get => shaText;
            set => Set(ref shaText, value);
        }


        public ICommand Md5Command { get; private set; }
        public ICommand CrcCommand { get; private set; }
        public ICommand ShaCommand { get; private set; }

        private void TapMd5(object? _)
        {
            var fs = App.ViewModel.Reader!.BaseStream;
            var pos = fs.Position;
            fs.Seek(0, SeekOrigin.Begin);
            Md5Text = Disk.GetMD5(fs);
            fs.Seek(0, SeekOrigin.Begin);
            CrcText = Disk.GetCRC32(fs);
            fs.Seek(0, SeekOrigin.Begin);
            ShaText = Disk.GetSha1(fs);
            fs.Seek(pos, SeekOrigin.Begin);
        }

        private void TapCrc(object? _)
        {
            TapMd5(_);
        }

        private void TapSha(object? _)
        {
            TapMd5(_);
        }
    }
}
