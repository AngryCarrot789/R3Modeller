using System;
using System.Windows;
using System.Windows.Data;

namespace R3Modeller.Controls.TreeViews.Controls {
    class ThicknessLeftConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value is int)
                return new Thickness {Left = (int) value};
            if (value is double)
                return new Thickness {Left = (double) value};
            return new Thickness();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}