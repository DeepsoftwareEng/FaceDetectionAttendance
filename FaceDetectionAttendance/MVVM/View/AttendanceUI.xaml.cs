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
using System.Windows.Threading;
using Emgu.CV.Structure;
using Emgu.CV;
using Emgu.CV.UI;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Pkcs;
using System.Text.RegularExpressions;
using System.Drawing;
using Emgu.CV.CvEnum;
using System.Diagnostics;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for AttendanceUI.xaml
    /// </summary>
    public partial class AttendanceUI : Page
    {
        private VideoCapture _videoSource;
        private bool _isCapturing = false;
        private CascadeClassifier _faceClassifier;
        private DispatcherTimer timer;

        public AttendanceUI()
        {
            InitializeComponent();
            string HaarFilePath = @"D:\wpf\FDA\FaceDetectionAttendance\HaarCascade\haarcascade_frontalface_default.xml";
            _faceClassifier = new CascadeClassifier(HaarFilePath);
        }
        private void StartCam_Click(object sender, RoutedEventArgs e)
        {
            if (!_isCapturing)
            {
                _videoSource = new VideoCapture();
                _videoSource.Start();
                _isCapturing = true;

                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
        }
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            // Capture a frame from the video source
            var frame = _videoSource.QueryFrame();

            // Convert the captured frame to a grayscale image
            var grayImage = new Image<Bgr, byte>(frame.Size);
            CvInvoke.CvtColor(frame, grayImage, ColorConversion.Bgr2Gray);

            // Detect faces in the current frame using the HaarCascade face detector
            var detectedFaces = _faceClassifier.DetectMultiScale(grayImage, 1.2, 5, System.Drawing.Size.Empty);

            // Draw rectangles around the detected faces
            foreach (var face in detectedFaces)
            {
                var rect = new Rectangle(face.X, face.Y, face.Width, face.Height);
                CvInvoke.Rectangle(frame, rect, new Bgr(System.Drawing.Color.Red).MCvScalar, 2);
            }

            // Update the video display with the current frame
            Bitmap image = grayImage.ToBitmap();
            VideoDisplay.Source = BitmapToImageSource(image);
        }
        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new System.IO.MemoryStream(ImageToByte(bitmap));
            bitmapImage.EndInit();
            return bitmapImage;
        }
        private byte[] ImageToByte(System.Drawing.Image img)
        {
            var converter = new System.Drawing.ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }
        private void StopCam_Click(object sender, RoutedEventArgs e)
        {
            if (_isCapturing)
            {
                _videoSource.Stop();
                _isCapturing = false;
                // Stop the frame capture timer
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            }
        }
        //protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        //{
        //    if (_isCapturing)
        //    {
        //        _videoSource.Stop();
        //    }
        //    base.OnClosing(e);
        //}
    }
}
