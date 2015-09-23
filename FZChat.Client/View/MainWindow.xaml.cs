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
using FZChat.Client.ViewModel.Utilities;
using FZChat.Client.ViewModel.Messages;

namespace FZChat.Client.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel(this.Close);
            Messenger.Default.Register<ShutDownMessage>(this, OnShutDown);
        }

        private void OnShutDown(ShutDownMessage obj)
        {
            Environment.Exit(0);
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void buttonSign_MouseEnter(object sender, MouseEventArgs e)
        {
 
        }

        private void buttonSign_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void closebutton_MouseEnter(object sender, MouseEventArgs e)
        {
            closebutton.Background = new SolidColorBrush(Color.FromArgb(255 , 255, 0, 0));
        }

        private void closebutton_MouseLeave(object sender, MouseEventArgs e)
        {
            closebutton.Background = new SolidColorBrush(Color.FromArgb(0, 255, 0, 0));
        }

        private void closebutton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            closebutton.Background = new SolidColorBrush(Color.FromArgb(120, 255, 0, 0));
        }

        private void closebutton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void minbutton_MouseEnter(object sender, MouseEventArgs e)
        {
            minbutton.Background = new SolidColorBrush(Color.FromArgb(120   , 120, 80,80 ));
        }

        private void minbutton_MouseLeave(object sender, MouseEventArgs e)
        {
            minbutton.Background = new SolidColorBrush(Color.FromArgb(0, 255, 0, 0));
        }

        private void minbutton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            minbutton.Background = new SolidColorBrush(Color.FromArgb(255, 120, 80, 80));
        }

        private void minbutton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            new ConnectionSetting().Show();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void pb_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((dynamic)this.DataContext).Password = ((PasswordBox)sender).Password; }
        }
    }
}
