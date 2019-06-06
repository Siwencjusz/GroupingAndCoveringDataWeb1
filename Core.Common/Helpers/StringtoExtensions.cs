using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using static System.String;

namespace Core.Common.Helpers
{
    public static class StringtoExtensions
    {
        public static double StringToDouble(this string valueString)
        {
            return IsNullOrEmpty(valueString) ? default(double) 
                : double.Parse(valueString, NumberStyles.Any, CultureInfo.InvariantCulture);
        }
        public static string StringToDoubleFormat(this string valueString)
        {
            return IsNullOrEmpty(valueString) ? default(double).ToString(CultureInfo.InvariantCulture) 
                : double.Parse(valueString, NumberStyles.Any, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
        }
        public static string DoubleToString(this double valueString)
        {
            return valueString.Equals(0) ? string.Empty 
                : valueString.ToString(CultureInfo.InvariantCulture);
        }
        public static string[] SplitString(this string str, string separator)
        {
            var separators = new char[] { separator.ToCharArray().First() };
            return ConvertWhitespacesToSingleSpaces(str).ToDecimalFormat()
                .Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string ConvertWhitespacesToSingleSpaces(this string value)
        {
            return Regex.Replace(value, @"\s+", " ");
        }

        public static string ToDecimalFormat(this string value)
        {
            return value.Replace('.', ',');
        }
    }
}
