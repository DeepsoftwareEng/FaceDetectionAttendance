using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using FaceDetectionAttendance.MVVM.Model;
using Microsoft.Data.SqlClient;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for AddWorkerUI.xaml
    /// </summary>
    public partial class AddWorkerUI : Page
    {
        private Dataconnecttion dtc = new Dataconnecttion();
        private SqlCommand SQLcommand = new SqlCommand();
        public AddWorkerUI()
        {
            InitializeComponent();
            Add_SetComboBoxData();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FullNametxt.Text) ||
               Dobtxt.Text.Length == 0 ||
               Falcutybox.SelectedItem == null
               )
            {
                MessageBox.Show("Hãy nhập đủ thông tin.");
                return;
            }
            string fullname = FullNametxt.Text;
            string DoB = Dobtxt.Text;
            string faculty =    Falcutybox.SelectedItem.ToString();
            // string role = Rolecbb.SelectedItem.ToString();
            // BitmapImage image = ((ImageBrush)Imagebd.Background).ImageSource as BitmapImage;
            FullNametxt.Text = "";
            Dobtxt.Text = "";
            Falcutybox.SelectedItem = null;
            //Rolecbb.SelectedItem = null;
            // Imagebd.Background = null;

            /* add du kieu*/
            string binFolderPath = System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            string projectFolderPath = Directory.GetParent(binFolderPath).FullName;
            string fix = projectFolderPath.Remove(projectFolderPath.Length - 9);
            string resourceFolderPath = System.IO.Path.Combine(fix, "Resource");
            string avt = resourceFolderPath + @$"\Avatar\{FullNametxt}.png";
            //     File.Copy("C:\\FDA temp\\temp.png", avt);
            string querry = "Insert into WorkerList Values(@fullname, @birth,@images, @fid)";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            try
            {
                SQLcommand = new SqlCommand(querry, dtc.GetConnection());
                SQLcommand.Parameters.Add("@fullname", SqlDbType.NVarChar).Value = FullNametxt.Text;
                SQLcommand.Parameters.Add("@birth", SqlDbType.Date).Value = Dobtxt.Text;
                SQLcommand.Parameters.Add("@fid", SqlDbType.NVarChar).Value = Falcutybox.SelectedItem.ToString();
                SQLcommand.Parameters.Add("@images", SqlDbType.NVarChar).Value = FullNametxt.Text;
                SQLcommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Backbtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
        private void Add_SetComboBoxData()
        {

            string querry = "Select* from Faculty";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            SQLcommand = new SqlCommand(querry, dtc.GetConnection());
            SqlDataReader reader = SQLcommand.ExecuteReader();
            while (reader.Read())
            {
                Faculty a = new Faculty();
                a.IdFaculty = reader.GetString(0);
                a.NameFaculty = reader.GetString(1);
                Falcutybox.Items.Add(a.IdFaculty);
            }
           
        }
    } }
