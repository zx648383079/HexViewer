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
using ZoDream.HexViewer.Storage;
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
            /*SaveBtn.Visibility = */PropertyBtn.Visibility = PreviewBtn.Visibility = SearchBtn.Visibility =
                    EditBtn.Visibility = string.IsNullOrWhiteSpace(file) ?
                    Visibility.Collapsed : Visibility.Visible;
            HexTb.Source = ViewModel.Reader;
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
            var model = new SettingViewModel()
            {
                FontSize = HexTb.FontSize,
                ByteLength = HexTb.ByteLength,
                LineMode = ViewModel.Get(HexTb.LineMode),
                ByteMode = ViewModel.Get(HexTb.ByteMode),
                TextEncoding = HexTb.TextEncoding.EncodingName
            };
            var page = new SettingView(model);
            page.Show();
            model.PropertyChanged += (_, e) =>
            {
                switch (e.PropertyName)
                {
                    case "FontSize":
                        HexTb.FontSize = model.FontSize;
                        HexTb.Refresh(true);
                        break;
                    case "ByteLength":
                        HexTb.ByteLength = model.ByteLength;
                        break;
                    case "LineMode":
                        HexTb.LineMode = model.LineMode.Mode;
                        break;
                    case "ByteMode":
                        HexTb.ByteMode = model.ByteMode.Mode;
                        break;
                    case "TextEncoding":
                        HexTb.TextEncoding = Encoding.GetEncoding(model.TextEncoding);
                        break;
                    default:
                        break;
                }
            };
        }

        private void PreviewBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new PreviewView();
            page.ShowDialog();
        }

        private void PropertyBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new PropertyView();
            page.ShowDialog();
        }

        private void HexTb_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            ToolTipTb.Text = e.NewValue < 1 ? string.Empty : $"已选中 {e.NewValue} 个子节";
            DeleteBtn.Visibility = e.NewValue < 1 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Reader == null || !HexTb.IsSelectionActive)
            {
                return;
            }
            TapDelete();
        }

        private void TapCopy()
        {
            if (ViewModel.Reader == null || !HexTb.IsSelectionActive)
            {
                return;
            }
            var select = HexTb.SelectionByte;
            var format = ViewModel.Get(HexTb.ByteMode);
            Clipboard.SetText(string.Join(" ", select.Item1.Select(i => format.Format(i, false, true))));
            MessageBox.Show("复制成功");
        }

        private void TapPaste()
        {
            if (ViewModel.Reader == null || !HexTb.IsSelectionActive)
            {
                return;
            }
            var text = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            TapSelectEdit(text);
        }

        private void TapSelectEdit(string text)
        {
            var page = new EditView();
            var select = HexTb.SelectionByte;
            page.UpdateRange(select.Item2 < 0 ? HexTb.Position : select.Item2, select.Item1.Length, text);
            if (page.ShowDialog() == true)
            {
                HexTb.Refresh(true);
            }
        }

        private void TapSelectPreview()
        {
            var page = new PreviewView();
            var select = HexTb.SelectionByte;
            page.UpdateRange(select.Item2 < 0 ? HexTb.Position : select.Item2, select.Item1.Length);
            page.ShowDialog();
        }

        private async void DeleteAsync(IByteStream stream, long position, int count)
        {
            await stream.DeleteAsync(position, count);
            HexTb.Select();
            HexTb.Refresh(true);
            MessageBox.Show("删除成功");
        }

        private void CommandBinding_Copy(object sender, ExecutedRoutedEventArgs e)
        {
            if (ViewModel.Reader == null || !HexTb.IsSelectionActive)
            {
                return;
            }
            TapCopy();
        }

        private void CommandBinding_Paste(object sender, ExecutedRoutedEventArgs e)
        {
            if (ViewModel.Reader == null || !HexTb.IsSelectionActive)
            {
                return;
            }
            TapPaste();
        }

        private void TapDelete()
        {
            if (ViewModel.Reader == null)
            {
                return;
            }
            var select = HexTb.SelectionByte;
            DeleteAsync(ViewModel.Reader, select.Item2, select.Item1.Length);
        }

        private void HexMenu_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as MenuItem)!.Header)
            {
                case "复制":
                    TapCopy();
                    break;
                case "粘贴":
                    TapPaste();
                    break;
                case "删除":
                    TapDelete();
                    break;
                case "编辑":
                    TapSelectEdit(string.Empty);
                    break;
                case "查看":
                    TapSelectPreview();
                    break;
                default:
                    break;
            }
        }

        private void HelpBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new HelpView();
            page.Show();
        }
    }
}
