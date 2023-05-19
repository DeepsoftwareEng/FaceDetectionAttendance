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
            reloaddatagrid();
            


        }

        private void Monthcbb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            reloaddatagrid();
            
             
        }
        private void reloaddatagrid() 
        {
            DataContext = this;

            WorkersDataGrid.Columns.Clear();

            //Workers = new ObservableCollection<Worker>();

            List<Worker> source= new List<Worker>();
            // Generate some sample data
            Random random = new Random();
            for (int i = 0; i < 10; i++)
            {
                List<int> shifts = new List<int>();
        
                for (int j = 0; j < 31; j++)
                {
                    int shift = random.Next(1, 4); // Random number between 1 and 3
                    shifts.Add(shift);
                }
                Worker worker = new Worker(i, "Worker" + (i + 1), shifts);
                source.Add(worker);
            }
            WorkersDataGrid.ItemsSource = source;
            // Add DataGrid columns dynamically
            DataGridTextColumn column1 = new DataGridTextColumn();
            column1.Header = "ID";
            column1.Binding = new Binding("ID_Worker");
            WorkersDataGrid.Columns.Add(column1);

            DataGridTextColumn column2 = new DataGridTextColumn();
            column2.Header = "Name";
            column2.Binding = new Binding("WorkerName");
            WorkersDataGrid.Columns.Add(column2);


            for (int i = 1; i <= 31; i++)
            {
                DataGridTextColumn column = new DataGridTextColumn();
                column.Header = i.ToString() + "/" + Monthcbb.SelectedValue;
                column.Binding = new Binding($"Shifts[{i - 1}]");
                WorkersDataGrid.Columns.Add(column);
            }

        }
    }

    public class Worker
    {
        public int ID_Worker { get; set; }
        public string WorkerName { get; set; }
        public List<int> Shifts { get; set; }

        public Worker( )
        {
            Shifts = new List<int>();
        }
        public Worker(int ID, String Name, List<int>shifts) 
        {
            this.ID_Worker = ID;
            this.WorkerName = Name;
            Shifts = shifts;
        }
    }

}


