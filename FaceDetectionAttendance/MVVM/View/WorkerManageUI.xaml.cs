using FaceDetectionAttendance.MVVM.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for WorkerManageUI.xaml
    /// </summary>
    public partial class WorkerManageUI : Page
    {
        Dataconnecttion dtc = new Dataconnecttion();
        SqlCommand cmd = new SqlCommand();
        private string _faculty;
        public WorkerManageUI(string username)
        {
            InitializeComponent();
            getFid(username);
            FacultyText.Text= _faculty;
            setData();
        }
        private void getFid(string username)
        {
            string query = "Select fid from Account where username = @username";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            cmd = new SqlCommand(query, dtc.GetConnection());
            try
            {
                cmd.Parameters.AddWithValue("@username", username);
                this._faculty = cmd.ExecuteScalar().ToString();
                dtc.GetConnection().Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void setData()
        {
            string query = "Select id,fullname, birth, fid from WorkerList Where fid = @fid";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            cmd = new SqlCommand(query, dtc.GetConnection());
            try
            {
                cmd.Parameters.AddWithValue("@fid", _faculty);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader.GetInt32(0));
                    string fullname = reader.GetString(1);
                    DateTime dob = reader.GetDateTime(2);
                    string fid = reader.GetString(3);
                    Workertxt.Items.Add(new { id = id, fullname = fullname, dob = dob, fid = fid });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new AddWorkerUI(_faculty));
        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //edit
            WorkerList worker = new WorkerList();   
            var temp = Workertxt.SelectedItem; 
            if (temp != null)
            {
                dynamic selected = temp;
                worker.Fullname = selected.fullname;
                worker.Birth = selected.dob;
                worker.Fid = selected.fid;
                this.NavigationService.Navigate(new EditWorkerUI(worker));
            }
            else
            {
                MessageBox.Show("Select a worker please");
            }
            
        }
    }
}
