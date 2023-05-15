﻿using Emgu.CV.CvEnum;
using FaceDetectionAttendance.MVVM.Model;
using Microsoft.Data.SqlClient;
using ClosedXML;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2013.Excel;
using System.Reflection;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for ReportUI.xaml
    /// </summary>    
    public partial class ReportUI : Page
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
        public ReportUI(string fid)
        {
            InitializeComponent();
            this.fid = fid;
            Get_NameFaculty(fid);
        }
        private void Shift_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedValue = Shift_ComboBox.SelectedValue.ToString();
            if (AttandanceWorkers_DataGrid_1 != null &&
                AbsenteeWorkers_DataGrid_1 != null &&
                AttandanceWorkers_DataGrid_2 != null &&
                AbsenteeWorkers_DataGrid_2 != null)
            {
                if ("Ca 1".Equals(selectedValue))
                {
                    AttandanceWorkers_DataGrid_2.Visibility = Visibility.Collapsed;
                    AbsenteeWorkers_DataGrid_2.Visibility = Visibility.Collapsed;

                    AttandanceWorkers_DataGrid_1.Visibility = Visibility.Visible;
                    AbsenteeWorkers_DataGrid_1.Visibility = Visibility.Visible;
                }
                else
                {
                    AttandanceWorkers_DataGrid_1.Visibility = Visibility.Collapsed;
                    AbsenteeWorkers_DataGrid_1.Visibility = Visibility.Collapsed;

                    AttandanceWorkers_DataGrid_2.Visibility = Visibility.Visible;
                    AbsenteeWorkers_DataGrid_2.Visibility = Visibility.Visible;
                }

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
        public bool Check_DMY(){ //DayMonthYear
            if (Day_TextBox.Text.ToString() == "" || Month_TextBox.Text.ToString() == "" || Year_TextBox.ToString() == "")
            {
                MessageBox.Show("Please enter full information", "Error", MessageBoxButton.OK);
                return false;
            }
            else 
            {
                int day = int.Parse(Day_TextBox.Text.ToString());
                int month = int.Parse(Month_TextBox.Text.ToString());
                int year = int.Parse(Year_TextBox.Text.ToString());

                switch (month)
                {
                    case 1:
                    case 3: 
                    case 5:
                    case 7:
                    case 8:
                    case 10:
                    case 12:
                        if (day <= 0 || day > 31) 
                        {
                            MessageBox.Show("There are only 31 days in "+month+", please re-enter", "Error", MessageBoxButton.OK);
                            return false; 
                        } 
                        else return true;
                        break;
                    case 2:
                        if(year % 4 == 0 && year % 100 != 0)
                        {
                            if (day <= 0 || day > 29) 
                            {
                                MessageBox.Show("There are only 29 days in " + month + "/" + year + ", please re-enter", "Error", MessageBoxButton.OK);
                                return false;
                            } 
                            else return true;
                        }
                        else
                        {
                            if(day <= 0 || day > 28)
                            {
                                MessageBox.Show("There are only 28 days in " + month + "/" + year + ", please re-enter", "Error", MessageBoxButton.OK);
                                return false;
                            }
                            else return true;
                        }
                        break;
                    case 4:
                    case 6:
                    case 9:
                    case 11:
                        if (day <= 0 || day > 30)
                        {
                            MessageBox.Show("There are only 30 days in " + month + ", please re-enter", "Error", MessageBoxButton.OK);
                            return false;
                        }
                        else return true;
                        break;
                    default:
                        MessageBox.Show("There are only 12 months in a year, please re-enter", "Error", MessageBoxButton.OK);
                        return false;
                }
            }
        }
        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Check_DMY())
            {
                int day = int.Parse(Day_TextBox.Text.ToString());
                int month = int.Parse(Month_TextBox.Text.ToString());
                int year = int.Parse(Year_TextBox.Text.ToString());

                AttandanceWorkers_DataGrid_1.Items.Clear();
                AttandanceWorkers_DataGrid_2.Items.Clear();
                AbsenteeWorkers_DataGrid_1.Items.Clear();
                AbsenteeWorkers_DataGrid_2.Items.Clear();

                try
                {
                    for (int i = 1; i <=2; i++) // 2 ca
                    {
                        string query1 = "SELECT id_worker,fullname,d_m " +
                                       "FROM Attendance " +
                                       "INNER JOIN WorkerList " +
                                       "ON Attendance.id_worker = WorkerList.id " +
                                       "WHERE id_faculty = @fid " +
                                       "AND shift_worked = @i " +
                                       "AND DAY(d_m) = @day " +
                                       "AND MONTH(d_m) = @month " +
                                       "AND YEAR(d_m) = @year ";

                        string query2 = "SELECT id, fullname " +
                                        "FROM WorkerList " +
                                        "WHERE id NOT IN (" +
                                            "SELECT id_worker " +
                                            "FROM Attendance " +
                                            "WHERE id_faculty = @fid " +
                                            "AND shift_worked = @i " +
                                            "AND DAY(d_m) = @day " +
                                            "AND MONTH(d_m) = @month " +
                                            "AND YEAR(d_m) = @year " +
                                        ")";
                        if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                        {
                            dtc.GetConnection().Open();
                        }

                        SqlCommand cmd = new SqlCommand(query1, dtc.GetConnection());
                        cmd.Parameters.AddWithValue("@fid", this.fid);
                        cmd.Parameters.AddWithValue("@i", i);
                        cmd.Parameters.AddWithValue("@year", year);
                        cmd.Parameters.AddWithValue("@month", month);
                        cmd.Parameters.AddWithValue("@day", day);
                        SqlDataReader read1 = cmd.ExecuteReader();
                        while (read1.Read())
                        {
                            int id = read1.GetInt32(0);
                            string name = read1.GetString(1);
                            DateTime dt = read1.GetDateTime(2);
                            if (i == 1)
                            {
                                AttandanceWorkers_DataGrid_1.Items.Add(new { id = id, name = name, dt = dt });
                            }
                            else 
                            {
                                AttandanceWorkers_DataGrid_2.Items.Add(new { id = id, name = name, dt = dt });
                            }
                        }
                        read1.Close();

                        SqlCommand cmd2 = new SqlCommand(query2, dtc.GetConnection());
                        cmd2.Parameters.AddWithValue("@fid", this.fid);
                        cmd2.Parameters.AddWithValue("@i", i);
                        cmd2.Parameters.AddWithValue("@year", year);
                        cmd2.Parameters.AddWithValue("@month", month);
                        cmd2.Parameters.AddWithValue("@day", day);
                        SqlDataReader read2 = cmd2.ExecuteReader();
                        while (read2.Read())
                        {
                            int id = read2.GetInt32(0);
                            string name = read2.GetString(1);
                            if (i == 1)
                            {
                                AbsenteeWorkers_DataGrid_1.Items.Add(new { id = id, name = name});
                            }
                            else
                            {
                                AbsenteeWorkers_DataGrid_2.Items.Add(new { id = id, name = name});
                            }
                        }
                        read2.Close();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "error");
                }
            }
        }

        private void ReportMonth_Button_Click(object sender, RoutedEventArgs e)
        {
            var mainWindown = Window.GetWindow(this) as MainWindow;
            if(mainWindown != null)
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

        private void Export_Button_Click(object sender, RoutedEventArgs e)
        {
            if (AttandanceWorkers_DataGrid_1.Items.Count > 0) {
                SaveFileDialog savefile = new SaveFileDialog();
                savefile.DefaultExt = ".xlsx";
                savefile.Filter = "Excel Files|*xlsx;*xls;*xlsm";
                savefile.FileName = "ReportDay.xlsx";
                if (savefile.ShowDialog() == true)
                {
                    try
                    {
                        var workbook = new XLWorkbook();

                        var sheet1 = workbook.Worksheets.Add("Shift 1");
                        int rowWrite = 1;

                        Type t;
                        PropertyInfo[] p;

                        sheet1.Cell(rowWrite, 1).Value = "Attandance workers list";
                        rowWrite++;
                        for (int col = 0; col < AttandanceWorkers_DataGrid_1.Columns.Count; col++)
                        {
                            sheet1.Cell(rowWrite, col + 1).Value = AttandanceWorkers_DataGrid_1.Columns[col].Header.ToString();
                        }
                        rowWrite++;

                        for (int row = 0; row < AttandanceWorkers_DataGrid_1.Items.Count; row++)
                        {
                            t = AttandanceWorkers_DataGrid_1.Items[row].GetType();
                            p = t.GetProperties();
                            for (int col = 0; col < AttandanceWorkers_DataGrid_1.Columns.Count; col++)
                            {
                                sheet1.Cell(rowWrite, col + 1).Value = p[col].GetValue(AttandanceWorkers_DataGrid_1.Items[row]).ToString();
                            }
                            rowWrite++;
                        }
                        rowWrite++;

                        //
                        sheet1.Cell(rowWrite, 1).Value = "Absentee workers list";
                        rowWrite++;
                        for (int col = 0; col < AbsenteeWorkers_DataGrid_1.Columns.Count; col++)
                        {
                            sheet1.Cell(rowWrite, col + 1).Value = AbsenteeWorkers_DataGrid_1.Columns[col].Header.ToString();
                        }
                        rowWrite++;
                        for (int row = 0; row < AbsenteeWorkers_DataGrid_1.Items.Count; row++)
                        {
                            t = AbsenteeWorkers_DataGrid_1.Items[row].GetType();
                            p = t.GetProperties();
                            for (int col = 0; col < AbsenteeWorkers_DataGrid_1.Columns.Count; col++)
                            {
                                sheet1.Cell(rowWrite, col + 1).Value = p[col].GetValue(AbsenteeWorkers_DataGrid_1.Items[row]).ToString();
                            }
                            rowWrite++;
                        }
                        rowWrite++;


                        ////////// sheet2 ///////////////////////////////////////////////////////////
                        rowWrite = 1;
                        var sheet2 = workbook.Worksheets.Add("shift 2");
                        sheet2.Cell(rowWrite, 1).Value = "Attandance workers list";
                        rowWrite++;
                        for (int col = 0; col < AttandanceWorkers_DataGrid_2.Columns.Count; col++)
                        {
                            sheet2.Cell(rowWrite, col + 1).Value = AttandanceWorkers_DataGrid_2.Columns[col].Header.ToString();
                        }
                        rowWrite++;

                        for (int row = 0; row < AttandanceWorkers_DataGrid_2.Items.Count; row++)
                        {
                            t = AttandanceWorkers_DataGrid_2.Items[row].GetType();
                            p = t.GetProperties();
                            for (int col = 0; col < AttandanceWorkers_DataGrid_2.Columns.Count; col++)
                            {
                                sheet2.Cell(rowWrite, col + 1).Value = p[col].GetValue(AttandanceWorkers_DataGrid_2.Items[row]).ToString();
                            }
                            rowWrite++;
                        }
                        rowWrite++;

                        //
                        sheet2.Cell(rowWrite, 1).Value = "Absentee workers list";
                        rowWrite++;
                        for (int col = 0; col < AbsenteeWorkers_DataGrid_2.Columns.Count; col++)
                        {
                            sheet2.Cell(rowWrite, col + 1).Value = AbsenteeWorkers_DataGrid_2.Columns[col].Header.ToString();
                        }
                        rowWrite++;
                        for (int row = 0; row < AbsenteeWorkers_DataGrid_2.Items.Count; row++)
                        {
                            t = AbsenteeWorkers_DataGrid_2.Items[row].GetType();
                            p = t.GetProperties();
                            for (int col = 0; col < AbsenteeWorkers_DataGrid_2.Columns.Count; col++)
                            {
                                sheet2.Cell(rowWrite, col + 1).Value = p[col].GetValue(AbsenteeWorkers_DataGrid_2.Items[row]).ToString();
                            }
                            rowWrite++;
                        }
                        rowWrite++;

                        workbook.SaveAs(savefile.FileName);
                        MessageBox.Show("Done", "Message", MessageBoxButton.OK);
                        workbook.Dispose();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else{
                MessageBox.Show("Data in the table is empty", "Error",MessageBoxButton.OK);
            }
        }

        private void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            SendEmailUI win = new SendEmailUI();
            win.Show();

        }

    }
}
