using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZoDream.HexViewer.Models;
using ZoDream.HexViewer.Storage;

namespace ZoDream.HexViewer.Pages
{
    /// <summary>
    /// EditView.xaml 的交互逻辑
    /// </summary>
    public partial class EditView : Window
    {
        public EditView()
        {
            InitializeComponent();
        }

        private void TypeTb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var i = (sender as ComboBox).SelectedIndex;
            BytePanel.Visibility = i == 1 ? Visibility.Visible : Visibility.Collapsed;
            TextPanel.Visibility = i < 1 ? Visibility.Visible : Visibility.Collapsed;
            FilePanel.Visibility = i == 2 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TypeTb.ItemsSource = new string[] { "文本", "子节", "文件" };
            EncodingTb.ItemsSource = Encoding.GetEncodings().Select(i => i.Name).ToArray();
            ByteTypeTb.ItemsSource = App.ViewModel.ByteModeItems;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (App.ViewModel.Reader == null)
            {
                return;
            }
            var start = StartTb.Value;
            var end = EndTb.Value;
            var count = end < start ? 0 : (end - start + 1);
            switch (TypeTb.SelectedIndex)
            {
                case 0:
                    WriteTextAsync(App.ViewModel.Reader, start, count);
                    break;
                case 1:
                    WriteByteAsync(App.ViewModel.Reader, start, count);
                    break;
                case 2:
                    WriteFileAsync(App.ViewModel.Reader, start, count);
                    break;
                default:
                    break;
            }
        }

        private async void WriteFileAsync(IByteStream stream, long start, long count)
        {
            var file = FileTb.FileName;
            using var fs = new FileStream(file, FileMode.Open, FileAccess.Read);
            await stream.WriteAsync(fs, start, count);
            DialogResult = true;
        }

        private async void WriteByteAsync(IByteStream stream, long start, long count)
        {
            var format = ByteTypeTb.SelectedItem as ByteModeItem;
            if (format == null)
            {
                return;
            }
            await stream.WriteAsync(format.ToByte(ByteTextTb.Text), start, count);
            DialogResult = true;
        }

        private async void WriteTextAsync(IByteStream stream, long start, long count)
        {
            var encoding = string.IsNullOrWhiteSpace(EncodingTb.Text) ? Encoding.Default : Encoding.GetEncoding(EncodingTb.Text.Trim());
            var bytes = encoding.GetBytes(TextTb.Text);
            await stream.WriteAsync(bytes, start, count);
            DialogResult = true;
        }
    }
}
