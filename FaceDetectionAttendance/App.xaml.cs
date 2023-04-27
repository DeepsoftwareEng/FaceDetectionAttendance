using FaceDetectionAttendance.MVVM.View;
using FaceDetectionAttendance.MVVM.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FaceDetectionAttendance
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;
        public App()
        { 
        }

        protected override void OnStartup(StartupEventArgs e)
        {
        }
    }
}
