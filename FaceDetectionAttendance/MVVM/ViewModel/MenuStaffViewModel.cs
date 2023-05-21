﻿///////////////////////////MenuStaff.cs
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
using Emgu.CV.Shape;
using FaceDetectionAttendance.MVVM.Model;
using Microsoft.Data.SqlClient;
namespace FaceDetectionAttendance.MVVM.ViewModel
{
    /// <summary>
    /// Interaction logic for MenuStaff.xaml
    /// </summary>
    public partial class MenuStaff : Page
    {
        private Dataconnecttion Dataconnecttion = new Dataconnecttion();
        private string _username;
        private string faculty;
        public MenuStaff(string username)
        {
            InitializeComponent();
            setInfor(username);
            setFaculty(username);
            _username = username;
        }
        private void WorkerManageBtn_Click(object sender, RoutedEventArgs e)
        {
            Content.NavigationService.Navigate(new WorkerManageUI());
        }

        private void AttendanceBtn_Click(object sender, RoutedEventArgs e)
        {
            Content.NavigationService.Navigate(new AttendanceUI(_username));
        }

        private void ReportBtn_Click(object sender, RoutedEventArgs e)
        {
            Content.NavigationService.Navigate(new ReportUI());
        }

        private void AccountantBtn_Click(object sender, RoutedEventArgs e)
        {
            Content.NavigationService.Navigate(new AccountantUI(this.faculty));
        }
        void setInfor(string username)
        {
            string querry = "Select images from Account where username = @username";
            if (Dataconnecttion.GetConnection().State == System.Data.ConnectionState.Closed)
                Dataconnecttion.GetConnection().Open();
            SqlCommand cmd = new SqlCommand(querry, Dataconnecttion.GetConnection());
            cmd.Parameters.AddWithValue("@username", username);
            BitmapImage source = new BitmapImage();
            source.BeginInit();
            source.UriSource = new Uri(@"/Resource/Avatar/" + Convert.ToString(cmd.ExecuteScalar()) + ".png", UriKind.RelativeOrAbsolute);
            source.EndInit();
            avt.Source = source;
            StaffName.Text = username;
        }
        void setFaculty(string username)
        {
            try
            {
                string querry = "SELECT fid FROM Account WHERE username = '" + username + "' ";
                if (Dataconnecttion.GetConnection().State == System.Data.ConnectionState.Closed)
                {
                    Dataconnecttion.GetConnection().Open();
                }
                SqlCommand cmd = new SqlCommand(querry, Dataconnecttion.GetConnection());
                this.faculty = Convert.ToString(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
