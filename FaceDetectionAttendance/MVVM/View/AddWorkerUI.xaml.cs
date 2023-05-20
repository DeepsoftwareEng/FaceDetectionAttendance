using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Emgu.CV.Structure;
using Emgu.CV;
using FaceDetectionAttendance.MVVM.Model;
using Microsoft.Data.SqlClient;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Globalization;

namespace FaceDetectionAttendance.MVVM.View
{
    /// <summary>
    /// Interaction logic for AddWorkerUI.xaml
    /// </summary>
    public partial class AddWorkerUI : Page
    {
        private Dataconnecttion dtc = new Dataconnecttion();
        private SqlCommand SQLcommand = new SqlCommand();
        VideoCapture capture;
        static readonly CascadeClassifier faceDetector = new CascadeClassifier("haarcascade_frontalface_defult.xml");
        Image<Bgr, byte> image = null;
        DispatcherTimer timer = new DispatcherTimer();
        private bool iscapturing = false;
        public AddWorkerUI()
        {
            InitializeComponent();
            Add_SetComboBoxData();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FullNametxt.Text) ||
               Dobtxt.Text.Length == 0 ||
               Falcutybox.SelectedItem == null
               )
            {
                MessageBox.Show("Hãy nhập đủ thông tin.");
                return;
            }
            string fullname = FullNametxt.Text;
            DateTime DoB = DateTime.ParseExact($"{Dobtxt.Text}", "dd/MM/yyyy",
                                        CultureInfo.InvariantCulture); ;
            string faculty =    Falcutybox.SelectedItem.ToString();
            FullNametxt.Text = "";
            Dobtxt.Text = "";
            Falcutybox.SelectedItem = null;

            /* add du kieu*/
            string binFolderPath = System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            string projectFolderPath = Directory.GetParent(binFolderPath).FullName;
            string fix = projectFolderPath.Remove(projectFolderPath.Length - 9);
            string resourceFolderPath = System.IO.Path.Combine(fix, "Resource");
            string avt = resourceFolderPath + @$"\Avatar\{FullNametxt}.png";
            //     File.Copy("C:\\FDA temp\\temp.png", avt);
            string querry = "Insert into WorkerList Values(@fullname, @birth,@images, @fid)";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            try
            {
                SQLcommand = new SqlCommand(querry, dtc.GetConnection());
                SQLcommand.Parameters.Add("@fullname", SqlDbType.NVarChar).Value = FullNametxt.Text;
                SQLcommand.Parameters.Add("@birth", SqlDbType.Date).Value = Dobtxt.Text;
                SQLcommand.Parameters.Add("@fid", SqlDbType.NVarChar).Value = Falcutybox.SelectedItem.ToString();
                SQLcommand.Parameters.Add("@images", SqlDbType.NVarChar).Value = FullNametxt.Text;
                SQLcommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Backbtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
        private void Add_SetComboBoxData()
        {

            string querry = "Select* from Faculty";
            if (dtc.GetConnection().State == System.Data.ConnectionState.Closed)
                dtc.GetConnection().Open();
            SQLcommand = new SqlCommand(querry, dtc.GetConnection());
            SqlDataReader reader = SQLcommand.ExecuteReader();
            while (reader.Read())
            {
                Faculty a = new Faculty();
                a.IdFaculty = reader.GetString(0);
                a.NameFaculty = reader.GetString(1);
                Falcutybox.Items.Add(a.IdFaculty);
            }
           
        }
        private void Startcam_Click(object sender, RoutedEventArgs e)
        {
            capture = new VideoCapture();
            capture.Set(Emgu.CV.CvEnum.CapProp.FrameWidth, 200);
            capture.Set(Emgu.CV.CvEnum.CapProp.FrameHeight, 200);
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(60);
            timer.Start();
        }

        async void Timer_Tick(object? sender, EventArgs e)
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
            webcam.Source = bitmap;
        }

        private void Capture_Click(object sender, RoutedEventArgs e)
        {
            string binFolderPath = System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            string projectFolderPath = Directory.GetParent(binFolderPath).FullName;
            string fix = projectFolderPath.Remove(projectFolderPath.Length - 9);
            string resourceFolderPath = Path.Combine(fix, "Resource");
            iscapturing = true;
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
                    if (iscapturing == true)
                    {
                        // Crop the face image
                        Rectangle face = faces[0];
                        Image<Gray, byte> faceImage = image.Convert<Gray, byte>().Copy(face);
                        string nameimg = FullNametxt.Text;
                        string imagePath = $"{resourceFolderPath}\\WorkerImage\\{Falcutybox.SelectedItem.ToString()}\\{nameimg}.png";

                        if (File.Exists(imagePath))
                        {
                            ResultImg.Source = null;
                            File.Delete(imagePath);
                        }
                        Resize(faceImage).Save(imagePath);

                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                        bitmap.EndInit();

                        ResultImg.Source = bitmap;

                        MessageBox.Show("Worker added successfully.");

                    }
                    timer.Stop();
                    capture.Dispose();
                    break;
                }
            }
        }
        Image<Gray, byte> Resize(Image<Gray, byte> image)
        {
            System.Drawing.Size size = new System.Drawing.Size(200, 200);
            Image<Gray, byte> temp = image.Resize(size.Width, size.Height, Emgu.CV.CvEnum.Inter.Cubic);
            return temp;
        }
    } 
}
