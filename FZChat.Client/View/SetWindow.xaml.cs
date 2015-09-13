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
using System.Windows.Navigation;
using System.Diagnostics;

namespace FZChat.Client.View
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class SetWindow : Window
    {
        public SetWindow()
        {
            InitializeComponent();
        }

        private void label_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void label_MouseEnter(object sender, MouseEventArgs e)
        {
           
        }

        private void SetWindowClose_MouseEnter(object sender, MouseEventArgs e)
        {
            SetWindowClose.Background = new SolidColorBrush(Color.FromArgb(255, 255, 110, 0));
        }

        private void SetWindowClose_MouseLeave(object sender, MouseEventArgs e)
        {
            SetWindowClose.Background = new SolidColorBrush(Color.FromArgb(0, 255, 110, 0));
        }

        private void SetWindowClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetWindowClose.Background = new SolidColorBrush(Color.FromArgb(255, 255, 75, 0));
        }

        private void SetWindowClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void button_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
            //Main().Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
