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
    public class GenderOptionConverterUltra : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)value == "Male")
            {
                return "男";
            }
            else if ((string)value == "Female")
            {
                return "女";
            }
            else if ((string)value == "Unlimited")
            {
                return "不限";
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)value == "男")
            {
                return "Male";
            }
            else if ((string)value == "女")
            {
                return "Female";
            }
            else if ((string)value == "不限")
            {
                return "Unlimited";
            }
            else
            {
                return null;
            }
        }
    }
}
