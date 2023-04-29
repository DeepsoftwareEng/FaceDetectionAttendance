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
using FaceDetectionAttendance.MVVM.View;

namespace FaceDetectionAttendance.MVVM.Components
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBoxChange : Window
    {
        MainWindow mainWindow = new MainWindow();
        private int stt;
        private string username;
        public MessageBoxChange(int s, string username)
        {
            InitializeComponent();
            stt=s;
            this.username = username;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (stt == 1)
            {
                MainWindow window1 = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                window1.Start.NavigationService.Navigate(new AdninMenu(username));
                this.Close();
            }
            else
            {
                MainWindow window1 = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                window1.Start.NavigationService.Navigate(new MenuStaff(username));
                this.Close();
            }    
        }
    }
}
