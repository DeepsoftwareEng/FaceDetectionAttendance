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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for EditWorkerUI.xaml
    /// </summary>
    public partial class EditWorkerUI : Page
    {
        public EditWorkerUI()
        {
            InitializeComponent();
        }
        private void Backbtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void Facultycbb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
