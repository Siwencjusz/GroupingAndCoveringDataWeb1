namespace Core.Common.Helpers
{
    public static class DoubleExtensions
    {
        public static double? ObjectToDouble(this object value)
        {
            if (value is double toDouble)
            {
                return toDouble;
            }
            if (value == null) return default(double?);
            if (double.TryParse(value.ToString(), out var parsed))
            {
                return parsed;
            }

            if (double.TryParse(value.ToString(), System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out parsed))
            {
                return parsed;
            }

            return default(double?);
        }
    
    }
}
