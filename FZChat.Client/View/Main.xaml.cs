﻿using System;
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
using FZChat.Client.View;
using System.Diagnostics;
using FZChat.Client.ViewModel.Service;
using FZChat.Client.ViewModel;
using FZChat.Client.ViewModel.Utilities;
using FZChat.Client.ViewModel.Messages;

namespace FZChat.Client.View
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {
        public Main(ClientDataService service)
        {
            InitializeComponent();
            this.DataContext = new MainViewModel(service, this.Close);
            Messenger.Default.Register<ViewModel.Messages.ShutDownMessage>(this, OnShutDownMessageReceived);
            Grid1.Visibility = Visibility.Hidden;
            Grid2.Visibility = Visibility.Hidden;
            FriendInfromationGrid.Visibility = Visibility.Hidden;
        }

        private void OnShutDownMessageReceived(ShutDownMessage obj)
        {
            Environment.Exit(0);
        }

        private void WindowsClose_MouseEnter(object sender, MouseEventArgs e)
        {
            WindowsClose.Background = new SolidColorBrush(Color.FromArgb(255, 255, 51, 51));
        }

        private void WindowsClose_MouseLeave(object sender, MouseEventArgs e)
        {
            WindowsClose.Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
        }

        private void WindowsClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowsClose.Background = new SolidColorBrush(Color.FromArgb(255, 234, 44, 44));
        }

        private void Path_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Grid1.Visibility = Visibility.Hidden;
            Grid2.Visibility = Visibility.Visible;
            ChatGrid.Visibility = Visibility.Visible;
            FriendInfromationGrid.Visibility = Visibility.Hidden;
            ChatButton.Fill = new SolidColorBrush(Color.FromArgb(0, 101, 187, 29));
            ChatButton.Stroke = new SolidColorBrush(Color.FromArgb(255, 191, 188, 188));
        }

        private void Path_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            Grid2.Visibility = Visibility.Hidden;
            Grid1.Visibility = Visibility.Visible;
            ChatGrid.Visibility = Visibility.Hidden;
            FriendInfromationGrid.Visibility = Visibility.Visible;
            LinkmanButton.Fill = new SolidColorBrush(Color.FromArgb(0, 101, 187, 29));
            LinkmanButton.Stroke = new SolidColorBrush(Color.FromArgb(255, 191, 188, 188));
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            WindowsMin.Background = new SolidColorBrush(Color.FromArgb(255, 85, 209, 0));
        }

        private void WindowsMin_MouseLeave(object sender, MouseEventArgs e)
        {
            WindowsMin.Background = new SolidColorBrush(Color.FromArgb(0, 85, 209, 0));
        }

        private void WindowsMax_MouseEnter(object sender, MouseEventArgs e)
        {
            WindowsMin.Background = new SolidColorBrush(Color.FromArgb(0, 85, 209, 0));
        }

        private void WindowsMin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowsMin.Background = new SolidColorBrush(Color.FromArgb(255, 68, 168, 0));
        }

        private void WindowsMin_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowBasic.WindowState = WindowState.Minimized;

        }


        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void TextWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextWindow.Background = new SolidColorBrush(Color.FromArgb(255, 245, 245, 245));
        }

        private void ChatView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextWindow.Background = new SolidColorBrush(Color.FromArgb(255, 228, 228, 228));
        }

        private void userName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Path_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChatButton.Fill = new SolidColorBrush(Color.FromArgb(255, 101, 187, 29));
            ChatButton.Stroke = new SolidColorBrush(Color.FromArgb(0, 101, 187, 29));
            LinkmanButton.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            LinkmanButton.Stroke = new SolidColorBrush(Color.FromArgb(255, 191, 188, 188));
        }

        private void Path_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            LinkmanButton.Fill = new SolidColorBrush(Color.FromArgb(255, 101, 187, 29));
            LinkmanButton.Stroke = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            ChatButton.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            ChatButton.Stroke = new SolidColorBrush(Color.FromArgb(255, 191, 188, 188));
        }

        private void Path_MouseEnter(object sender, MouseEventArgs e)
        {
            SetButton.Fill = new SolidColorBrush(Color.FromArgb(255, 228, 228, 228));
        }

        private void LinkmanButton_MouseEnter(object sender, MouseEventArgs e)
        {
            LinkmanButton.Stroke = new SolidColorBrush(Color.FromArgb(255, 228, 228, 228));
        }

        private void ChatButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ChatButton.Stroke = new SolidColorBrush(Color.FromArgb(255, 228, 228, 228));
        }

        private void SetButton_MouseLeave(object sender, MouseEventArgs e)
        {
            SetButton.Fill = new SolidColorBrush(Color.FromArgb(255, 191, 188, 188));
        }

        private void LinkmanButton_MouseLeave(object sender, MouseEventArgs e)
        {
            LinkmanButton.Stroke = new SolidColorBrush(Color.FromArgb(255, 191, 188, 188));
        }

        private void ChatButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ChatButton.Stroke = new SolidColorBrush(Color.FromArgb(255, 191, 188, 188));
        }


        private void CloseWindow1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Close();
            TextWindow.Background = new SolidColorBrush(Color.FromArgb(255, 236, 236, 236));
        }

        private void Border_MouseEnter_2(object sender, MouseEventArgs e)
        {
            SendMessage.Background = new SolidColorBrush(Color.FromArgb(255, 85, 209, 0));
        }

        private void SendMessage_MouseLeave(object sender, MouseEventArgs e)
        {
            SendMessage.Background = new SolidColorBrush(Color.FromArgb(0, 85, 209, 0));
        }

        private void SendMessage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SendMessage.Background = new SolidColorBrush(Color.FromArgb(255, 68, 168, 0));
        }

        private void AddFriendPath_MouseEnter(object sender, MouseEventArgs e)
        {
            AddFriendPath.Stroke = new SolidColorBrush(Color.FromArgb(255, 228, 228, 228));
        }

        private void AddFriendPath_MouseLeave(object sender, MouseEventArgs e)
        {
            AddFriendPath.Stroke = new SolidColorBrush(Color.FromArgb(255, 191, 188, 188));
            AddFriendPath.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        private void AddFriendPath_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddFriendPath.Fill = new SolidColorBrush(Color.FromArgb(255, 101, 187, 29));
            AddFriendPath.Stroke = new SolidColorBrush(Color.FromArgb(255, 101, 187, 29));
        }

        private void SetButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            new SetWindow().Show();
        }

        private void SetButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetButton.Fill = new SolidColorBrush(Color.FromArgb(255, 68, 168, 0));
        }

        private void Path_MouseEnter_2(object sender, MouseEventArgs e)
        {
            groupchatbutton.Fill = new SolidColorBrush(Color.FromArgb(255, 233, 233, 233));
        }

        private void groupchatbutton_MouseLeave(object sender, MouseEventArgs e)
        {
            groupchatbutton.Fill = new SolidColorBrush(Color.FromArgb(255, 191, 188, 188));
        }

        private void groupchatbutton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            groupchatbutton.Fill = new SolidColorBrush(Color.FromArgb(255, 68, 168, 0));
        }

        private void groupchatbutton_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            groupchatbutton.Fill = new SolidColorBrush(Color.FromArgb(255, 68, 168, 0));
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Border_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}



