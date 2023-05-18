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
using System.Drawing;
using Emgu.CV.Face;
using CascadeClassifier = Emgu.CV.CascadeClassifier;
using System.Windows.Interop;
using System.Linq;
using System.IO;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for AttendanceUI.xaml
    /// </summary>
    public partial class AttendanceUI : Page
    {
        private Emgu.CV.VideoCapture _videoSource = new VideoCapture();
        private CascadeClassifier _faceClassifier;
        private readonly EigenFaceRecognizer _recognizer = new EigenFaceRecognizer();
        private Dataconnecttion Dataconnecttion = new Dataconnecttion();
        private SqlCommand command;
        private List<Image<Gray, byte>> WorkerList = new List<Image<Gray, byte>>();// Worker's Image
        private List<AttendanceWorker> AttendList = new List<AttendanceWorker>(); //Attend datagrid
        private List<WorkerLabel> workerLabels = new List<WorkerLabel>();// Worker's infor
        private List<int> IdListIn = new List<int>();
        private bool _isCapturing;
        private string _username;
        private string _faculty;
        private string querry;
        private int shift=1;
        public AttendanceUI(string username)
        {
            InitializeComponent();
            _faceClassifier = new CascadeClassifier("haarcascade_frontalface_default.xml");
            _username = username;
            setData();
            FacultyText.Text = _faculty;
            ShiftText.Text = "Shift: " + shift;
        }
        private void setData()
        {
            string binFolderPath = System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            string projectFolderPath = Directory.GetParent(binFolderPath).FullName;
            string fix = projectFolderPath.Remove(projectFolderPath.Length - 9); 
            List<int> WorkerID = new List<int>();
            querry = "Select fid From Account where username = @username";
            if (Dataconnecttion.GetConnection().State == System.Data.ConnectionState.Closed)
                Dataconnecttion.GetConnection().Open();
            try
            {
                command = new SqlCommand(querry, Dataconnecttion.GetConnection());
                command.Parameters.AddWithValue("@username", _username);
                _faculty = Convert.ToString(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            querry = "Select images From WorkerList where fid = @fid";
            try
            {
                command = new SqlCommand(querry, Dataconnecttion.GetConnection());
                command.Parameters.AddWithValue("@fid", _faculty);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string imageName = reader.GetString(0) + ".png";
                        string resourceFolderPath = Path.Combine(fix, "Resource");
                        Image<Gray, byte> temp = new Image<Gray, byte>(@$"{resourceFolderPath}\WorkerImage\{_faculty}\{imageName}");
                        WorkerList.Add(temp);
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            querry = "Select fullname, id from WorkerList where fid = @fid";
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
                        wl.Id = reader.GetInt32(1);
                        workerLabels.Add(wl);
                        WorkerID.Add(wl.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            List<Mat> workerImage = new List<Mat>();
            foreach(var image in WorkerList)
            {
                Mat mat = image.Mat;
                workerImage.Add(mat);
            }
            try
            {
                _recognizer.Train(new VectorOfMat(workerImage.ToArray()), new VectorOfInt(WorkerID.ToArray()));
            }catch(Exception ex)
            {
                MessageBox.Show("Can not retrieve worker data! Please add worker data!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
        }

        private void StartCam_Click(object sender, EventArgs e)
        {
            if (_isCapturing)
            {
                return;
            }

            _videoSource.Start();
            _isCapturing = true;

            // Start capturing video frames
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(ProcessFrame);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 30);
            timer.Start();
        }
        private void StopCam_Click(object sender, EventArgs e)
        {
            if (!_isCapturing)
            {
                return;
            }
            _videoSource.Stop();
            _isCapturing = false;
            var absentWorkers = from workerLabel in workerLabels
                                join workerAttend in AttendList on workerLabel.Id equals workerAttend.id into workerGroup
                                from worker in workerGroup.DefaultIfEmpty()
                                where worker == null 
                                select workerLabel;
            Absentee.ItemsSource= absentWorkers;
        }
        private void ProcessFrame(object sender, EventArgs e)
        {
            var frame = _videoSource.QueryFrame().ToImage<Bgr, byte>().Resize(640, 480, Inter.Cubic);
            var grayFrame = frame.Convert<Gray, byte>();

            // Detect faces in current frame
            var faces = _faceClassifier.DetectMultiScale(grayFrame, 1.2, 5);

            foreach (var face in faces)
            {
                // Extract face region of interest
                var faceRect = new Rectangle(face.X, face.Y, face.Width, face.Height);
                var faceImage = grayFrame.Copy(faceRect).Resize(200, 200, Inter.Cubic);

                // Recognize face
                var result = _recognizer.Predict(faceImage);

                // Display result
                var label = result.Label.ToString();

                if (checkId(Int32.Parse(label)))
                {
                    //Display worker name if recognized
                    var worker = workerLabels.FirstOrDefault(w => w.Id == Int32.Parse(label));// take worker id
                    if (worker != null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            //KHi nhan dien ra cong nhan
                            //string querry = "insert into Attendane values (@id, @date, @fid, @shift)";
                            //if (Dataconnecttion.GetConnection().State == System.Data.ConnectionState.Closed)
                            //    Dataconnecttion.GetConnection().Open();
                            //command = new SqlCommand(querry, Dataconnecttion.GetConnection());
                            //command.Parameters.AddWithValue("@id", Int32.Parse(label));
                            //command.Parameters.AddWithValue("@date", DateTime.Now);
                            //command.Parameters.AddWithValue("@fid", _faculty);
                            //command.Parameters.AddWithValue("@shift", shift);
                            //command.ExecuteNonQuery();
                            //Dataconnecttion.GetConnection().Close();
                            AttendanceWorker temp = new AttendanceWorker();
                            temp.name = worker.Name;
                            temp.id = Int32.Parse(label);
                            temp.date = DateTime.Now;
                            Attendance.Items.Add(new { id = worker.Id,Name = worker.Name, TimeIn = DateTime.Now });
                            workerLabels.Remove(worker);
                            AttendanceTxt.Text = Attendance.Items.Count.ToString();
                        });
                    }
                }
                // Draw a rectangle around the face
                frame.Draw(faceRect, new Bgr(0, 0, 255), 2);
            }

            // Display the processed frame in the image control
            var bitmap = frame.ToBitmap();
            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            VideoDisplay.Source = bitmapSource;
        }
        private bool checkId(int id)
        {
            for(int i =0; i < workerLabels.Count; i++)
            {
                if (workerLabels[i].Id == id)
                    return true;
            }
            return false;
        }
        private void EndBtn_Click(object sender, RoutedEventArgs e)
        {
            //Attendance.ItemsSource= AttendList;
            //foreach (var worker in AttendList)
            //    MessageBox.Show(worker.name);
        }

        private void Switchbtn_Click(object sender, RoutedEventArgs e)
        {
            shift = 2;
            ShiftText.Text = "Shift: " + shift;
        }
    }
}
