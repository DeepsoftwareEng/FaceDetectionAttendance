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
    /// Interaction logic for RecoveryUI.xaml
    /// </summary>
    public partial class RecoveryUI : Page
    {
        private SqlConnection dc = new SqlConnection("Data Source = (localdb)\\MSSqlLocalDB; Initial catalog = CCPTPM; Integrated Security=true; TrustServerCertificate=True");
        public RecoveryUI()
        {
            InitializeComponent();
        }

        private void RecovBtn_Click(object sender, RoutedEventArgs e)
        {
            string querry = "Select count(1) from Account where name_faculty = @faculty and email=@email and username=@username";
            try
            {
                if(dc.State == System.Data.ConnectionState.Closed)
                    dc.Open();
                SqlCommand cmd = new SqlCommand(querry, dc);
                cmd.Parameters.AddWithValue("@username", UsernameBox.Text);
                cmd.Parameters.AddWithValue("@faculty",FalcultyBox.Text);
                cmd.Parameters.AddWithValue("@email",EmailBox.Text);
                int check = Convert.ToInt32(cmd.ExecuteScalar());
                if (check == 1)
                {

                }
                else
                    MessageBox.Show("Incorrect information, please fill in again");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
