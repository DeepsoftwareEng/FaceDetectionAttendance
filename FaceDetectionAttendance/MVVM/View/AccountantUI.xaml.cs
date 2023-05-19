using FaceDetectionAttendance.MVVM.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Data.SqlClient;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for AccountantUI.xaml
    public partial class AccountantUI : Page
    {
        private ObservableCollection<Worker> Workers { get; set; }
        private Dataconnecttion dtc = new Dataconnecttion();
        public SqlCommand SQLcmd;
        public void Add_SetComboBoxData()
        {

            string querry = "Select* from Faculty";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            SQLcmd = new SqlCommand(querry, dtc.GetConnection());
            SqlDataReader reader = SQLcmd.ExecuteReader();
            while (reader.Read())
            {
                Faculty a = new Faculty();
                a.IdFaculty = reader.GetString(0);
                a.NameFaculty = reader.GetString(1);
                facultycbb.Items.Add(a.IdFaculty);
            }
            for (int i = 1; i <= 12; i++)
            {
                Monthcbb.Items.Add( i + "");

            }
        }
        // test combobox succefull
        public AccountantUI()
        {

            InitializeComponent();
            Add_SetComboBoxData();
            DataContext = this;

            Workers = new ObservableCollection<Worker>();

            // Generate some sample data
            Random random = new Random();
            for (int i = 0; i < 10; i++)
            {
                Worker worker = new Worker { WorkerName = "Worker " + (i + 1) };
                for (int j = 0; j < 31; j++)
                {
                    int shift = random.Next(1, 2); // Random number between 1 and 3
                    worker.Shifts.Add(shift);
                }
                Workers.Add(worker);
            }

            // Add DataGrid columns dynamically
            for (int i = 1; i <= 31; i++)
            {
                DataGridTextColumn column = new DataGridTextColumn();
                column.Header = i.ToString();
                column.Binding = new Binding($"Shifts[{i - 1}]");
                WorkersDataGrid.Columns.Add(column);
            }


        }

    }

    public class Worker
    {
        public string WorkerName { get; set; }
        public List<int> Shifts { get; set; }

        public Worker()
        {
            Shifts = new List<int>();
        }
    }

}


