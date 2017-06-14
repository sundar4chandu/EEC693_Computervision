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
using System.IO;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.AudioFormat;

namespace Challengetask_lecture12
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor sensor;
        Stream audioStream;
        Skeleton[] totalSkeleton = new Skeleton[6];

        SpeechRecognitionEngine speechEngine;
        WriteableBitmap colorBitmap;
        private byte[] colorPixels;
        Skeleton skeleton;
        int currentSkeletonID = 0;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(WindowLoaded);

        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            this.sensor = KinectSensor.KinectSensors[0];
            if (KinectSensor.KinectSensors.Count > 0)
            {
                this.sensor = KinectSensor.KinectSensors[0];
                if (this.sensor != null && !this.sensor.IsRunning)
                {
                    this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
                    this.sensor.SkeletonStream.Enable();
                    this.sensor.SkeletonFrameReady += skeletonFrameReady;
                    this.sensor.ColorStream.Enable();
                    this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];
                    this.colorBitmap = new WriteableBitmap(this.sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
                    this.image.Source = this.colorBitmap;
                    this.sensor.ColorFrameReady += this.colorFrameReady;

                    this.sensor.Start();
                }

                /*  else {
                      MessageBox.Show("No device is connected!");
                      this.Close();
                  }*/

                audioStream = this.sensor.AudioSource.Start();
                RecognizerInfo recognizerInfo = GetKinectRecognizer();
                if (recognizerInfo == null)
                {
                    MessageBox.Show("Could not find Kinect speech recognizer");
                    return;
                }

                BuildGrammarforRecognizer(recognizerInfo); // provided earlier
                statusBar.Text = "Speech Recognizer is ready";
            }
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


        void skeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            // canvas1.Children.Clear();
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
                }


                //   DrawSkeleton(skeleton);
            }
        }
    

        private Point ScalePosition(SkeletonPoint skeletonPoint)
        {
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeletonPoint, DepthImageFormat.Resolution320x240Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        private void speechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        { }

        private void speechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            wordsTenative.Text = e.Result.Text;
        }

        private void speechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            wordsRecognized.Text = e.Result.Text;
            confidenceTxt.Text = e.Result.Confidence.ToString();
            float confidenceThreshold = 0.4f;
            if (e.Result.Confidence > confidenceThreshold)
            {
                CommandsParser(e);
            }
        }
        private void MapJointsWithUIElements(Skeleton skeleton)
        {


            Point mappedPoint = ScalePosition(skeleton.Joints[JointType.HandRight].Position);

            Point mappedPoint1 = ScalePosition(skeleton.Joints[JointType.HandLeft].Position);
        
            Point mappedPoint2 = ScalePosition(skeleton.Joints[JointType.Head].Position);
            Point mappedPoint3 = ScalePosition(skeleton.Joints[JointType.HipCenter].Position);
        }
        private void CommandsParser(SpeechRecognizedEventArgs e)
        {
            var result = e.Result;
        
       
            //this.textBox1.Text = "x="+mappedPoint.X+", y="+mappedPoint.Y;


            Color objectColor;
            Shape drawObject;

           System.Collections.ObjectModel.ReadOnlyCollection<RecognizedWordUnit> words = e.Result.Words;

            if (words[0].Text == "draw")
            {
                string colorObject = words[1].Text;
                switch (colorObject)
                {
                    case "red":
                        objectColor = Colors.Red;
                        break;
                    case "green":
                        objectColor = Colors.Green;
                        break;
                    case "blue":
                        objectColor = Colors.Blue;
                        break;
                    case "yellow":
                        objectColor = Colors.Yellow;
                        break;
                    case "gray":
                        objectColor = Colors.Gray;
                        break;
                    case "pink":
                        objectColor = Colors.Pink;
                        break;
                    default:
                        return;
                }


                var shapeString = words[2].Text;
                switch (shapeString)
                {
                    case "circle":

                        string drawposition = words[3].Text;

                        if (drawposition == "righthand")
                        {

                            Point mappedPoint = ScalePosition(skeleton.Joints[JointType.HandRight].Position);
                            canvas1.Children.Clear();
                            drawObject = new Ellipse();
                            drawObject.Width = 25; drawObject.Height = 25;
                            drawObject.Fill = new SolidColorBrush(objectColor);
                          //  Canvas.SetLeft(drawObject, mappedPoint.X);
                           //Canvas.SetTop(drawObject, mappedPoint.Y);
                           drawObject.SetValue(Canvas.LeftProperty, mappedPoint.X);
                           drawObject.SetValue(Canvas.TopProperty, mappedPoint.Y);
                            canvas1.Children.Add(drawObject);
                            return;

                        }


                        else if (drawposition == "lefthand")
                        {
                            Point mappedPoint1 = ScalePosition(skeleton.Joints[JointType.HandLeft].Position);
                            canvas1.Children.Clear();

                            drawObject = new Ellipse();
                            drawObject.Width = 25; drawObject.Height = 25;
                            drawObject.SetValue(Canvas.LeftProperty, mappedPoint1.X);
                            drawObject.SetValue(Canvas.TopProperty, mappedPoint1.Y);
                            drawObject.Fill = new SolidColorBrush(objectColor);
                            canvas1.Children.Add(drawObject);
                            return;
                        }

                        else if (drawposition == "head")
                        {
                            Point mappedPoint2 = ScalePosition(skeleton.Joints[JointType.Head].Position);
                            canvas1.Children.Clear();

                            drawObject = new Ellipse();
                            drawObject.Width = 25; drawObject.Height = 25;
                            drawObject.SetValue(Canvas.LeftProperty, mappedPoint2.X);
                            drawObject.SetValue(Canvas.TopProperty, mappedPoint2.Y);
                            drawObject.Fill = new SolidColorBrush(objectColor);
                            canvas1.Children.Add(drawObject);
                        } 

                        else if (drawposition == "hip")
                        {
                            Point hip = newPosition(skeleton.Joints[JointType.HipCenter].Position);
                            canvas1.Children.Clear();
                            drawObject = new Ellipse();
                            drawObject.Width = 25; drawObject.Height = 25;
                            drawObject.SetValue(Canvas.LeftProperty, hip.X);
                            drawObject.SetValue(Canvas.TopProperty, hip.Y);
                            drawObject.Fill = new SolidColorBrush(objectColor);
                            canvas1.Children.Add(drawObject);
                        }
                        //   
                        break;

                    case "square":
                        string drawposition1 = words[3].Text;
                        if (drawposition1 == "righthand")
                        {
                            Point mappedPoint = ScalePosition(skeleton.Joints[JointType.HandRight].Position);

                            drawObject = new Rectangle();
                            drawObject.Width = 25; drawObject.Height = 25;
                            canvas1.Children.Clear();
                            drawObject.SetValue(Canvas.LeftProperty, mappedPoint.X);
                            drawObject.SetValue(Canvas.TopProperty, mappedPoint.Y);
                            drawObject.Fill = new SolidColorBrush(objectColor);
                            canvas1.Children.Add(drawObject);
                            
                        }

                      else  if (drawposition1 == "lefthand")
                        {
                            Point mappedPoint1 = ScalePosition(skeleton.Joints[JointType.HandLeft].Position);

                            drawObject = new Rectangle();
                            drawObject.Width = 25; drawObject.Height = 25;
                            canvas1.Children.Clear();
                            drawObject.SetValue(Canvas.LeftProperty, mappedPoint1.X);
                            drawObject.SetValue(Canvas.TopProperty, mappedPoint1.Y);
                            drawObject.Fill = new SolidColorBrush(objectColor);
                            canvas1.Children.Add(drawObject);
                        }
                     else   if (drawposition1 == "head")
                        {
                            Point mappedPoint1 = ScalePosition(skeleton.Joints[JointType.Head].Position);

                            canvas1.Children.Clear();
                            drawObject = new Rectangle();
                            drawObject.Width = 100; drawObject.Height = 100;
                            drawObject.SetValue(Canvas.LeftProperty, mappedPoint1.X);
                            drawObject.SetValue(Canvas.TopProperty, mappedPoint1.Y);
                            drawObject.Fill = new SolidColorBrush(objectColor);
                            canvas1.Children.Add(drawObject);
                        }

                        else if (drawposition1 == "hip")
                        {

                            Point mappedPoint = newPosition(skeleton.Joints[JointType.HipCenter].Position);
                            canvas1.Children.Clear();
                            drawObject = new Rectangle();
                            drawObject.Width = 100; drawObject.Height = 100;
                            Canvas.SetLeft(drawObject, mappedPoint.X);
                            Canvas.SetTop(drawObject, mappedPoint.Y);
                            drawObject.Fill = new SolidColorBrush(objectColor);
                            canvas1.Children.Add(drawObject);
                            break;
                        }
                      
                        break;
                    case "rectangle":
                        string drawposition2 = words[3].Text;
                        if (drawposition2 == "righthand")
                        {
                            drawObject = new Rectangle();
                            drawObject.Width = 100; drawObject.Height = 60;
                            Point mappedPoint = ScalePosition(skeleton.Joints[JointType.HandRight].Position);
                            canvas1.Children.Clear();

                            Canvas.SetLeft(drawObject, mappedPoint.X);
                            Canvas.SetTop(drawObject, mappedPoint.Y);
                            drawObject.Fill = new SolidColorBrush(objectColor);
                            canvas1.Children.Add(drawObject);
                        }
                        break;
                    case "triangle":
                        var polygon = new Polygon();
                        polygon.Points.Add(new Point(0, 30));
                        polygon.Points.Add(new Point(-60, -30));
                        polygon.Points.Add(new Point(60, -30));
                        drawObject = polygon;
                        break;
                    default:
                        return;
                }
            }
               // string drawposition = words[3].Text;
                //switch (drawposition)
               /* {
                    case " righthand":
                        Point mappedPoint = ScalePosition(skeleton.Joints[JointType.HandRight].Position);
                        canvas1.Children.Clear();

                             Canvas.SetLeft(drawObject, mappedPoint.X);
                         Canvas.SetTop(drawObject, mappedPoint.Y);
                        drawObject.SetValue(Canvas.LeftProperty, mappedPoint.X - 15);
                        drawObject.SetValue(Canvas.TopProperty, mappedPoint.Y - 15);
                        drawObject.Fill = new SolidColorBrush(objectColor);
                        canvas1.Children.Add(drawObject);
                        break;

                    case " lefthand":
                        Point lefthand = ScalePosition(skeleton.Joints[JointType.HandLeft].Position);
                        canvas1.Children.Clear();

                          Canvas.SetLeft(drawObject, lefthand.X);
                        Canvas.SetTop(drawObject, lefthand.Y);
                        drawObject.SetValue(Canvas.LeftProperty, lefthand.X - 15);
                        drawObject.SetValue(Canvas.TopProperty, lefthand.Y - 15);
                        drawObject.Fill = new SolidColorBrush(objectColor);
                        canvas1.Children.Add(drawObject);
                        break;

                    case " head":
                        Point head = newPosition(skeleton.Joints[JointType.Head].Position);
                        canvas1.Children.Clear();

                        //   Canvas.SetLeft(drawObject, head.X);
                        // Canvas.SetTop(drawObject, head.Y);
                        drawObject.SetValue(Canvas.LeftProperty, head.X - 15);
                        drawObject.SetValue(Canvas.TopProperty, head.Y - 15);
                        drawObject.Fill = new SolidColorBrush(objectColor);

                        canvas1.Children.Add(drawObject);


                        break;
                    case " hip":
                        
                    default:
                        return;
                }
            }

            /* canvas1.Children.Clear();
             drawObject.SetValue(Canvas.LeftProperty, JointType.HipCenter );
              drawObject.SetValue(Canvas.TopProperty, JointType.HipCenter);
                drawObject.Fill = new SolidColorBrush(objectColor);
             canvas1.Children.Add(drawObject);
               }*/

            if (words[0].Text == "close" && words[1].Text == "the" && words[2].Text == "application")
            {
                this.Close();
            }
        }
        private Point newPosition(SkeletonPoint skeletonPoint)
        {
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.
                      MapSkeletonPointToDepthPoint(skeletonPoint, DepthImageFormat.
                                 Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }
        private void BuildGrammarforRecognizer(RecognizerInfo recognizerInfo)
        {
            var grammarBuilder = new GrammarBuilder { Culture = recognizerInfo.Culture };
            // first say Draw
            grammarBuilder.Append(new Choices("draw"));
            var colorObjects = new Choices();
            colorObjects.Add("red"); colorObjects.Add("green"); colorObjects.Add("blue");
            colorObjects.Add("yellow"); colorObjects.Add("gray"); colorObjects.Add("pink");
            // New Grammar builder for color
            grammarBuilder.Append(colorObjects);
            // Another Grammar Builder for object
            grammarBuilder.Append(new Choices("circle", "square", "triangle", "rectangle"));
            // Another Grammer Builder For Position
          /* var position = new Choices() ;
            position.Add("righthand"); position.Add("lefthand"); position.Add("rightleg");
            position.Add("leftleg"); position.Add("head"); position.Add("hip");*/
            grammarBuilder.Append(new Choices("righthand", "lefthand" , "head" , "hip"));

            // Create Grammar from GrammarBuilder
            var grammar = new Grammar(grammarBuilder);

            // Creating another Grammar and load
            var newGrammarBuilder = new GrammarBuilder();
            newGrammarBuilder.Append("close the application");
            var grammarClose = new Grammar(newGrammarBuilder);
            // Start the speech recognizer
            speechEngine = new SpeechRecognitionEngine(recognizerInfo.Id);
            speechEngine.LoadGrammar(grammar); // loading grammer into recognizer
            speechEngine.LoadGrammar(grammarClose);

            // Attach the speech audio source to the recognizer
            int SamplesPerSecond = 16000; int bitsPerSample = 16;
            int channels = 1; int averageBytesPerSecond = 32000; int blockAlign = 2;
            speechEngine.SetInputToAudioStream(
                 audioStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm,
                 SamplesPerSecond, bitsPerSample, channels, averageBytesPerSecond,
                  blockAlign, null));

            // Register the event handler for speech recognition
            speechEngine.SpeechRecognized += speechRecognized;
            speechEngine.SpeechHypothesized += speechHypothesized;
            speechEngine.SpeechRecognitionRejected += speechRecognitionRejected;

            speechEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }
            return null;
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}