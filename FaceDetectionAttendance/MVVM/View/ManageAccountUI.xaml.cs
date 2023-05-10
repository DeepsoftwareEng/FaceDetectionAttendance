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
using FaceDetectionAttendance.MVVM.Model;
using Microsoft.Data.SqlClient;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for ManageAccountUI.xaml
    /// </summary>
    public partial class ManageAccountUI : Page
    {
        private Dataconnecttion dtc = new Dataconnecttion();
        SqlCommand command;

        //Lay du lieu tu sql
        private void Loaddata()
        {
            string querry = "Select username,passwords,fid, gmail,roles From Account";
            if(dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            command = new SqlCommand(querry, dtc.GetConnection());
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int temp = reader.GetInt32(4);
                string role;
                if (temp == 1)
                {
                    role = "Admin";
                }
                else
                {
                    role = "Staff";
                }  
                AccountManagement t = new AccountManagement();
                t.username = reader.GetString(0);
                t.password = reader.GetString(1);
                t.fid = reader.GetString(2);
                t.gmail = reader.GetString(3);
                t.roles = role;
                Accountdtg.Items.Add(t);
            }
        }
        public ManageAccountUI()
        {
            InitializeComponent();
            Loaddata();
        }

        private void Searchbtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Addbtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new AddAccountUI());
        }

        private void Editbtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new EditAccountUI());
        }

        private void Delbtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
