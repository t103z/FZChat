using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace FZChat.Client.ViewModel.Converter
{
    [ValueConversion(typeof(Brush), typeof(string))]
    public class ChatBorderColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string sender = (string)values[0];
            string currentUser = (string)values[1];
            if (sender == currentUser)
            {
                return Brushes.Green;
            }
            else
            {
                return Brushes.Violet;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
