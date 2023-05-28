using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using FaceDetectionAttendance.MVVM.Model;
using System.Linq;
using Microsoft.Data.SqlClient;
using Emgu.CV.XImgproc;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for RequestUI.xaml
    /// </summary>
    public partial class RequestUI : Page
    {
        Dataconnecttion dtc = new Dataconnecttion();
        SqlCommand cmd = new SqlCommand();
        List<Request> requests = new List<Request>();
        public RequestUI()
        {
            InitializeComponent();
            setStatusData();
            setData();
        }
        private void setStatusData()
        {
            Statuscbb.Items.Add("Idle");
            Statuscbb.Items.Add("Accepted");
            Statuscbb.Items.Add("Denied");
        }
        private void setData()
        {
            string query = "select * from Request";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
            {
                dtc.GetConnection().Open();
            }
            try
            {
                cmd = new SqlCommand(query, dtc.GetConnection());
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Request request = new Request();
                    request.id = reader.GetInt32(0);
                    request.id_attendance = reader.GetInt32(1);
                    request.detail = reader.GetString(2);
                    request.states = reader.GetString(3);
                    request.usernamesent = reader.GetString(4);
                    Requestdtg.Items.Add(new {id = request.id ,id_attendance = request.id_attendance, detail = request.detail, status = request.states, usernamesent = request.usernamesent});
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Acceptbtn_Click(object sender, RoutedEventArgs e)
        {
            var temp = Requestdtg.SelectedItem;
            int id = 0;
            if (temp != null)
            {
                dynamic selected = temp;
                id = selected.id;
            }
            string query = "Update Request set states = 'Accept' where id = @id";
            try
            {
                cmd = new SqlCommand(query, dtc.GetConnection());
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                Requestdtg.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            dtc.GetConnection().Close();
        }

        private void Denybtn_Click(object sender, RoutedEventArgs e)
        {
            var temp = Requestdtg.SelectedItem;
            int id = 0;
            if (temp != null)
            {
                dynamic selected = temp;
                id = selected.id;
            }
            string query = "Update Request set states = 'Deny' where id = @id";
            try
            {
                cmd = new SqlCommand(query, dtc.GetConnection());
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            dtc.GetConnection().Close();
        }

        private void Statuscbb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Statuscbb.SelectedItem != null)
            {
                var selectedItem = from request in requests where request.states == Statuscbb.SelectedItem.ToString() select request;
                if (selectedItem != null)
                {
                    Requestdtg.ItemsSource = selectedItem;
                }
            }
        }
    }
}
