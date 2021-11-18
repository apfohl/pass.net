using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Pass.Controls;

public sealed class ProgressSpinner : ContentControl
{
    public static readonly StyledProperty<double> DiameterProperty =
        AvaloniaProperty.Register<ProgressSpinner, double>(nameof(Label), 100d);

    public double Diameter
    {
        get => GetValue(DiameterProperty);
        set => SetValue(DiameterProperty, value);
    }

    public static readonly StyledProperty<IBrush> ColorProperty =
        AvaloniaProperty.Register<ProgressSpinner, IBrush>(nameof(Label), Brushes.Black);

    public IBrush Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public static readonly StyledProperty<double> ThicknessProperty =
        AvaloniaProperty.Register<ProgressSpinner, double>(nameof(Label), 1d);

    public double Thickness
    {
        get => GetValue(ThicknessProperty);
        set => SetValue(ThicknessProperty, value);
    }

    public static readonly StyledProperty<PenLineCap> CapProperty =
        AvaloniaProperty.Register<ProgressSpinner, PenLineCap>(nameof(Label));

    public PenLineCap Cap
    {
        get => GetValue(CapProperty);
        set => SetValue(CapProperty, value);
    }

    public ProgressSpinner() => AvaloniaXamlLoader.Load(this);
}