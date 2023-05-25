using FaceDetectionAttendance.MVVM.Model;
using Microsoft.Data.SqlClient;
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

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for LateManageUI.xaml
    /// </summary>
    public partial class LateManageUI : Page
    {
        private Dataconnecttion dtc = new Dataconnecttion();
        SqlCommand cmd = new SqlCommand();
        private string faculty;

        public void setLate_DataGrid()
        {
            try
            {
                string querry = "SELECT LateList.id, LateList.id_worker, " +
                                "fullname, d_m, shift_worked, detail " +
                                "FROM LateList INNER JOIN WorkerList ON " +
                                "LateList.id_worker = WorkerList.id " +
                                "AND id_faculty = '" + this.faculty + "' ";
                if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                {
                    dtc.GetConnection().Open();
                }
                cmd = new SqlCommand(querry, dtc.GetConnection());
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int id_worker = reader.GetInt32(1);
                    string name_worker = reader.GetString(2);
                    DateTime date = reader.GetDateTime(3);
                    int shift = reader.GetInt32(4);
                    string detail = reader.GetString(5);
                    Late_DataGrid.Items.Add(new { Id = id, IdWorker = id_worker, NameWorker = name_worker, DateTime = date.ToString(), Shift = shift, Detail = detail });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }
        public LateManageUI(string faculty)
        {
            InitializeComponent();
            this.faculty = faculty;
            Faculty_Header.Text = faculty;
            Date_DatePicker.SelectedDate = DateTime.Now.Date;
            Month_TextBox.Text = DateTime.Now.Month.ToString();
            Year_TextBox.Text = DateTime.Now.Year.ToString();
            setLate_DataGrid();
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new AddLateUI(faculty));
        }

        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Late_DataGrid.SelectedItems.Count == 1)
            {
                dynamic temp = Late_DataGrid.SelectedItem;
                if (temp != null)
                {
                    dynamic Idselected = temp.Id;
                    this.NavigationService.Navigate(new EditLateUI(this.faculty, Idselected));
                }
            }
            else
            {
                MessageBox.Show("Please choose one to edit", "Message");
            }
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Late_DataGrid.SelectedItems.Count == 1)
            {
                MessageBoxResult result = MessageBox.Show("Delete this data from database ?", "Warning", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    dynamic temp = Late_DataGrid.SelectedItem;
                    if (temp != null)
                    {
                        dynamic Idselected = temp.Id;
                        string querry = "DELETE FROM LateList WHERE id = '" + Idselected + "'";
                        if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                        {
                            dtc.GetConnection().Open();
                        }
                        cmd = new SqlCommand(querry, dtc.GetConnection());
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Successful", "Message");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please choose one to edit", "Message");
            }
        }

        private void Date_DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                /*DateTime dateSearch = Convert.ToDateTime(Date_DatePicker.SelectedDate.Value);
                string querry = "SELECT LateList.id, LateList.id_worker, " +
                                "fullname, d_m, shift_worked, detail " +
                                "FROM LateList INNER JOIN WorkerList ON " +
                                "LateList.id_worker = WorkerList.id " +
                                "AND id_faculty = '" + this.faculty + "' " +
                                "AND ";
                if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                {
                    dtc.GetConnection().Open();
                }
                cmd = new SqlCommand(querry, dtc.GetConnection());
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int id_worker = reader.GetInt32(1);
                    string name_worker = reader.GetString(2);
                    DateTime date = reader.GetDateTime(3);
                    int shift = reader.GetInt32(4);
                    string detail = reader.GetString(5);
                    Late_DataGrid.Items.Add(new { Id = id, IdWorker = id_worker, NameWorker = name_worker, DateTime = date.ToString(), Shift = shift, Detail = detail });
                }*/
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }
    }
}
