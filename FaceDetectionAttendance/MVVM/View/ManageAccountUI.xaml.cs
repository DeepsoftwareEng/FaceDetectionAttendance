using System;
using System.Collections.Generic;
using System.Data;
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
using Emgu.CV;
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
        private AccountManagement selectitem = new AccountManagement();
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

                string Username = reader.GetString(0);
                string Passwords = reader.GetString(1);
                string Fid = reader.GetString(2);
                string Gmail = reader.GetString(3);
                string Roles = role;
                Accountdtg.Items.Add(new {username = Username, password = Passwords, fid = Fid, gmail = Gmail, roles = Roles});
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
           AccountManagement selec = Accountdtg.SelectedItem as AccountManagement;
            if (selec != null)
            {
                
                this.NavigationService.Navigate(new EditAccountUI(selectitem));
            }
        }

        private void Delbtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Accountdtg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //// Lấy DataGrid được chọn
            //DataGrid dataGrid = sender as DataGrid;

            //// Lấy chỉ số dòng được chọn
            //int selectedIndex = dataGrid.SelectedIndex;
            if (Accountdtg.SelectedItem != null)
            {
                AccountManagement t = (AccountManagement)Accountdtg.SelectedItem;
                selectitem.username = t.username;
                selectitem.password = t.password;
                selectitem.fid = t.fid;
                selectitem.gmail = t.gmail;
                selectitem.roles = t.roles;
            }

        }
    }
}

