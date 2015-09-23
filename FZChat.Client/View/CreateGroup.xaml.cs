using FZChat.Client.ViewModel;
using FZChat.Client.ViewModel.Service;
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
    /// CreatGroup.xaml 的交互逻辑
    /// </summary>
    public partial class CreateGroup : Window
    {
        public CreateGroup(ClientDataService service)
        {
            InitializeComponent();
            this.DataContext = new CreateGroupViewModel(service, this.Close);
        }

        private void textBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CloseButton.Background = new SolidColorBrush(Color.FromArgb(120, 255, 0, 0));
        }

        private void textBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void CloseButton_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseButton.Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
        }

        private void CloseButton_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseButton.Background = new SolidColorBrush(Color.FromArgb(0, 255, 0, 0));
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
