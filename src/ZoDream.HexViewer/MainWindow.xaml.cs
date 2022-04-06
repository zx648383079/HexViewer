using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZoDream.HexViewer.Pages;
using ZoDream.HexViewer.ViewModels;

namespace ZoDream.HexViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public MainViewModel ViewModel = App.ViewModel;

        private void HexTb_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Link;
            e.Handled = true;
        }

        private void HexTb_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }
            var items = (IEnumerable<string>)e.Data.GetData(DataFormats.FileDrop);
            if (items == null)
            {
                return;
            }
            OpenFile(items.First());
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            var picker = new OpenFileDialog();
            if (picker.ShowDialog() != true)
            {
                return;
            }
            OpenFile(picker.FileName);
        }

        private void OpenFile(string file)
        {
            ViewModel.FileName = file;
            if (string.IsNullOrWhiteSpace(file))
            {
                PreviewBtn.Visibility = SearchBtn.Visibility = SaveBtn.Visibility = DeleteBtn.Visibility = EditBtn.Visibility = Visibility.Collapsed;
            } else
            {
                PreviewBtn.Visibility = SearchBtn.Visibility = SaveBtn.Visibility = DeleteBtn.Visibility = EditBtn.Visibility = Visibility.Visible;
            }
            
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new EditView();
            if (page.ShowDialog() == true)
            {
                HexTb.Refresh(true);
            }
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new SearchView(HexTb.Position);
            page.OnFound += (sender, e) =>
            {
                HexTb.GotoPosition(e.NewValue);
            };
            page.Show();
        }

        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new SettingView();
            page.Show();
        }

        private void PreviewBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new PreviewView();
            page.ShowDialog();
        }
    }
}
