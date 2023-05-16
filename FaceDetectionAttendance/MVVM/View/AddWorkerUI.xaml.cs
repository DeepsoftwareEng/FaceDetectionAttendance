using FaceDetectionAttendance.MVVM.Model;
using Microsoft.Data.SqlClient;
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
using Emgu.CV;
using Emgu.CV.UI;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Emgu.CV.Structure;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for AddWorkerUI.xaml
    /// </summary>
    public partial class AddWorkerUI : Page
    {
        Dataconnecttion dtc = new Dataconnecttion();
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
        VideoCapture capture = new VideoCapture();
        async void Button_Click(object sender, RoutedEventArgs e)
        {
            // Create a capture object to capture images from the webcam
            capture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 200);
            capture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 200);
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(60); // Update the image control 30 times per second
            timer.Tick += (s, ev) =>
            {
                // Query the capture object for the latest image from the webcam
                Mat frame = new Mat();
                capture.Read(frame);

                // Convert the image to a BitmapSource that can be displayed in the Image control
                Image<Bgr, byte> image = frame.ToImage<Bgr, byte>();
                BitmapSource bitmap = BitmapSourceConvert.ToBitmapSource(image);
                // Set the Image control's Source property to the BitmapSource
                Stream_image.Source = bitmap;
            };

            // Start the timer
            timer.Start();
        }

        private void Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            capture.Stop();
        }
    }
}
