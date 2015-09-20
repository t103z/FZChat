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
using FZChat.Client.ViewModel;
using FZChat.Client.ViewModel.Service;

namespace FZChat.Client.View
{
    /// <summary>
    /// PopupWindow1.xaml 的交互逻辑
    /// </summary>
    public partial class SearchFriendWindow : Window
    {
        public SearchFriendWindow(ClientDataService service)
        {
            InitializeComponent();
            this.DataContext = new SearchFriendViewModel(service, this.Close);
        }

        private void textBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            textBlock.Background = new SolidColorBrush(Color.FromArgb(255, 255, 32, 32));
        }

        private void textBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            textBlock.Background = new SolidColorBrush(Color.FromArgb(0, 255, 32, 32));
        }

        private void textBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            textBlock.Background = new SolidColorBrush(Color.FromArgb(255, 200, 64, 64));
        }

        private void textBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
