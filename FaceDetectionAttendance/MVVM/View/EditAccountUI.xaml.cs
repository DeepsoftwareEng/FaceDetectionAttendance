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

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for EditAccountUI.xaml
    /// </summary>
    public partial class EditAccountUI : Page
    {
        private Dataconnecttion dtc = new Dataconnecttion();
        private SqlCommand command;
        private DataGridRow selectedRow;
        private AccountManagement temp = new AccountManagement();
        public EditAccountUI()
        {
            InitializeComponent();
            setComboBoxData();
        }

        public EditAccountUI(AccountManagement selec)
        {
            InitializeComponent();
            this.selectedRow = selectedRow;
            temp = selec;
        }
        private void Currentinfor(AccountManagement selec)
        {
            Usernametxb.Text = selec.username;
            Passwordtxb.Text = selec.password;
            facultycbb.SelectedItem = selec.fid;
            Gmailtxb.Text = selec.gmail;
            Rolecbb.SelectedItem = selec.roles;
        }

        private void setComboBoxData()
        {
            string querry = "Select* from Faculty";//cau lenh sql
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();//ket noi den database
            command = new SqlCommand(querry, dtc.GetConnection());
            SqlDataReader reader = command.ExecuteReader();//doc du lieu tu database
            while (reader.Read())
            {
                Faculty a = new Faculty();
                a.IdFaculty = reader.GetString(0);
                a.NameFaculty = reader.GetString(1);
                facultycbb.Items.Add(a.IdFaculty);
            }
            Rolecbb.Items.Add("Admin");
            Rolecbb.Items.Add("Staff");
            //set tiep Faculty- sua trong while va querry
            //lay du lieu trong sql ra
            //tao ra 1 bien kieu Faculty(trong while)

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        internal void ShowDialog()
        {
            throw new NotImplementedException();
        }
    }
}
