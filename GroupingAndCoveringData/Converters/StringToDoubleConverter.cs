using System;
using System.Globalization;
using System.Windows.Data;
using Core.Common.Helpers;

namespace GroupingAndCoveringData.Converters
{
    [ValueConversion(typeof(string), typeof(double?))]
    public class StringToDoubleConverter : BaseConverter, IValueConverter
    {
        public object Convert(
            object value, 
            Type targetType, 
            object parameter, 
            CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(
            object value, 
            Type targetType, 
            object parameter, 
            CultureInfo culture)
        {
            return value.ObjectToDouble();            
        }
    }
}
