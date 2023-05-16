using FaceDetectionAttendance.MVVM.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection.PortableExecutable;
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
    /// Interaction logic for AddAccountUI.xaml
    /// </summary>
    public partial class AddAccountUI : Page
    {
        Dataconnecttion dtc = new Dataconnecttion();
        private void ComboBox_Reload()
        {
            string querry = "Select name_faculty From Faculty";
            if(dtc.GetConnection().State == System.Data.ConnectionState.Closed)
            {
                dtc.GetConnection().Open();
            }
            SqlCommand cmd = new SqlCommand(querry, dtc.GetConnection());
            SqlDataReader dataReader = cmd.ExecuteReader();
            while(dataReader.Read())
            {
                string item = dataReader.GetString(0);
                Faculty_ComboBox.Items.Add(item);
            }
            Faculty_ComboBox.SelectedIndex = 0;
        }

        public AddAccountUI()
        {
            InitializeComponent();
            ComboBox_Reload();
        }
    }
}
