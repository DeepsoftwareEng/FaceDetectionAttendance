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
            IServiceCollection services= new ServiceCollection();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<AdninMenuViewModel>();
            services.AddSingleton<MenuStaffViewModel>();
            services.AddSingleton<RecoveryViewModel>();
            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }
    }
}
