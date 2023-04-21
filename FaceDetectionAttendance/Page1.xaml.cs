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
using Microsoft.Data.SqlClient;
using FaceDetectionAttendance.Model;

namespace FaceDetectionAttendance
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        private Dataconnecttion dataconnecttion = new Dataconnecttion();
        private LoginInfor _infor ;
        public Page1()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string querry = "Select Count(1) from Account where username =@username and passwords=@password";
            try
            {
                if(dataconnecttion.GetConnection().State == System.Data.ConnectionState.Closed)
                    dataconnecttion.GetConnection().Open();

                SqlCommand cmd = new SqlCommand(querry, dataconnecttion.GetConnection());
                cmd.Parameters.AddWithValue("@username", UsernameBox.Text);
                cmd.Parameters.AddWithValue("@password", Password.Password);
                int check = Convert.ToInt32(cmd.ExecuteScalar());
                if (check == 1) {
                    MessageBox.Show("Login Sucess!");
                    string querry2 = "Select Roles from Account where username=@username";
                    try
                    {
                        SqlCommand cmd2 = new SqlCommand(querry2, dataconnecttion.GetConnection());
                        cmd2.Parameters.AddWithValue("@username", UsernameBox.Text);
                        int roles = Convert.ToInt32(cmd2.ExecuteScalar());
                        dataconnecttion.GetConnection().Close();
                        if (roles == 1)
                        {
                            AdninMenu adninMenu = new AdninMenu();

                            this.NavigationService.Navigate(adninMenu );   
                        }
                        else
                        {
                            _infor = new LoginInfor();
                            DataContext = _infor;
                            MenuStaff menu = new MenuStaff(_infor);
                            this.NavigationService.Navigate(menu);
                        }         
                    }catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ForgotPassBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new RecoveryUI());
        }
    }
}
