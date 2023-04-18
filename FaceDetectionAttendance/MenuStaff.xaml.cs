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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FaceDetectionAttendance
{
    /// <summary>
    /// Interaction logic for MenuStaff.xaml
    /// </summary>
    public partial class MenuStaff : Page
    {
        public MenuStaff()
        {
            InitializeComponent();
        }

        private void WorkerManageBtn_Click(object sender, RoutedEventArgs e)
        {
            Content.NavigationService.Navigate(new WorkerManageUI());
        }

        private void AttendanceBtn_Click(object sender, RoutedEventArgs e)
        {
            Content.NavigationService.Navigate(new AttendanceUI());
        }

        private void ReportBtn_Click(object sender, RoutedEventArgs e)
        {
            Content.NavigationService.Navigate(new ReportUI());
        }
    }
}
