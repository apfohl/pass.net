using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Pass.Views
{
    public sealed class Spinner : UserControl
    {
        private readonly Path spinner;

        public static readonly StyledProperty<int> SizeProperty = AvaloniaProperty.Register<Spinner, int>(nameof(Size));

        public int Size
        {
            get => GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        static Spinner() => SizeProperty.Changed.AddClassHandler<Spinner>((b, e) => b.OnSizeChanged(e));

        public Spinner()
        {
            AvaloniaXamlLoader.Load(this);
            spinner = this.FindControl<Path>("Spinner");
        }

        private void OnSizeChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var size = (int?) e.NewValue ?? 10;
            spinner.Data =
                Geometry.Parse($"M0,0 A{size},{size} 0 1 0 {size * Math.Sin(30)},{size * Math.Cos(30)}");
        }
    }
}