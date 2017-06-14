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
using Microsoft.Xna.Framework;
using LightBuzz.Vitruvius;



namespace ChallengeTask_lecture9
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
        Skeleton[] totalSkeleton = new Skeleton[6];
        WriteableBitmap colorBitmap;
        byte[] colorPixels;
        Skeleton skeleton;
        int currentSkeletonID = 0;
        void skeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            canvas.Children.Clear();

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame == null) { return; }
                skeletonFrame.CopySkeletonDataTo(totalSkeleton);
                skeleton = (from trackskeleton in totalSkeleton
                            where trackskeleton.TrackingState == SkeletonTrackingState.Tracked
                            select trackskeleton).FirstOrDefault();
                if (skeleton == null)
                    return;
                if (skeleton != null && this.currentSkeletonID != skeleton.TrackingId)
                {
                    this.currentSkeletonID = skeleton.TrackingId;
                    int totalTrackedJoints = skeleton.Joints.Where(item => item.TrackingState == JointTrackingState.Tracked).Count();
                    string TrackedTime = DateTime.Now.ToString("hh:mm:ss");
                    string status = "Skeleton Id: " + this.currentSkeletonID + ", total tracked joints: " + totalTrackedJoints + ", TrackTime: " + TrackedTime + "\n";
                    // this.textblock.Text += status;

                }
                DrawSkeleton(skeleton);
                Point spinePt = ScalePosition(skeleton.Joints[JointType.Spine].Position);
                Point handPt = ScalePosition(skeleton.Joints[JointType.WristRight].Position);
                Point shouldr = ScalePosition(skeleton.Joints[JointType.ShoulderRight].Position);

                Vector3 a1 = new Vector3(skeleton.Joints[JointType.WristRight].Position.X, skeleton.Joints[JointType.WristRight].Position.Y, skeleton.Joints[JointType.WristRight].Position.Z);
                Vector3 a2 = new Vector3(skeleton.Joints[JointType.Spine].Position.X, skeleton.Joints[JointType.Spine].Position.Y, skeleton.Joints[JointType.Spine].Position.Z);
                Vector3 a3 = new Vector3(skeleton.Joints[JointType.ShoulderRight].Position.X, skeleton.Joints[JointType.ShoulderRight].Position.Y, skeleton.Joints[JointType.ShoulderRight].Position.Z);
                Vector3 b1 = a3 - a1;
                Vector3 b2 = a3 - a2;
                var angle = AngleBetweenTwoVectors(b2, b1);
                AngleBetweenTwoVectors(b2, b1).ToString();

                this.textblock.Text = "" + angle;
                AddCircularArcGraph(spinePt, handPt, new Size(20, 25));

            //    Canvas.SetLeft(textblock, handPt.X ); 
             //  Canvas.SetTop(textblock, handPt.Y );

            }
        }
        
        private void AddCircularArcGraph(Point startPoint, Point endPoint, Size size)
        {
            
            PathFigure pf = new PathFigure();
            pf.StartPoint = new Point(startPoint.X, startPoint.Y);
            ArcSegment arcSegment = new ArcSegment();

            arcSegment.Point = new Point(endPoint.X, endPoint.Y);
            arcSegment.Size = size;
            arcSegment.SweepDirection = SweepDirection.Counterclockwise;

            PathSegmentCollection psc = new PathSegmentCollection();
            psc.Add(arcSegment); pf.Segments = psc;

            PathFigureCollection pfc = new PathFigureCollection(); pfc.Add(pf);
            PathGeometry pg = new PathGeometry(); pg.Figures = pfc;

            var path = new Path();
            path.Stroke = Brushes.Black;
            path.StrokeThickness = 1; path.Data = pg;
            canvas.Children.Add(path);
          Canvas.SetLeft(textblock, startPoint.X + 30);
          Canvas.SetTop(textblock, endPoint.Y - 10);


        }

        public double AngleBetweenTwoVectors(Vector3 vectorA, Vector3 vectorB)
        {
            double dotProduct = 0.0;
            vectorA.Normalize();
            vectorB.Normalize();
            dotProduct = Vector3.DotProduct(vectorA, vectorB);

            return (double)Math.Acos(dotProduct) / Math.PI * 180;
        }

        private void DrawSkeleton(Skeleton skeleton)
        {
            drawBone(skeleton.Joints[JointType.Head], skeleton.Joints[JointType.ShoulderCenter]);
            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.Spine]);
            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderLeft]);
            drawBone(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft]);
            drawBone(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft]);
            drawBone(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.HandLeft]);
            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderRight]);
            drawBone(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight]);
            drawBone(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight]);
            drawBone(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.HandRight]);
            drawBone(skeleton.Joints[JointType.Spine], skeleton.Joints[JointType.HipCenter]);
            drawBone(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipLeft]);
            drawBone(skeleton.Joints[JointType.HipLeft], skeleton.Joints[JointType.KneeLeft]);
            drawBone(skeleton.Joints[JointType.KneeLeft], skeleton.Joints[JointType.AnkleLeft]);
            drawBone(skeleton.Joints[JointType.AnkleLeft], skeleton.Joints[JointType.FootLeft]);
            drawBone(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipRight]);
            drawBone(skeleton.Joints[JointType.HipRight], skeleton.Joints[JointType.KneeRight]);
            drawBone(skeleton.Joints[JointType.KneeRight], skeleton.Joints[JointType.AnkleRight]);
            drawBone(skeleton.Joints[JointType.AnkleRight], skeleton.Joints[JointType.FootRight]);
        }
        private Point ScalePosition(SkeletonPoint skeletonPoint)
        {
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeletonPoint, DepthImageFormat.Resolution320x240Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }
        void drawBone(Joint trackedJoint1, Joint trackedJoint2)
        {
            Line bone = new Line();
            bone.Stroke = Brushes.Red;
            bone.StrokeThickness = 3;
            Point joint1 = this.ScalePosition(trackedJoint1.Position);
            bone.X1 = joint1.X;
            bone.Y1 = joint1.Y;
            Point joint2 = this.ScalePosition(trackedJoint2.Position);
            bone.X2 = joint2.X;
            bone.Y2 = joint2.Y;
            canvas.Children.Add(bone);
        }

        void colorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                if (null == imageFrame)
                    return;

                imageFrame.CopyPixelDataTo(colorPixels);
                int stride = imageFrame.Width * imageFrame.BytesPerPixel;

                // Write the pixel data into our bitmap
                this.colorBitmap.WritePixels(
                    new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels, stride, 0);
            }
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.sensor = KinectSensor.KinectSensors[0];
            this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
            this.sensor.SkeletonStream.Enable();
            this.sensor.SkeletonFrameReady += skeletonFrameReady;
            this.sensor.ColorStream.Enable();
            this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];
            this.colorBitmap = new WriteableBitmap(this.sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
            this.image.Source = this.colorBitmap;
            this.sensor.ColorFrameReady += this.colorFrameReady;
            // start the sensor.
            this.sensor.Start();

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }


    }
}
