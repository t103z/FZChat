using FZChat.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FZChat.Client.ViewModel.Converter
{
    [ValueConversion(typeof(string), typeof(GenderOption))]
    public class GenderOptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)value == "男")
            {
                return GenderOption.Male;
            }
            else if ((string)value == "女")
            {
                return GenderOption.Female;
            }
            else
            {
                return null;
            }
        }
    }
}
