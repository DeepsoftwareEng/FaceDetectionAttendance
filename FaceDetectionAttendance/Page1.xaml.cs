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
using Microsoft.Data.SqlClient;

namespace FaceDetectionAttendance
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        private SqlConnection dc = new SqlConnection("Data Source = (localdb)\\MSSqlLocalDB; Initial catalog = CCPTPM; Integrated Security=true; TrustServerCertificate=True");
        public Page1()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string querry = "Select Count(1) from Account where username =@username and passwords=@password";
            try
            {
                if(dc.State == System.Data.ConnectionState.Closed)
                    dc.Open();

                SqlCommand cmd = new SqlCommand(querry,dc);
                cmd.Parameters.AddWithValue("@username", UsernameBox.Text);
                cmd.Parameters.AddWithValue("@password", Password.Password);
                int check = Convert.ToInt32(cmd.ExecuteScalar());
                if (check == 1) {
                    MessageBox.Show("Login Sucess!");
                    string querry2 = "Select Roles from Account where username=@username";
                    try
                    {
                        SqlCommand cmd2 = new SqlCommand(querry2, dc);
                        cmd2.Parameters.AddWithValue("@username", UsernameBox.Text);
                        int roles = Convert.ToInt32(cmd2.ExecuteScalar());
                        dc.Close();
                        if (roles == 1)
                            this.NavigationService.Navigate(new AdninMenu());
                        else
                            this.NavigationService.Navigate(new MenuStaff());
                    }catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    this.NavigationService.Navigate(new MenuStaff());
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
