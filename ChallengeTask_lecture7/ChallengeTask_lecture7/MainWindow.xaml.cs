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
using Microsoft.Kinect;

namespace ChallengeTask_lecture7
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {

            InitializeComponent();
        }
        KinectSensor sensor;
        private WriteableBitmap depthBitmap;
        private short[] depthPixels;
        private int frameWidth; // to calculate depthIndex

        void depthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame imageFrame = e.OpenDepthImageFrame())
            {
                if (null == imageFrame)
                {
                    return;
                }
                this.frameWidth = imageFrame.Width;
                this.textBox.Text = "" + imageFrame.MaxDepth;
                this.textBox1.Text = "" + imageFrame.MinDepth;
                byte[] pixels = GenerateColoredBytes(imageFrame);

                imageFrame.CopyPixelDataTo(depthPixels);
                int stride = imageFrame.Width * imageFrame.BytesPerPixel;
                this.depthBitmap.WritePixels(
                    new Int32Rect(0, 0, this.depthBitmap.PixelWidth, this.depthBitmap.PixelHeight),
                    this.depthPixels, stride, 0);
                image.Source = BitmapSource.Create(imageFrame.Width, imageFrame.Height, 96, 96, PixelFormats.Pbgra32, null, pixels, stride);
                
            }
        }
        private byte[] GenerateColoredBytes(DepthImageFrame imageFrame)
        {
            short[] rawDepthData = new short[imageFrame.PixelDataLength];
            imageFrame.CopyPixelDataTo(rawDepthData);
            Byte[] pixels = new byte[imageFrame.Height * imageFrame.Width * 4];
            const int BlueIndex = 0;
            const int GreenIndex = 1;
            const int RedIndex = 2;
            int depthIndex = 0;
            int colorIndex = 0;

            while (depthIndex < rawDepthData.Length && colorIndex < pixels.Length)
            {
                int player = rawDepthData[depthIndex] & DepthImageFrame.PlayerIndexBitmask;
                int depth = rawDepthData[depthIndex] >> DepthImageFrame.PlayerIndexBitmaskWidth;
                if (depth <= 2000)
                {
                    pixels[colorIndex + BlueIndex] = 0;
                    pixels[colorIndex + GreenIndex] = 69;
                    pixels[colorIndex + RedIndex] = 255;
                }
                else if (depth > 2000 && depth < 3000)
                {
                    pixels[colorIndex + BlueIndex] = 0;
                    pixels[colorIndex + GreenIndex] = 0;
                    pixels[colorIndex + RedIndex] = 255;
                }
                else if (depth > 3000)
                {
                    pixels[colorIndex + BlueIndex] = 0;
                    pixels[colorIndex + GreenIndex] = 255;
                    pixels[colorIndex + RedIndex] = 255;
                }
                depthIndex++;
                colorIndex += 4;
            }
            return pixels;
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                this.sensor = KinectSensor.KinectSensors[0];
                if (this.sensor != null && !this.sensor.IsRunning)
                {
                    this.sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
                    this.depthPixels = new short[this.sensor.DepthStream.FramePixelDataLength];
                    this.depthBitmap = new WriteableBitmap(this.sensor.DepthStream.FrameWidth, this.sensor.DepthStream.FrameHeight, 96.0, 96.0, PixelFormats.Gray16, null);

                    this.image.Source = this.depthBitmap;
                    this.sensor.DepthFrameReady += this.depthFrameReady;
                    this.sensor.Start();
                }
                else {
                    MessageBox.Show("No device is connected!"); this.Close();
                }

            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point currentPoint = e.GetPosition(image);
            this.textBox2.Text = currentPoint.X.ToString();
            this.textBox3.Text = currentPoint.Y.ToString();
            int pixelIndex = (int)(currentPoint.X + ((int)currentPoint.Y * this.frameWidth));
            this.textBox4.Text = "" + pixelIndex;
            int distancemm = this.depthPixels[pixelIndex] >> DepthImageFrame.PlayerIndexBitmaskWidth;
            this.textBox5.Text = "" + distancemm;
        }
    }
}
