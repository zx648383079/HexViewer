using System;
using System.Collections.Generic;
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

namespace ZoDream.HexViewer.Pages
{
    /// <summary>
    /// SearchView.xaml 的交互逻辑
    /// </summary>
    public partial class SearchView : Window
    {
        public SearchView(long positon)
        {
            LastPosition = positon;
            InitializeComponent();
        }

        public event RoutedPropertyChangedEventHandler<long>? OnFound;

        private long LastPosition = -1;

        private void ByteRadio_Checked(object sender, RoutedEventArgs e)
        {
            BytePanel.Visibility = Visibility.Visible;
            TextPanel.Visibility = Visibility.Collapsed;
        }

        private void TextRadio_Checked(object sender, RoutedEventArgs e)
        {
            BytePanel.Visibility = Visibility.Collapsed;
            TextPanel.Visibility = Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            EncodingTb.ItemsSource = Encoding.GetEncodings().Select(i => i.Name).ToArray();
            ByteTypeTb.ItemsSource = App.ViewModel.ByteModeItems;
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            FindAsync(GetByte(), 0, false);
        }

        private async void FindAsync(byte[] bytes, long positon, bool reverse)
        {
            LastPosition = await App.ViewModel.Reader.FindAsync(bytes, positon, reverse);
            if (LastPosition >= 0)
            {
                OnFound?.Invoke(this, new RoutedPropertyChangedEventArgs<long>(positon, LastPosition));
            }
        }

        private byte[] GetByte()
        {
            if (TextRadio.IsChecked == true)
            {
                var encoding = string.IsNullOrWhiteSpace(EncodingTb.Text) ? Encoding.Default : Encoding.GetEncoding(EncodingTb.Text.Trim());
                return encoding.GetBytes(TextTb.Text);
            }
            return (ByteTypeTb.SelectedItem as ByteModeItem).ToByte(TextTb.Text);
        }

        private void PreviousBtn_Click(object sender, RoutedEventArgs e)
        {
            FindAsync(GetByte(), LastPosition, true);
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            FindAsync(GetByte(), LastPosition, false);
        }
    }
}
