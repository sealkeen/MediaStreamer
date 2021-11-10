﻿using System.Windows;
using MediaStreamer.Domain;
using MediaStreamer.WPF.Components;
using MediaStreamer.DataAccess.NetStandard.Models;

namespace MediaStreamer.WPF.NetCore3_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Program.DBAccess = new MediaStreamer.DataAccess.NetStandard.NetCoreDBRepository() 
            {  DB = new MediaStreamer.DMEntitiesContext() };

            InitializeComponent();
            windowFrame.Content = new MainPage();
        }
    }
}
