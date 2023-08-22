using System;
using System.Globalization;
using System.IO;
using R3Modeller.Core.Utils;

namespace R3Modeller.Converters {
    public class FileExistsToBoolConverter : SingletonValueConverter<FileExistsToBoolConverter> {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return File.Exists((string) value).Box();
        }
    }
}