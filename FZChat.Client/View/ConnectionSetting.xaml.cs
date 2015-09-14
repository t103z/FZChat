using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FZChat.Client.View
{
    /// <summary>
    /// ConnectionSetting.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectionSetting : Window
    {
        public ConnectionSetting()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
