﻿using System.ComponentModel;
using System.Windows;
using Kaeos.Modules;

namespace Kaeos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            ControlModule.AppConfigModule.SaveToSettings();
        }
    }
}
