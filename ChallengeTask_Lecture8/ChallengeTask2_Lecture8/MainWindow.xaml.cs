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


namespace ChallengeTask2_Lecture8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor sensor;
        Skeleton[] totalSkeleton = new Skeleton[6];


        public MainWindow()
        {

            InitializeComponent();
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.sensor = KinectSensor.KinectSensors[0];
            this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
            this.sensor.SkeletonStream.Enable();
            this.sensor.SkeletonFrameReady += skeletonFrameReady;
            //start the sensor.
            this.sensor.Start();

        }

        private Point ScalePosition(SkeletonPoint skeletonPoint)
        {
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.
                      MapSkeletonPointToDepthPoint(skeletonPoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);

        }

        void skeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
          //  canvas1.Children.Clear();

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame == null)
                {
                    return;
                }
                skeletonFrame.CopySkeletonDataTo(totalSkeleton);
                Skeleton skeleton = (from trackskeleton in totalSkeleton
                                          where trackskeleton.TrackingState == SkeletonTrackingState.Tracked
                                          select trackskeleton).FirstOrDefault();
                if (skeleton == null)
                {
                    return;
                }
                if (skeleton.Joints[JointType.HandRight].TrackingState ==
                   JointTrackingState.Tracked)
                {
                //    if (firstSkeleton.Joints[JointType.HandLeft].TrackingState ==
                  //  JointTrackingState.Tracked)
                    {

                        this.MapJointsWithUIElement(skeleton);
                    }

                }

            }
        }

    
        private void MapJointsWithUIElement(Skeleton skeleton)
        {
            Point mappedPoint = ScalePosition(skeleton.Joints[JointType.HandRight].Position);
            Point mappedPointL = ScalePosition(skeleton.Joints[JointType.HandLeft].Position);

            Color Red = Colors.Red;
            Color Green = Colors.Green;
            Color Blue = Colors.Blue;
            Color Yellow = Colors.Yellow;
            Color Black = Colors.Black;

            Point erasept = new Point(38, 30);
            Point redpt = new Point(38, 92);
            Point greenpt = new Point(38, 140);
            Point bluept = new Point(38, 188);
            Point yellowpt = new Point(38, 229);
            Vector vector1 = new Vector(10, 5);

            Point redresult = Point.Add(redpt, vector1);
            Point greenresult = Point.Add(greenpt, vector1);
            Point yellowresult = Point.Add(yellowpt, vector1); 
            Point blueresult = Point.Add(bluept, vector1);
            Point eraseresult = Point.Add(erasept, vector1);
            //Point leftsuub = Point.Subtract(mappedPointL, vector2);
            Point redresultsub = Point.Subtract(redpt, vector1);
            Point greenresultsub = Point.Subtract(greenpt, vector1);
            Point yellowresultsub = Point.Subtract(yellowpt, vector1);
            Point blueresultsub = Point.Subtract(bluept, vector1);
            Point eraseresultsub = Point.Subtract(erasept, vector1);

        


          //  righthand = new Ellipse();
           // righthand.Width = 50; righthand.Height = 50;
            // canvas1.Children.Clear();
            //  righthand.SetValue(Canvas.LeftProperty, mappedPoint.X - 15);
            // righthand.SetValue(Canvas.TopProperty, mappedPoint.Y - 15);
            // canvas1.Children.Add(righthand);
          // Canvas.SetLeft(righthand, mappedPoint.X);
           // Canvas.SetTop(righthand, mappedPoint.Y);
            canvas2.Children.Clear();
            lefthand = new Ellipse();
            lefthand.Width = 20; lefthand.Height = 20;
            lefthand.Fill = new SolidColorBrush(Black);
            Canvas.SetLeft(lefthand, mappedPointL.X);
            Canvas.SetTop(lefthand, mappedPointL.Y);
            // canvas1.Children.Add(lefthand);
            canvas2.Children.Add(lefthand);


            if (mappedPointL.X <= redresult.X && mappedPointL.X >= redresultsub.X)
            {
                if (mappedPointL.Y <= redresult.Y && mappedPointL.Y >= redresultsub.Y)
                {
                    /*    righthand.SetValue(Canvas.LeftProperty, mappedPoint.X);
                        righthand.SetValue(Canvas.TopProperty, mappedPoint.Y);
                        righthand.Fill = new SolidColorBrush(Red);
                        Canvas.SetLeft(lefthand, mappedPointL.X);
                        Canvas.SetTop(lefthand, mappedPointL.Y);
                        canvas1.Children.Add(righthand);*/
              //      canvas1.Children.Clear();

                    righthand = new Ellipse();
                    righthand.Width = 30; righthand.Height = 30;
                    righthand.SetValue(Canvas.LeftProperty, mappedPoint.X - 15);
                    righthand.SetValue(Canvas.TopProperty, mappedPoint.Y - 15);
                    righthand.Fill = new SolidColorBrush(Red);
                    canvas1.Children.Add(righthand);
                }

            }

            if (mappedPointL.X <= greenresult.X && mappedPointL.X >= greenresultsub.X)
            {
                if (mappedPointL.Y <= greenresult.Y && mappedPointL.Y >= greenresultsub.Y)
                {
                  //  canvas1.Children.Clear();
                    righthand = new Ellipse();
                    righthand.Width = 30; righthand.Height = 30;
                    righthand.SetValue(Canvas.LeftProperty, mappedPoint.X - 15);
                    righthand.SetValue(Canvas.TopProperty, mappedPoint.Y - 15);
                    righthand.Fill = new SolidColorBrush(Green);
                    canvas1.Children.Add(righthand);
                }

            }
            if (mappedPointL.X <= yellowresult.X && mappedPointL.X >= yellowresultsub.X)
            {
                if (mappedPointL.Y <= yellowresult.Y && mappedPointL.Y >= yellowresultsub.Y)
                {
                  //  canvas1.Children.Clear();

                    righthand = new Ellipse();
                    righthand.Width = 30; righthand.Height = 30;
                    righthand.SetValue(Canvas.LeftProperty, mappedPoint.X - 15);
                    righthand.SetValue(Canvas.TopProperty, mappedPoint.Y - 15);
                    righthand.Fill = new SolidColorBrush(Yellow);
                    canvas1.Children.Add(righthand);
                }
            }
            if (mappedPointL.X <= blueresult.X && mappedPointL.X >= blueresultsub.X)
            {
                if (mappedPointL.Y <= blueresult.Y && mappedPointL.Y >= blueresultsub.Y)
                {
                   // canvas1.Children.Clear();

                    righthand = new Ellipse();
                    righthand.Width = 30; righthand.Height = 30;
                    righthand.SetValue(Canvas.LeftProperty, mappedPoint.X - 15);
                    righthand.SetValue(Canvas.TopProperty, mappedPoint.Y - 15);
                    righthand.Fill = new SolidColorBrush(Blue);
                    canvas1.Children.Add(righthand);

                }

            }
            if (mappedPointL.X <= eraseresult.X && mappedPointL.X >= eraseresultsub.X)
            {
                if (mappedPointL.Y <= eraseresult.Y && mappedPointL.Y >= eraseresultsub.Y)
                {
                    canvas1.Children.Clear();
                 //   this.MapJointsWithUIElement(firstskeleton);
                } 
            }
        }
        // this.textBox1.Text = "x="+mappedPoint.X+", y="+mappedPoint.Y;






    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {

    }

    private void Ellipse_Loaded(object sender, RoutedEventArgs e)
    {

    }
        
    }
}





