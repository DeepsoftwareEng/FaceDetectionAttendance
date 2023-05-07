using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using Emgu.CV.Structure;
using Emgu.CV;
using Emgu.CV.Util;
using FaceDetectionAttendance.MVVM.Model;
using Microsoft.Data.SqlClient;
using System.Windows.Media;
using Emgu.CV.CvEnum;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;


namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for AttendanceUI.xaml
    /// </summary>
    public partial class AttendanceUI : Page
    {
        private VideoCapture _videoSource;
        private Task _processingTask;
        private CascadeClassifier _faceClassifier;
        private Dataconnecttion Dataconnecttion = new Dataconnecttion();
        private SqlCommand command;
        private readonly SolidColorBrush _highlightBrush = new SolidColorBrush(Colors.LimeGreen);
        private List<Image<Gray, byte>> WorkerList = new List<Image<Gray, byte>>();// Worker's Image
        private List<AttendanceWorker> AttendList = new List<AttendanceWorker>(); //Attend datagrid
        private List<WorkerLabel> workerLabels= new List<WorkerLabel>();// Worker's infor
        private List<int> IdListIn = new List<int>();
        private string _username;
        private string _faculty;
        private string querry;

        public AttendanceUI(string username)
        {
            InitializeComponent();
            _faceClassifier= new CascadeClassifier("haarcascade_frontalface_default.xml");
            _username = username;
            setData();
        }
        private void setData()
        {
            querry = "Select fid where username = @username";
            if(Dataconnecttion.GetConnection().State == System.Data.ConnectionState.Closed)
                Dataconnecttion.GetConnection().Open();
            try
            {
                command = new SqlCommand(querry, Dataconnecttion.GetConnection());
                command.Parameters.AddWithValue("@username", _username);
                _faculty = Convert.ToString(command.ExecuteScalar());
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            querry = "Select images where fid = @fid";
            try
            {
                command = new SqlCommand(querry, Dataconnecttion.GetConnection());
                command.Parameters.AddWithValue("@fid", _faculty);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string imageName = reader.ToString() + ".png";
                        Image<Gray, byte> temp = new Image<Gray, byte>("/Resource/WorkerImage/"+_faculty+"/"+imageName);
                        WorkerList.Add(temp);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            querry = "Select fullname, id where fid = @fid";
            try
            {
                command = new SqlCommand(querry, Dataconnecttion.GetConnection());
                command.Parameters.AddWithValue("@fid", _faculty);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        WorkerLabel wl = new WorkerLabel();
                        wl.Name = reader.GetString(0);
                        wl.Id= reader.GetInt32(1);
                        workerLabels.Add(wl);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);    
            }
        }
        private void StartCam_Click(object sender, EventArgs e)
        {
            _videoSource = new VideoCapture();
            //_processingTask = Task.Run(() => ProcessFramesAsync());
        }
        private void StopCam_Click(object sender, EventArgs e)
        {
            _videoSource.Stop();
        }
       
    }
}
