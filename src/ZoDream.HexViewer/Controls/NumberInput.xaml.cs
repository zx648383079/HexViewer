using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ZoDream.HexViewer.Controls
{
    /// <summary>
    /// NumberInput.xaml 的交互逻辑
    /// </summary>
    public partial class NumberInput : UserControl
    {
        public NumberInput()
        {
            InitializeComponent();
        }

        public long Max
        {
            get { return (long)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(long), typeof(NumberInput), new PropertyMetadata(0L));




        public long Min
        {
            get { return (long)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Min.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(long), typeof(NumberInput), new PropertyMetadata(0L));




        public uint Step
        {
            get { return (uint)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Step.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(uint), typeof(NumberInput), new PropertyMetadata((uint)1));



        public long Value
        {
            get { return (long)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(long), typeof(NumberInput), new PropertyMetadata(0L, OnValueChanged));

        public event RoutedPropertyChangedEventHandler<long>? ValueChanged;

        private CancellationTokenSource TokenSource = new();

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as NumberInput;
            tb!.ValueTb.Text = e.NewValue.ToString();
        }

        private void MinusBtn_Click(object sender, RoutedEventArgs e)
        {
            var oldVal = Value;
            var val = Value - Step;
            if (val < Min)
            {
                val = Min;
            }
            Value = Convert.ToInt32(val);
            ValueTb.Text = val.ToString();
            ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<long>(oldVal, Value));
        }

        private void PlusBtn_Click(object sender, RoutedEventArgs e)
        {
            var oldVal = Value;
            var val = Value + Step;
            if (Max > 0 && val > Max)
            {
                val = Max;
            }
            Value = Convert.ToInt32(val);
            ValueTb.Text = val.ToString();
            ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<long>(oldVal, Value));
        }

        private void ValueTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            TokenSource.Cancel();
            TokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(2000);
                if (TokenSource.IsCancellationRequested)
                {
                    return;
                }
                App.Current.Dispatcher.Invoke(() =>
                {
                    var oldVal = Value;
                    var val = Convert.ToInt64((sender as TextBox)!.Text);
                    if (val < Min)
                    {
                        val = Min;
                    }
                    else if (Max > 0 && val > Max)
                    {
                        val = Max;
                    }
                    Value = val;
                    ValueTb.Text = val.ToString();
                    ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<long>(oldVal, Value));
                });
            }, TokenSource.Token);
        }


    }
}
