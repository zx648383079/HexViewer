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

namespace ZoDream.HexViewer.Pages
{
    /// <summary>
    /// PreviewView.xaml 的交互逻辑
    /// </summary>
    public partial class PreviewView : Window
    {
        public PreviewView()
        {
            InitializeComponent();
        }

        private void ViewBtn_Click(object sender, RoutedEventArgs e)
        {
            var encoding = Encoding.GetEncoding(EncodingTb.Text.Trim());
            var start = StartTb.Value;
            var end = EndTb.Value;
            if (end < start)
            {
                MessageBox.Show("区间错误");
                return;
            }
            LoadAsync(encoding, start, end);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            EncodingTb.ItemsSource = Encoding.GetEncodings().Select(i => i.Name).ToArray();
        }

        private async void LoadAsync(Encoding encoding, long start, long end)
        {
            if (App.ViewModel.Reader == null)
            {
                TextTb.Text = string.Empty;
                return;
            }
            TextTb.Text = encoding.GetString(await App.ViewModel.Reader.ReadAsync(start, Convert.ToInt32(end - start + 1)));
        }

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            var start = StartTb.Value;
            var end = EndTb.Value;
            if (end <= start)
            {
                MessageBox.Show("区间错误");
                return;
            }
            var picker = new Microsoft.Win32.SaveFileDialog();
            if (picker.ShowDialog() != true)
            {
                return;
            }
            ExportBtn.IsEnabled = false;
            SaveAsync(picker.FileName, start, end);
        }

        private async void SaveAsync(string fileName, long start, long end)
        {
            using var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            await App.ViewModel.Reader!.CopyToAsync(fs, start, end - start);
            ExportBtn.IsEnabled = true;
        }

        public void UpdateRange(long position, int length)
        {
            StartTb.Value = position;
            EndTb.Value = position + length;
            ViewBtn_Click(ViewBtn, new RoutedEventArgs());
        }
    }
}
