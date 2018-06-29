using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VarPControl.Converter
{
    public class PercentToAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var percent = double.Parse(value.ToString());
            if (percent >= 1) return 360.0D;
            return percent * 360;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static PercentToAngleConverter _inst;
        public static PercentToAngleConverter GetInstance()
        {
            if (_inst == null)
            {
                _inst = new PercentToAngleConverter();
            }
            return _inst;
        }

    }
}
