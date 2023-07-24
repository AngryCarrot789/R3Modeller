using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace R3Modeller.Converters {
    public class SingleItemToListConverter : IValueConverter {
        public static SingleItemToListConverter Instance { get; } = new SingleItemToListConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value != DependencyProperty.UnsetValue ? new List<object>() {value} : DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value != DependencyProperty.UnsetValue ? ((List<object>) value)?[0] : DependencyProperty.UnsetValue;
        }
    }
}