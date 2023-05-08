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
            _processingTask = Task.Run(() => ProcessFramesAsync());
        }
        private void StopCam_Click(object sender, EventArgs e)
        {
            _videoSource.Stop();
        }
       private async Task ProcessFramesAsync()
       {
           while(_videoSource.IsOpened)
            {
                var frame = _videoSource.QueryFrame().ToImage<Bgr, byte>();
                if(frame == null)
                {
                    break;
                }
                var faces = DetectFaces(frame);
                foreach( var face in faces)
                {
                    var roi = frame.Copy(face);
                    var grayRoi = roi.Convert<Gray, byte>().Resize(100, 100, Inter.Cubic);
                    var result = RecognizeWorker(grayRoi);
                    if (result.Label != -1 && result.Distance < 3000)
                    {
                        await Dispatcher.InvokeAsync(() =>
                        {
                            // display the recognized worker name on the UI thread
                           
                        });
                    }

                    var rect = new Rectangle(face.X, face.Y, face.Width, face.Height);
                    var highlightRect = new Rectangle((int)(rect.X * VideoDisplay.ActualWidth / frame.Width),
                                                       (int)(rect.Y * VideoDisplay.ActualHeight / frame.Height),
                                                       (int)(rect.Width * VideoDisplay.ActualWidth / frame.Width),
                                                       (int)(rect.Height * VideoDisplay.ActualHeight / frame.Height));

                // display the frame on the UI thread
                var imageSource = BitmapSourceConvert.ToBitmapSource(frame);
                await Dispatcher.InvokeAsync(() =>
                {
                    //CameraGrid.Background = new ImageBrush(imageSource);
                });
            }
            }
       }
        private Rectangle[] DetectFaces(Image<Bgr, byte> image)
        {
            using var gray = image.Convert<Gray, byte>();
            var faces = _faceClassifier.DetectMultiScale(gray,1.1,3,new System.Drawing.Size(500,500));
            return faces;
        }

        private (int Label, double Distance) RecognizeWorker(Image<Gray, byte> image)
        {
            var recognizer = new EigenFaceRecognizer(WorkerList.Count, double.PositiveInfinity);
            var labels = new List<int>();
            for (var i = 0; i < WorkerList.Count; i++)
            {
                labels.Add(i);
            }
            recognizer.Train(WorkerList.ToArray(), labels.ToArray());

            var result = recognizer.Predict(image);
            return (result.Label, result.Distance);
        }
    }
    public static class BitmapSourceConvert
        {
            [System.Runtime.InteropServices.DllImport("gdi32.dll")]
            private static extern bool DeleteObject(System.IntPtr hObject);

            public static BitmapSource ToBitmapSource<T>(this Image<T, byte> image) where T : struct, IColor
            {
                using var bitmap = image.ToBitmap();
                var hBitmap = bitmap.GetHbitmap();
                try
                {
                    return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        hBitmap,
                        System.IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
                finally
                {
                    DeleteObject(hBitmap);
                }
            }
        }
}
