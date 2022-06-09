using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.HexViewer.Models;
using ZoDream.Shared.ViewModels;

namespace ZoDream.HexViewer.ViewModels
{
    public class SettingViewModel: BindableBase
    {

        private ByteModeItem[] byteModeItems = App.ViewModel.ByteModeItems;

        public ByteModeItem[] ByteModeItems
        {
            get => byteModeItems;
            set => Set(ref byteModeItems, value);
        }

        private string[] encodingItems = Encoding.GetEncodings().Select(i => i.Name).ToArray();

        public string[] EncodingItems
        {
            get => encodingItems;
            set => Set(ref encodingItems, value);
        }


        private double fontSize = 16;

        public double FontSize
        {
            get => fontSize;
            set => Set(ref fontSize, value);
        }

        private int byteLength = 16;

        public int ByteLength
        {
            get => byteLength;
            set => Set(ref byteLength, value);
        }

        private ByteModeItem lineMode = App.ViewModel.Get(ByteBaseMode.Hex);

        public ByteModeItem LineMode
        {
            get => lineMode;
            set => Set(ref lineMode, value);
        }

        private ByteModeItem byteMode = App.ViewModel.Get(ByteBaseMode.Hex);

        public ByteModeItem ByteMode
        {
            get => byteMode;
            set => Set(ref byteMode, value);
        }

        private string textEncoding = "ASCII";

        public string TextEncoding
        {
            get => textEncoding;
            set => Set(ref textEncoding, value);
        }



    }
}
