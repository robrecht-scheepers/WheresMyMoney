﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WMM.WPF.Converters
{
    public class AmountColorConverter : IValueConverter
    {
        public AmountColorConverter()
        {
            PositiveColor = Colors.Green;
            NegativeColor = Colors.Red;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var amount = System.Convert.ToDouble(value);

            return new SolidColorBrush(amount < 0 
                ? NegativeColor
                : PositiveColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public Color PositiveColor { get; set; }
        public Color NegativeColor { get; set; }
    }
}
