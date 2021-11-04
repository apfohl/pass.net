using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Collections;
using Avalonia.Data.Converters;

namespace Pass.Converters
{
    public sealed class DiameterAndThicknessToStrokeDashArrayConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture) =>
            values.Count switch
            {
                2 when double.TryParse(values[0].ToString(), out var diameter) &&
                       double.TryParse(values[1].ToString(), out var thickness) => StrokeDashArray(diameter, thickness),
                _ => new AvaloniaList<double> { 0d }
            };

        private static AvaloniaList<double> StrokeDashArray(double diameter, double thickness)
        {
            var circumference = Math.PI * diameter;
            var lineLength = circumference * 0.75;
            var gapLength = circumference - lineLength;

            return new AvaloniaList<double> { lineLength / thickness, gapLength / thickness };
        }
    }
}