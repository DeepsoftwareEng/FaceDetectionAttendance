using FaceDetectionAttendance.MVVM.Model;
using FaceDetectionAttendance.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
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
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for ReportMonthUI.xaml
    /// </summary>
    
    public partial class ReportMonthUI : Page
    {
        private Dataconnecttion dtc = new Dataconnecttion();
        private string fid;
        private void Get_NameFaculty(string fid)
        {
            string querry = "SELECT name_faculty FROM Faculty WHERE id_faculty = @id_faculty";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
            {
                dtc.GetConnection().Open();
            }
            SqlCommand cmd = new SqlCommand(querry, dtc.GetConnection());
            cmd.Parameters.AddWithValue("@id_faculty", fid);
            NameFaculty_TextBlock.Text = "Faculty name : " + cmd.ExecuteScalar().ToString();
        }
        public ReportMonthUI(string fid)
        {
            InitializeComponent();
            this.fid = fid;
            Get_NameFaculty(fid);
        }

        private void ReportMonth_Button_Click(object sender, RoutedEventArgs e)
        {
            var mainWindown = Window.GetWindow(this) as MainWindow;
            if (mainWindown != null)
            {
                NavigationService.Navigate(new ReportMonthUI(fid));
            }
        }

        private void ReportDay_Button_Click(object sender, RoutedEventArgs e)
        {
            var mainWindown = Window.GetWindow(this) as MainWindow;
            if (mainWindown != null)
            {
                NavigationService.Navigate(new ReportUI(fid));
            }
        }

        private void TextBlock_OnlyNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
                Message_Label.Visibility = Visibility.Visible;
            }
            else Message_Label.Visibility = Visibility.Collapsed;
        }
        public bool Check_DMY()
        { //DayMonthYear
            if (Month_TextBox.Text.ToString() == "" || Year_TextBox.ToString() == "")
            {
                MessageBox.Show("Please enter full information", "Error", MessageBoxButton.OK);
                return false;
            }
            else
            {
                int month = int.Parse(Month_TextBox.Text.ToString());
                int year = int.Parse(Year_TextBox.Text.ToString());

                if (month <= 0 || month >= 13)
                {
                    MessageBox.Show("Just have 12 month a year, please re-enter", "Error", MessageBoxButton.OK);
                    return false;
                }
            }
            return true;
        }

        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Check_DMY())
            {
                int month = int.Parse(Month_TextBox.Text.ToString());
                int year = int.Parse(Year_TextBox.Text.ToString());
                try
                {
                    string querry = "SELECT WorkerList.id, fullname, COUNT(id_worker) As Attend " +
                                    "FROM WorkerList FULL JOIN Attendance " +
                                    "ON Attendance.id_worker = WorkerList.id " +
                                    "AND id_faculty = @fid " +
                                    "AND MONTH(d_m) = @month " +
                                    "AND YEAR(d_m) = @year " +
                                    "GROUP BY WorkerList.id, fullname ";
                    if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                    {
                        dtc.GetConnection().Open();
                    }
                    SqlCommand cmd = new SqlCommand(querry,dtc.GetConnection());
                    cmd.Parameters.AddWithValue("@fid", this.fid);
                    cmd.Parameters.AddWithValue("@month", month);
                    cmd.Parameters.AddWithValue("@year", year);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        string monthYear = month + "/" + year;
                        int attend = reader.GetInt32(2);
                        MonthCount_DataGrid.Items.Add(new {id=id,name=name, monthYear = monthYear,attend = attend});
                    }
                    reader.Close();
                }
                catch(Exception ex) 
                {
                    MessageBox.Show(ex.ToString(), "Error");
                }
            }
        }

        private void Export_Button_Click(object sender, RoutedEventArgs e)
        {
            ExportExcel ex = new ExportExcel();
            ex.ExportExcel_DataGrid(MonthCount_DataGrid);
        }

        private void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            SendEmailUI win = new SendEmailUI();
            win.Show();
        }
    }
}
