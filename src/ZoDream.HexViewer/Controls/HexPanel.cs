using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZoDream.HexViewer.Models;
using ZoDream.HexViewer.Storage;
using ZoDream.HexViewer.Utils;

namespace ZoDream.HexViewer.Controls
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ZoDream.HexViewer.Controls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ZoDream.HexViewer.Controls;assembly=ZoDream.HexViewer.Controls"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:HexPanel/>
    ///
    /// </summary>
    [TemplatePart(Name = ByteModeTbName, Type =typeof(TextBlock))]
    [TemplatePart(Name = TextHeaderTbName, Type =typeof(TextBlock))]
    [TemplatePart(Name = ByteHeaderPanelName, Type =typeof(StackPanel))]
    [TemplatePart(Name = LinePanelName, Type =typeof(StackPanel))]
    [TemplatePart(Name = BytePanelName, Type =typeof(StackPanel))]
    [TemplatePart(Name = TextPanelName, Type =typeof(StackPanel))]
    [TemplatePart(Name = ByteScrollBarName, Type =typeof(ScrollBar))]
    public class HexPanel : Control
    {
        const string ByteModeTbName = "PART_ByteModeTb";
        const string ByteHeaderPanelName = "PART_ByteHeaderPanel";
        const string TextHeaderTbName = "PART_TextHeaderTb";
        const string LinePanelName = "PART_LinePanel";
        const string BytePanelName = "PART_BytePanel";
        const string TextPanelName = "PART_TextPanel";
        const string ByteScrollBarName = "PART_ByteScrollBar";

        static HexPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HexPanel), new FrameworkPropertyMetadata(typeof(HexPanel)));
        }


        public ByteBaseMode LineMode
        {
            get { return (ByteBaseMode)GetValue(LineModeProperty); }
            set { SetValue(LineModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LineMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineModeProperty =
            DependencyProperty.Register("LineMode", typeof(ByteBaseMode), typeof(HexPanel), new PropertyMetadata(ByteBaseMode.Hex, (d, e) =>
            {
                (d as HexPanel)?.UpdateLineMode();
            }));

        public int ByteLength
        {
            get { return (int)GetValue(ByteLengthProperty); }
            set { SetValue(ByteLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ByteLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ByteLengthProperty =
            DependencyProperty.Register("ByteLength", typeof(int), typeof(HexPanel), new PropertyMetadata(16, (d, e) =>
            {
                (d as HexPanel)?.UpdateByteLength();
            }));


        public ByteBaseMode ByteMode
        {
            get { return (ByteBaseMode)GetValue(ByteModeProperty); }
            set { SetValue(ByteModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ByteMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ByteModeProperty =
            DependencyProperty.Register("ByteMode", typeof(ByteBaseMode), typeof(HexPanel), new PropertyMetadata(ByteBaseMode.Hex, (d, e) =>
            {
                (d as HexPanel)?.UpdateByteMode();
            }));


        public Encoding TextEncoding
        {
            get { return (Encoding)GetValue(TextEncodingProperty); }
            set { SetValue(TextEncodingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextEncoding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextEncodingProperty =
            DependencyProperty.Register("TextEncoding", typeof(Encoding), typeof(HexPanel), new PropertyMetadata(Encoding.ASCII, (d, e) =>
            {
                (d as HexPanel)?.UpdateEncoding();
            }));



        public bool TextLineEncode {
            get { return (bool)GetValue(TextLineEncodeProperty); }
            set { SetValue(TextLineEncodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextLineEncode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextLineEncodeProperty =
            DependencyProperty.Register("TextLineEncode", typeof(bool), typeof(HexPanel), 
                new PropertyMetadata(false, (d, e) => {
                    (d as HexPanel)?.UpdateEncoding();
                }));



        public IByteStream? Source
        {
            get { return (IByteStream)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(IByteStream), typeof(HexPanel), new PropertyMetadata(null, (d, e) =>
            {
                if (LocalizedLangExtension.IsDesignMode)
                {
                    return;
                }
                (d as HexPanel)?.UpdateSource();
            }));

        public long Position
        {
            get { return (long)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(long), typeof(HexPanel), new PropertyMetadata(0L, (d, e) =>
            {
                (d as HexPanel)?.GotoPosition((long)e.NewValue);
            }));


        private TextBlock? ByteModeTb;
        private TextBlock? TextHeaderTb;
        private StackPanel? ByteHeaderPanel;
        private StackPanel? LinePanel;
        private StackPanel? BytePanel;
        private StackPanel? TextPanel;
        private ScrollBar? ByteScrollBar;
        private byte[]? OriginalBuffer;
        private Action<Point, bool>? OnMouseMoveEnd;
        public event RoutedPropertyChangedEventHandler<int>? SelectionChanged;
        public event RoutedPropertyChangedEventHandler<string>? DropChanged;
        private CancellationTokenSource TokenSource = new();

        public bool IsSelectionActive { get; private set; } = false;

        public Tuple<byte[], long> SelectionByte
        {
            get
            {
                var items = new List<byte>();
                var start = -1;
                ForEachByte((item, j) =>
                {
                    if (!item.IsActive)
                    {
                        return;
                    }
                    if (start < 0)
                    {
                        start = j;
                    }
                    items.Add(item.OriginalByte);
                });
                return Tuple.Create(items.ToArray(), start < 0 ? -1L : (start + Position));
            }
        }

        private double ByteWidth
        {
            get
            {
                return App.ViewModel.Get(ByteMode).Length * FontSize * .7 + 10;
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
                return Math.Max((int)Math.Floor(BytePanel!.ActualHeight / ByteHeight), 1);
            }
        }

        private long PageCount
        {
            get
            {
                return (long)Math.Ceiling((double)LineCount / PageLineCount);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ByteModeTb = GetTemplateChild(ByteModeTbName) as TextBlock;
            TextHeaderTb = GetTemplateChild(TextHeaderTbName) as TextBlock;
            ByteHeaderPanel = GetTemplateChild(ByteHeaderPanelName) as StackPanel;
            LinePanel = GetTemplateChild(LinePanelName) as StackPanel;
            BytePanel = GetTemplateChild(BytePanelName) as StackPanel;
            TextPanel = GetTemplateChild(TextPanelName) as StackPanel;
            ByteScrollBar = GetTemplateChild(ByteScrollBarName) as ScrollBar;

            if (ByteScrollBar != null)
            {
                ByteScrollBar.ValueChanged += ByteScrollBar_ValueChanged;
            }
            if (ByteModeTb != null)
            {
                ByteModeTb.MouseDown += ByteModeTb_MouseDown;
            }
            if (TextHeaderTb != null)
            {
                TextHeaderTb.MouseDown += TextHeaderTb_MouseDown;
            }
            if (LinePanel != null)
            {
                LinePanel.MouseWheel += BytePanel_MouseWheel;
            }
            if (BytePanel != null)
            {
                BytePanel.MouseWheel += BytePanel_MouseWheel;
                BytePanel.MouseDown += BytePanel_MouseDown;
                BytePanel.MouseMove += BytePanel_MouseMove;
                BytePanel.MouseUp += BytePanel_MouseUp;
            }
            if (TextPanel != null)
            {
                TextPanel.MouseWheel += BytePanel_MouseWheel;
            }
            ContextMenu.Visibility = Visibility.Collapsed;
            UpdateByteHeader();
            UpdateTextHeader();
        }


        public void Refresh(bool sizeChanged = false)
        {
            if (ByteScrollBar is null)
            {
                return;
            }
            if (sizeChanged)
            {
                var pageCount = PageCount;
                ByteScrollBar.Maximum = LineCount;
                ByteScrollBar.Value = 0;
                ByteScrollBar.Visibility = pageCount > 1 ? Visibility.Visible : Visibility.Collapsed;
            }
            GotoPosition(Position);
        }

        private void UpdateBytes(IList<byte> items, int lineCount)
        {
            var format = App.ViewModel.Get(ByteMode);
            ForEachChildren(BytePanel!, lineCount, (panel, i) =>
            {
                panel.Height = ByteHeight;
                var start = i * ByteLength;
                ForEachChildren(panel, Math.Min(items.Count - start, ByteLength), (tb, j) =>
                {
                    tb.Width = ByteWidth;
                    tb.FontSize = FontSize;
                    var itemIndex = start + j;
                    tb.OriginalByte = items[itemIndex];
                    tb.OriginalPosition = Position + itemIndex;
                    tb.Content = format.Format(items[itemIndex], false, true);
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
            var format = App.ViewModel.Get(LineMode);
            ForEachChildren(LinePanel!, lineCount, (tb, i) =>
            {
                tb.Height = ByteHeight;
                tb.FontSize = FontSize;
                tb.Content = format.Format(start + (i * ByteLength), true);
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
            ForEachChildren(TextPanel!, lineCount, (tb, i) =>
            {
                tb.Height = ByteHeight;
                tb.FontSize = FontSize;
                var start = i * ByteLength;
                var bytes = new byte[Math.Min(items.Count - start, ByteLength)];
                for (int j = 0; j < bytes.Length; j++)
                {
                    bytes[j] = items[start + j];
                }
                var str = TextEncoding.GetString(bytes);
                tb.Content = TextLineEncode ? str.Replace("\r", "\\r").Replace("\n", "\\n") : str;
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
            TextHeaderTb!.Text = $"文本({TextEncoding.EncodingName})";
        }

        private void UpdateByteHeader()
        {
            ByteModeTb!.Text = App.ViewModel.Get(ByteMode).Name;
            var byteFormat = 16;
            ForEachChildren(ByteHeaderPanel!, ByteLength, (tb, i) =>
            {
                tb.Width = ByteWidth;
                tb.FontSize = FontSize;
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

        private void ForEachByte(Action<ByteLabel, int> func)
        {
            var i = 0;
            foreach (StackPanel item in BytePanel!.Children)
            {
                foreach (ByteLabel it in item.Children)
                {
                    if (it.Visibility == Visibility.Visible)
                    {
                        func.Invoke(it, i);
                        i++;
                    }
                }
            }
        }

        private void UpdateSource()
        {
            var pageCount = PageCount;
            ByteScrollBar!.Maximum = LineCount;
            ByteScrollBar.Value = 0;
            ByteScrollBar.Visibility = pageCount > 1 ? Visibility.Visible : Visibility.Collapsed;
            if (Source == null)
            {
                ContextMenu.Visibility = Visibility.Collapsed;
                UpdateByteSource(Array.Empty<byte>());
                return;
            }
            GotoPosition(0);
        }

        private void UpdateByteSource(byte[] buffer, long position = 0)
        {
            var lineCount = (int)Math.Ceiling((double)buffer.Length / ByteLength);
            OriginalBuffer = buffer;
            Position = position;
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
            UpdateLine(Position, (int)Math.Ceiling((double)OriginalBuffer.Length / ByteLength));
        }

        private void UpdateByteLength()
        {
            UpdateByteHeader();
            Refresh(true);
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
                ByteBaseMode.Binary => ByteBaseMode.Octal,
                ByteBaseMode.Octal => ByteBaseMode.Decimal,
                ByteBaseMode.Hex => ByteBaseMode.Binary,
                _ => ByteBaseMode.Hex,
            };
        }

        private void ToggleTextEncoding()
        {
            if (TextEncoding == Encoding.ASCII)
            {
                TextEncoding = Encoding.UTF8;
            }
            else if (TextEncoding == Encoding.UTF8)
            {
                TextEncoding = Encoding.Unicode;
            }
            else
            {
                TextEncoding = Encoding.ASCII;
            }
        }

        private void ToggleLineMode()
        {
            LineMode = LineMode == ByteBaseMode.Decimal ? ByteBaseMode.Hex : ByteBaseMode.Decimal;
        }

        private void ByteScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Source == null)
            {
                return;
            }
            TokenSource.Cancel();
            TokenSource = new CancellationTokenSource();
            var token = TokenSource.Token;
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(200);
                if (token.IsCancellationRequested)
                {
                    return;
                }
                App.Current.Dispatcher.Invoke(() =>
                {
                    GotoLine((long)(e.NewValue > e.OldValue ? Math.Ceiling(e.NewValue) : Math.Floor(e.NewValue)), token);
                });
            }, token);

        }

        public void GotoPage(double index, CancellationToken cancellationToken = default)
        {
            GotoLine((long)Math.Floor(index * PageLineCount), cancellationToken);
        }

        public void GotoLine(long index, CancellationToken cancellationToken = default)
        {
            if (index < 0)
            {
                index = 0;
            }
            GotoPosition(index * ByteLength, cancellationToken);
        }

        public async void GotoPosition(long index, CancellationToken cancellationToken = default)
        {
            if (Source == null)
            {
                return;
            }
            ByteScrollBar!.Value = Math.Floor((double)index / ByteLength);
            var buffer = await Source.ReadAsync(index, PageLineCount * ByteLength, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            UpdateByteSource(buffer, index);
        }


        public void Select(long position, int count, bool IsFinish = true)
        {
            var i = position - Position;
            var end = i + count;
            var len = 0;
            ForEachByte((item, j) =>
            {
                var isActive = j >= i && j < end;
                item.IsActive = isActive;
                if (isActive)
                {
                    len++;
                }
            });
            IsSelectionActive = len > 0;
            if (IsFinish)
            {
                SelectionChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<int>(0, len));
                var isMenuVisible = Source == null || len < 1;
                if (isMenuVisible)
                {
                    ContextMenu.StaysOpen = false;
                }
                ContextMenu.Visibility = isMenuVisible ? Visibility.Collapsed : Visibility.Visible;

            }
        }

        public void Select()
        {
            Select(0L, 0);
        }

        public void Select(Point begin, Point end, bool IsFinish = true)
        {
            var bY = Math.Floor(begin.Y / ByteHeight);
            var eY = Math.Floor(end.Y / ByteHeight);
            var bX = Math.Floor(begin.X / ByteWidth);
            var eX = Math.Floor(end.X / ByteWidth);
            if (bY < eY || (bY == eY && bX < eX))
            {
                var b = Position + bY * ByteLength + bX;
                Select(Convert.ToInt64(b), Convert.ToInt32(Position + eY * ByteLength + eX + 1 - b), IsFinish);
            }
            else
            {
                var e = Position + eY * ByteLength + eX;
                Select(Convert.ToInt64(e), Convert.ToInt32(Position + bY * ByteLength + bX + 1 - e), IsFinish);
            }
        }

        private void BytePanel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ByteScrollBar_ValueChanged(ByteScrollBar!, new RoutedPropertyChangedEventArgs<double>(ByteScrollBar!.Value, ByteScrollBar.Value - (e.Delta / 120)));
        }

        private void ByteModeTb_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ToggleByteMode();
        }

        private void TextHeaderTb_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ToggleTextEncoding();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Refresh();
        }

        private void BytePanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }
            var start = e.GetPosition(BytePanel);
            OnMouseMoveEnd = (end, up) =>
            {
                Select(start, end, up);
            };
        }

        private void BytePanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }
            var end = e.GetPosition(BytePanel);
            OnMouseMoveEnd?.Invoke(end, true);
        }

        private void BytePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }
            var end = e.GetPosition(BytePanel);
            OnMouseMoveEnd?.Invoke(end, false);
        }
    }
}
