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
using FaceDetectionAttendance.MVVM.Model;
using Unity.Policy;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.Win32;
using System.IO;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for AddAccountUI.xaml
    /// </summary>
    public partial class AddAccountUI : Page
    {
        private Dataconnecttion dtc = new Dataconnecttion();
        private SqlCommand SQLcommand;
        private FileInfo imgSource;
        public AddAccountUI()
        {
            InitializeComponent();
            Add_SetComboBoxData();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Addimagebtn_Click(object sender, RoutedEventArgs e)
        {

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
                facultycbb.Items.Add(a.IdFaculty);
            }
            Rolecbb.Items.Add("Admin");
            Rolecbb.Items.Add("Staff");
        }



        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void Addimagebtn_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                Imagebd.Background = new ImageBrush(new BitmapImage(new Uri(openFileDialog.FileName)));
                imgSource = new FileInfo(openFileDialog.FileName);
            }
            
        }

        private void Savebtn_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra xem tất cả các trường thông tin đã được nhập đầy đủ và hợp lệ
            if (string.IsNullOrEmpty(Usernametxb.Text) ||
                Passwordtxb.Text.Length == 0 ||
                Confirmtxb.Text.Length == 0 ||
                Gmailtxb.Text == "" ||
                facultycbb.SelectedItem == null ||
                Rolecbb.SelectedItem == null ||
                Imagebd.Background == null)
            {
                MessageBox.Show("Hãy nhập đủ thông tin.");
                return;
            }

            // Kiểm tra xem mật khẩu và xác nhận mật khẩu có giống nhau không
            if (Passwordtxb.Text != Confirmtxb.Text)
            {
                MessageBox.Show("Hãy nhập lại mật khẩu.");
                return;
            }

            // Lấy thông tin người dùng từ các ô nhập và chọn
            string username = Usernametxb.Text;
            string password = new System.Net.NetworkCredential(string.Empty, Passwordtxb.Text).Password;
            string gmail = Gmailtxb.Text;
            string faculty = facultycbb.SelectedItem.ToString();
            string role = Rolecbb.SelectedItem.ToString();
            BitmapImage image = ((ImageBrush)Imagebd.Background).ImageSource as BitmapImage;

            // Xóa các ô nhập để chuẩn bị cho việc thêm người dùng tiếp theo
            Usernametxb.Text = "";
            Passwordtxb.Clear();
            Confirmtxb.Clear();
            Gmailtxb.Text = "";
            facultycbb.SelectedItem = null;
            Rolecbb.SelectedItem = null;
            Imagebd.Background = null;

            //luu du lieu
            string avt = $"{username}.png";
            imgSource.MoveTo(imgSource.Directory.FullName + "//"+avt);
            File.Copy(imgSource.ToString(), @"//Resource//Avatar");
            string querry = "Insert @username,@passwords,@fid, @gmail,@roles,@image";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            SQLcommand = new SqlCommand(querry, dtc.GetConnection());
            SQLcommand.Parameters.AddWithValue("@username", username);
            SQLcommand.Parameters.AddWithValue("@password", password);
            SQLcommand.Parameters.AddWithValue("@fid", faculty);
            SQLcommand.Parameters.AddWithValue("@gmail", gmail);
            SQLcommand.Parameters.AddWithValue("@roles", role);

            SQLcommand.ExecuteNonQuery();
        }
    }
}
