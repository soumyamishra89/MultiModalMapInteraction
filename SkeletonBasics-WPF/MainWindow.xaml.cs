//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.Kinect;
    using System.Timers;

    using Maps.MapControl.WPF;
    using System;
    using System.Collections.Generic;
   
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // some high value is assigned at the begining to calibrate the hands for zoom in zoom out
        int referenceDistanceBetweenHands = 10000;
        
        /**List<Double> leftHandX = new List<double>();
        List<Double> leftHandY = new List<double>();
        List<Double> rightHandX = new List<double>();
        List<Double> rightHandY = new List<double>();**/
        /// <summary>
        /// Width of output drawing : window vertical
        /// </summary>
        private const float RenderWidth = 640.0f;

        /// <summary>
        /// Height of our output drawing window horizontal
        /// </summary>
        private const float RenderHeight = 480.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 5;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 1;
        //before : 10

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;
        // before : 10

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        //private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));
        private readonly Brush trackedJointBrush = Brushes.Black;

        /// <summary>
        /// Brush used for drawing joints that are currently inferred : déduits (pas vus ?)
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Black;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Transparent, 6);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Transparent, 1);

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        private DateTime handsclosed = default(DateTime);
        int zoomin = 0;
        int zoomout = 0;
        int time = 0;
        bool isZoomedIn = false;
        bool isZoomedOut = false;
        Joint joint1 = new Joint();
        Joint joint2 = new Joint();



        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            myMap.AnimationLevel = Maps.MapControl.WPF.AnimationLevel.Full; // adding a animation level to the Map to avoid jerk while transition
        }

        // Create a DrawingVisual that contains a rectangle.
        private DrawingVisual CreateDrawingVisualRectangle(DrawingContext drawingContext)
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            // Retrieve the DrawingContext in order to create new drawing content.
            //DrawingContext drawingContext = drawingVisual.RenderOpen();

            // Create a rectangle and draw it in the DrawingContext.
            Rect rect = new Rect(new Point(160, 100), new Size(320, 80));
            drawingContext.DrawRectangle(System.Windows.Media.Brushes.LightGreen, (System.Windows.Media.Pen)null, rect);

            // Persist the drawing content.
            drawingContext.Close();

            return drawingVisual;
        }

        

        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Transparent,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                Brushes.Transparent,
                null,
                new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                Brushes.Transparent,
                null,
                new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                Brushes.Transparent,
                null,
                new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }

        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            Image.Source = this.imageSource;

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                MessageBox.Show("Kinect Sensor is not Powered");
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();

                /** leftHandX.Sort();
                 leftHandY.Sort();
                 rightHandX.Sort();
                 rightHandX.Sort();
                 Console.WriteLine("Left Hand max X: " + leftHandX[leftHandX.Count - 1] + ", Left Hand min X: " + leftHandX[0] + ", Right Hand max X: " + rightHandX[rightHandX.Count - 1] + ", Right Hand min X: " + rightHandX[0]);
                 Console.WriteLine("Left Hand max Y: " + leftHandY[leftHandY.Count - 1] + ", Left Hand min Y " + leftHandY[0] + ", Right Hand max Y: " + rightHandY[rightHandY.Count - 1] + ", Right Hand min Y: " + rightHandY[0]);        **/
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {

            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            using (DrawingContext dc = this.drawingGroup.Open())
            {

                Rect rec = new Rect(50.0, 50.0, 50.0, 50.0);
                Joint joint1 = new Joint();
                Joint joint2 = new Joint();
                Joint jointreg = new Joint();

                // Create a timer with a two second interval.
                Timer aTimer = new Timer(1000);

                // Hook up the Elapsed event for the timer. 
                //aTimer.Elapsed += OnTimedEvent;

                DateTime handsclosed = default(DateTime);
                // Draw a transparent background to set the render size
                //dc.DrawImage(imageSource, rec);
                //dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));  // only to say the background has to be dark but actually it's not our case

                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            this.DrawBonesAndJoints(skel, dc);  // draw the skeleton
                            foreach (Joint joint in skel.Joints)
                            {
                                if (joint.JointType.Equals(JointType.HandLeft) || joint.JointType.Equals(JointType.HandRight))
                                {
                                    if (joint.JointType.Equals(JointType.HandLeft))
                                    {
                                        joint1 = joint;
                                    }
                                    else if (joint.JointType.Equals(JointType.HandRight))
                                    {
                                        joint2 = joint;
                                    }
                                    //@marion94 CompareTo returns an integer value and not float.
                                    if (joint2.Position.X.CompareTo(joint1.Position.X) < 0.1f)
                                    {
                                        if (zoomout > 20 && !isZoomedOut)
                                        {
                                            zoomoutMap();
                                            isZoomedOut = true;//dc.DrawRectangle(Brushes.Fuchsia, null, rec);
                                        } //handsclosed = DateTime.Now;
                                       
                                        zoomin = zoomin + 1;
                                        //dc.DrawRectangle(Brushes.Green, null, rec);
                                    }
                                    if (joint2.Position.X > 0.3f && joint1.Position.X < -0.3f)
                                    {
                                        //dc.DrawRectangle(Brushes.Green, null, rec);
                                        if (zoomin > 20 && !isZoomedIn)
                                        {
                                            zoominMap();
                                            isZoomedIn = true;//dc.DrawRectangle(Brushes.Green, null, rec);  // zoom in 
                                        }
                                        else zoomout = zoomout + 1;
                                       
                                    }


                                }
                                //if ((joint2.Position.X+0.65f ) < joint1.Position.X && (joint2.Position.Z<(joint1.Position.Z+0.1) || joint2.Position.Z > (joint1.Position.Z + 0.1)))
                                //{
                                //dc.DrawRectangle(Brushes.Brown, null, rec);
                                //}
                                time = time + 1;

                                if (time == 1000) // roughly 770ms
                                {
                                    time = 0;

                                    //if (incr > 80)
                                    {
                                        //MessageBox.Show("increment = "+incr);
                                        //dc.DrawRectangle(Brushes.Brown, null, rec);
                                    }
                                    zoomin = 0;
                                    zoomout = 0;
                                    isZoomedIn = false;
                                    isZoomedOut = false;
                                }
                            }
                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly) // if the skeleton is seated
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            this.SkeletonPointToScreen(skel.Position),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }

                        foreach (Joint joint in skel.Joints)
                        {
                            if (joint.JointType == JointType.HandLeft || joint.JointType == JointType.HandRight)
                            {
                                if (joint.JointType == JointType.HandLeft)
                                    joint1 = joint;
                            }

                            else { joint2 = joint; }

                            //if ((joint2.Position.X+0.65f ) < joint1.Position.X && (joint2.Position.Z<(joint1.Position.Z+0.1) || joint2.Position.Z > (joint1.Position.Z + 0.1)))
                            //{
                            //dc.DrawRectangle(Brushes.Brown, null, rec);
                            //}

                            if ((joint2.Position.X) < joint1.Position.X && (joint2.Position.X + 0.01f) > joint1.Position.X)
                            {
                                handsclosed = DateTime.Now;
                              
                            }
                            if (DateTime.Now == handsclosed.AddSeconds(1))
                                dc.DrawRectangle(Brushes.Brown, null, rec);
                        }
                    }
                }



                // prevent drawing outside of our render area
                //this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }


        /// <summary>
        /// Draws a skeleton's bones and joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {
            // Render Torso
            this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

            // Left Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

            // Right Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

            // Left Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
            this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

            // Right Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
            this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
            this.DrawBone(skeleton, drawingContext, JointType.HandRight, JointType.HandLeft);

            // Render Joints
            foreach (Joint joint in skeleton.Joints)
            if  (joint.JointType==JointType.HandLeft || joint.JointType== JointType.HandRight)
            {
                Brush drawBrush = null;

                    
                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;                   
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;                 
                }

                    if (drawBrush != null)
                    {
                        drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);  // draw everything : needed !
                        //zoomInZoomOutMap(skeleton.Joints[JointType.HandLeft], skeleton.Joints[JointType.HandRight]);
                        //Console.WriteLine(refRightHand.Position.X);
                       /** if (joint.JointType == JointType.HandLeft)
                        {
                            leftHandX.Add(joint.Position.X);
                           // leftHandY.Add(joint.Position.Y);
                        }
                        else if (joint.JointType == JointType.HandRight)
                        {
                            rightHandX.Add(joint.Position.X);
                            rightHandY.Add(joint.Position.Y);
                        }**/
                    }
                    
                    /** System.Console.WriteLine(joint.JointType);
                     System.Console.WriteLine(joint.Position.X + "  : " + myMap.ZoomLevel);**/
                    
            }

        }

        

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }

            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }

        /// <summary>
        /// Handles the checking or unchecking of the seated mode combo box
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void CheckBoxSeatedModeChanged(object sender, RoutedEventArgs e)
         {
            if (null != this.sensor)
            {
                if (this.checkBoxSeatedMode.IsChecked.GetValueOrDefault())
                {
                    this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                }
                else
                {
                    this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
               }
            }
        }
    }
}