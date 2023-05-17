using FaceDetectionAttendance.MVVM.Model;
using Microsoft.Data.SqlClient;
using System.Drawing;
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
using Emgu.CV;
using Emgu.CV.UI;
using System.Text.RegularExpressions;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Emgu.CV.Structure;
using static System.Net.Mime.MediaTypeNames;
using System.IO;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for AddWorkerUI.xaml
    /// </summary>
    public partial class AddWorkerUI : Page
    {
        Dataconnecttion dtc = new Dataconnecttion();
        static readonly CascadeClassifier faceDetector = new CascadeClassifier("D:\\Download\\Webcam-master\\Webcam-master\\Webcam\\haarcascade_frontalface_default.xml");
        bool iscapture = false;
        VideoCapture capture;
        Image<Bgr, byte> image = null;
        DispatcherTimer timer = new DispatcherTimer();

        private void ComboBox_Reload()
        {
            string querry = "Select id_faculty From Faculty";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
            {
                dtc.GetConnection().Open();
            }
            SqlCommand cmd = new SqlCommand(querry, dtc.GetConnection());
            SqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                string item = dataReader.GetString(0);
                Faculty_ComboBox.Items.Add(item);
            }
            Faculty_ComboBox.SelectedIndex = 0;
        }
        public AddWorkerUI()
        {
            InitializeComponent();
            ComboBox_Reload();
        }

        async void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            // Create a timer to update the webcam feed
            capture = new VideoCapture();
            capture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 200);
            capture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 200);
            timer.Interval = TimeSpan.FromMilliseconds(144);
            timer.Tick += (s, ev) =>
            {
                Mat frame = new Mat();
                capture.Read(frame);
                // Convert the image to a BitmapSource that can be displayed in the Image control
                Image<Bgr, byte> images = frame.ToImage<Bgr, byte>();
                //
                Rectangle[] faces = faceDetector.DetectMultiScale(images.Convert<Gray, byte>(), 1.2, 10, System.Drawing.Size.Empty);

                // Draw rectangles around the faces
                foreach (Rectangle face in faces)
                {
                    images.Draw(face, new Bgr(System.Drawing.Color.Red), 2);
                }
                BitmapSource bitmap = BitmapSourceConvert.ToBitmapSource(images);
                // Set the Image control's Source property to the BitmapSource
                webcam_image.Source = bitmap;

            };
            timer.Start();
        }

        private void Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            capture.Stop();
            capture.Dispose();
        }

        private void AddWorker_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not yet", "Error", MessageBoxButton.OK);
        }

        private void Capture_Button_Click(object sender, RoutedEventArgs e)
        {
            iscapture = true;
            using (capture)
            {

                // Capture images from the webcam
                while (true)
                {
                    Mat frame = new Mat();
                    capture.Read(frame);
                    image = frame.ToImage<Bgr, byte>();

                    // Detect faces in the image
                    Rectangle[] faces = faceDetector.DetectMultiScale(image.Convert<Gray, byte>(), 1.2, 10, System.Drawing.Size.Empty);

                    // Draw rectangles around the faces
                    foreach (Rectangle face in faces)
                    {
                        image.Draw(face, new Bgr(System.Drawing.Color.Red), 2);
                    }

                    // Update the image control with the latest image
                    //result.Source = BitmapSourceConvert.ToBitmapSource(image);
                    // Wait for the user to click on the image control
                    if (iscapture == true)
                    {
                        // Crop the face image
                        Rectangle face = faces[0];
                        Image<Gray, byte> faceImage = image.Convert<Gray, byte>().Copy(face);

                        string nameimg = FullName_Txb.Text;
                        string imagePath = "D:\\"+nameimg+".png";
                        
                        if (File.Exists(imagePath))
                        {
                            File.Delete(imagePath);
                        }
                        faceImage.Save(imagePath);

                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(imagePath);
                        bitmap.EndInit();
                        Worker_Image.Source = bitmap;

                        MessageBox.Show("Worker added successfully.");
                    }
                    timer.Stop();
                    capture.Dispose();
                    break;
                }
            }
        }

    }
}
