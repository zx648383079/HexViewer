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
using ZoDream.HexViewer.Storage;

namespace ZoDream.HexViewer.Controls
{
    /// <summary>
    /// HexEditor.xaml 的交互逻辑
    /// </summary>
    public partial class HexEditor : UserControl
    {
        public HexEditor()
        {
            InitializeComponent();
        }



        public BaseMode LineMode
        {
            get { return (BaseMode)GetValue(LineModeProperty); }
            set { SetValue(LineModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LineMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineModeProperty =
            DependencyProperty.Register("LineMode", typeof(BaseMode), typeof(HexEditor), new PropertyMetadata(BaseMode.Hex, (d, e) =>
            {
                (d as HexEditor)?.UpdateLineMode();
            }));

        public int ByteLength
        {
            get { return (int)GetValue(ByteLengthProperty); }
            set { SetValue(ByteLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ByteLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ByteLengthProperty =
            DependencyProperty.Register("ByteLength", typeof(int), typeof(HexEditor), new PropertyMetadata(16, (d, e) =>
            {
                (d as HexEditor)?.UpdateByteLength();
            }));


        public BaseMode ByteMode
        {
            get { return (BaseMode)GetValue(ByteModeProperty); }
            set { SetValue(ByteModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ByteMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ByteModeProperty =
            DependencyProperty.Register("ByteMode", typeof(BaseMode), typeof(HexEditor), new PropertyMetadata(BaseMode.Hex, (d, e) =>
            {
                (d as HexEditor)?.UpdateByteMode();
            }));


        public Encoding TextEncoding
        {
            get { return (Encoding)GetValue(TextEncodingProperty); }
            set { SetValue(TextEncodingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextEncoding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextEncodingProperty =
            DependencyProperty.Register("TextEncoding", typeof(Encoding), typeof(HexEditor), new PropertyMetadata(Encoding.ASCII, (d, e) =>
            {
                (d as HexEditor)?.UpdateEncoding();
            }));

        public IByteStream? Source
        {
            get { return (IByteStream)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(IByteStream), typeof(HexEditor), new PropertyMetadata(null, (d, e) =>
            {
                (d as HexEditor)?.UpdateSource();
            }));

        private byte[]? OriginalBuffer;
        private long StartPosition = 0;

        private double ByteWidth
        {
            get
            {
                return ByteMode switch
                {
                    BaseMode.Octal => 4 * 10,
                    BaseMode.Decimal => 3 * 10,
                    BaseMode.Hex => 2 * 10,
                    _ => (double)(8 * 10),
                };
            }
        }

        private double ByteHeight
        {
            get
            {
                return FontSize + 20;
            }
        }

        private long LineCount
        {
            get
            {
                if (Source == null)
                {
                    return 0;
                }
                return (long)Math.Ceiling((double)Source.Length / ByteLength);
            }
        }

        private int PageLineCount
        {
            get
            {
                return Math.Max((int)Math.Floor(BytePanel.ActualHeight / ByteHeight), 1);
            }
        }

        private long PageCount
        {
            get
            {
                return (long)Math.Ceiling((double)LineCount / PageLineCount);
            }
        }

        private int FormatBaseMode(BaseMode mode)
        {
            return mode switch
            {
                BaseMode.Binary => 2,
                BaseMode.Octal => 8,
                BaseMode.Decimal => 10,
                BaseMode.Hex => 16,
                _ => 10,
            };
        }

        private int PadBaseMode(BaseMode mode)
        {
            return mode switch
            {
                BaseMode.Binary => 8,
                BaseMode.Octal => 4,
                BaseMode.Decimal => 3,
                BaseMode.Hex => 2,
                _ => 3,
            };
        }

        private string PrefixBaseMode(BaseMode mode)
        {
            return mode switch
            {
                BaseMode.Binary => "0b",
                BaseMode.Octal => "0",
                BaseMode.Decimal => string.Empty,
                BaseMode.Hex => "0x",
                _ => string.Empty,
            };
        }

        private void UpdateBytes(IList<byte> items, int lineCount)
        {
            var byteFormat = FormatBaseMode(ByteMode);
            var pad = PadBaseMode(ByteMode);
            ForEachChildren(BytePanel, lineCount, (panel, i) =>
            {
                panel.Height = ByteHeight;
                var start = i * ByteLength;
                ForEachChildren(panel, Math.Min(items.Count - start, ByteLength), (tb, j) =>
                {
                    tb.Width = ByteWidth;
                    tb.Content = Convert.ToString(items[start + j], byteFormat).PadLeft(pad, '0');
                }, i =>
                {
                    return new ByteLabel()
                    {
                        TextAlignment = HorizontalAlignment.Center,
                    };
                });
            }, i =>
            {
                return new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                };
            });
        }

        private void UpdateLine(long start, int lineCount)
        {
            var lineFormat = FormatBaseMode(LineMode);
            var linePrefix = PrefixBaseMode(LineMode);
            ForEachChildren(LinePanel, lineCount, (tb, i) =>
            {
                tb.Height = ByteHeight;
                tb.Content = linePrefix + Convert.ToString(start + (i * ByteLength), lineFormat);
            }, i =>
            {
                return new ByteLabel()
                {
                    TextAlignment = HorizontalAlignment.Right,
                    Padding = new Thickness(0, 0, 5, 0),
                };
            });
        }

        private void UpdateText(IList<byte> items, int lineCount)
        {
            ForEachChildren(TextPanel, lineCount, (tb, i) =>
            {
                tb.Height = ByteHeight;
                var start = i * ByteLength;
                var bytes = new byte[Math.Min(items.Count - start, ByteLength)];
                for (int j = 0; j < bytes.Length; j++)
                {
                    bytes[j] = items[start + j];
                }
                tb.Content = TextEncoding.GetString(bytes);
            }, i =>
            {
                return new ByteLabel()
                {
                    TextAlignment = HorizontalAlignment.Left,
                };
            }, true);
        }

        private void UpdateTextHeader()
        {
            TextHeaderTb.Text = $"文本({TextEncoding.EncodingName})";
        }

        private void UpdateByteHeader()
        {
            ByteModeTb.Text = ByteMode switch
            {
                BaseMode.Binary => "二进制",
                BaseMode.Octal => "八进制",
                BaseMode.Decimal => "十进制",
                BaseMode.Hex => "十六进制",
                _ => "?",
            };
            var byteFormat = 16;
            ForEachChildren(ByteHeaderPanel, ByteLength, (tb, i) =>
            {
                tb.Width = ByteWidth;
                tb.Content = Convert.ToString(i, byteFormat);
            }, i =>
            {
                return new ByteLabel()
                {
                    TextAlignment = HorizontalAlignment.Center,
                    FontWeight = FontWeights.Bold,
                };
            }, true);
        }

        private void ForEachChildren<T>(Panel panel, int count, Action<T, int> updateFn, Func<int, T> addFn, bool overRemove = false) 
            where T : UIElement
        {
            if (overRemove)
            {
                for (int j = panel.Children.Count - 1; j >= count; j--)
                {
                    panel.Children.RemoveAt(j);
                }
            }
            var i = -1;
            foreach (T item in panel.Children)
            {
                if (item == null)
                {
                    continue;
                }
                i++;
                if (i >= count)
                {
                    item.Visibility = Visibility.Collapsed;
                    continue;
                }
                item.Visibility = Visibility.Visible;
                updateFn.Invoke(item, i);
            }
            while (i < count - 1)
            {
                i++;
                var item = addFn.Invoke(i);
                panel.Children.Add(item);
                updateFn.Invoke(item, i);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateByteHeader();
            UpdateTextHeader();
        }

        private void UpdateSource()
        {
            var pageCount = PageCount;
            ByteScrollBar.Maximum = LineCount;
            ByteScrollBar.Value = 0;
            ByteScrollBar.Visibility = pageCount > 1 ? Visibility.Visible : Visibility.Collapsed;
            if (Source == null)
            {
                UpdateByteSource(Array.Empty<byte>());
                return;
            }
            GotoPage(0);
        }

        private void UpdateByteSource(byte[] buffer, long position = 0)
        {
            var lineCount = (int)Math.Ceiling((double)buffer.Length / ByteLength);
            OriginalBuffer = buffer;
            StartPosition = position;
            UpdateLine(position, lineCount);
            UpdateBytes(buffer, lineCount);
            UpdateText(buffer, lineCount);
        }


        private void UpdateByteMode()
        {
            UpdateByteHeader();
            if (OriginalBuffer == null)
            {
                return;
            }
            UpdateBytes(OriginalBuffer, (int)Math.Ceiling((double)OriginalBuffer.Length / ByteLength));
        }

        private void UpdateLineMode()
        {
            if (OriginalBuffer == null)
            {
                return;
            }
            UpdateLine(StartPosition, (int)Math.Ceiling((double)OriginalBuffer.Length / ByteLength));
        }

        private void UpdateByteLength()
        {
            UpdateByteHeader();
            GotoPosition(StartPosition);
        }

        private void UpdateEncoding()
        {
            UpdateTextHeader();
            if (OriginalBuffer == null)
            {
                return;
            }
            UpdateText(OriginalBuffer, (int)Math.Ceiling((double)OriginalBuffer.Length / ByteLength));
        }

        private void ToggleByteMode()
        {
            ByteMode = ByteMode switch
            {
                BaseMode.Binary => BaseMode.Octal,
                BaseMode.Octal => BaseMode.Decimal,
                BaseMode.Hex => BaseMode.Binary,
                _ => BaseMode.Hex,
            };
        }

        private void ToggleTextEncoding()
        {
            if (TextEncoding == Encoding.ASCII)
            {
                TextEncoding = Encoding.UTF8;
            } else if (TextEncoding == Encoding.UTF8)
            {
                TextEncoding = Encoding.Unicode;
            } else
            {
                TextEncoding = Encoding.ASCII;
            }
        }

        private void ToggleLineMode()
        {
            LineMode = LineMode == BaseMode.Decimal ? BaseMode.Hex : BaseMode.Decimal;
        }

        private void ByteScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Source == null)
            {
                return;
            }
            GotoLine((long)(e.NewValue > e.OldValue ? Math.Ceiling(e.NewValue) : Math.Floor(e.NewValue)));
        }

        public void GotoPage(double index)
        {
            GotoLine((long)Math.Floor(index * PageLineCount));
        }

        public void GotoLine(long index)
        {
            if (index < 0)
            {
                index = 0;
            }
            GotoPosition(index * ByteLength);
        }

        public async void GotoPosition(long index)
        {
            if (Source == null)
            {
                return;
            }
            ByteScrollBar.Value = Math.Floor((double)index / ByteLength);
            UpdateByteSource(await Source.ReadAsync(index, PageLineCount * ByteLength), index);
        }

        private void BytePanel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ByteScrollBar_ValueChanged(ByteScrollBar, new RoutedPropertyChangedEventArgs<double>(ByteScrollBar.Value, ByteScrollBar.Value - (e.Delta / 120)));
        }

        private void ByteModeTb_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ToggleByteMode();
        }

        private void TextHeaderTb_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ToggleTextEncoding();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GotoPosition(StartPosition);
        }
    }


    public enum BaseMode
    {
        Binary,
        Octal,
        Decimal,
        Hex
    }
}
