using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace R3Modeller.Converters {
    public class FilePathToWindowTitleConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null || value == DependencyProperty.UnsetValue) {
                return DependencyProperty.UnsetValue;
            }

            if (value is string fullName) {
                if (parameter is string format) {
                    return string.Format(format, fullName);
                }
                else {
                    return fullName;
                }
            }
            else {
                return "DEBUG_ERROR_NOT_STRING: " + value.GetType() + " -> " + value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException("Cannot convert back window title to file path");
        }
    }
}
