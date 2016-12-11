
// This class file contains related to Bing Maps
namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Maps.MapControl.WPF.Design;
    using Microsoft.Kinect;
    using Speech.Recognition;
    using Speech.Synthesis;
    public partial class MainWindow
    {
        SpeechRecognitionEngine _recognizer = new SpeechRecognitionEngine();
        SpeechSynthesizer VAS = new SpeechSynthesizer();
        String[] zoomlevel = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten" };

        int zoominFactor = 2;
        int zoomoutFactor = 2;
        LocationConverter locConv = new LocationConverter();
        // locConv.ConvertFrom("52.520008,13.404954");
        // the distance between hands from kinect is in meters which varies approx. 0 to 1. this needs to be scaled to the zoom level allowed in Bing i.e 1-20.
        private int zoomScalingValue = 19;

        //myMap.SetView((Location)locConv.ConvertFrom("52.520008,13.404954"), 8);
        // scales the increasing or decreasing distance between the hands to zoom in and zoom out respectively
        private void zoomInZoomOutMap(Joint leftHand, Joint rightHand)
        {
            double distance_between_hands = Math.Sqrt(Math.Pow(leftHand.Position.X - rightHand.Position.X, 2) + Math.Pow(leftHand.Position.Y - rightHand.Position.Y, 2));
            // leftHandY.Add(distance_between_hands * 19);

            int distance_in_integer = Convert.ToInt32(distance_between_hands * zoomScalingValue);
            Console.WriteLine("Distance between left and right hand: " + distance_in_integer);// if(distance_between_hands)
            if (distance_in_integer != referenceDistanceBetweenHands)
            {
                myMap.ZoomLevel = distance_between_hands * zoomScalingValue;
            }

        }

        // zooms in the map based on a defined zoom factor
        private void zoominMap()
        {
            myMap.ZoomLevel = myMap.ZoomLevel + zoominFactor;
        }

        // zooms out the map based on a defined zoom factor
        private void zoomoutMap()
        {
            myMap.ZoomLevel = myMap.ZoomLevel - zoomoutFactor;
        }
        private void initialiseSpeechComponent()
        {
            foreach (RecognizerInfo ri in SpeechRecognitionEngine.InstalledRecognizers())
            {
                Console.WriteLine(" TEST: " + ri.Culture.Name);
            }
            //"Show me" activate pointing
            Choices pointActivate = new Choices("Show me");
            //Zooming
            Choices Zooming = new Choices("Hello", "Zoom-in", "Zoom-out");

            //Route Search
            Choices cities = new Choices(new string[] { "Berlin", "Barcelona", "Paris", "London", "Beijing" });
            GrammarBuilder GB_zooming = new GrammarBuilder(Zooming);
            GrammarBuilder GB_route = new GrammarBuilder();
            GrammarBuilder GB_point = new GrammarBuilder();

            GB_zooming.Append("By");
            GB_zooming.Append(new Choices(zoomlevel));

            GB_route.Append("from");
            GB_route.Append(cities);
            GB_route.Append("to");
            GB_route.Append(cities);

            GB_point.Append(pointActivate);
            GB_point.Append(cities);


            Grammar SudeepGrammer = new Grammar(GB_zooming);
            Grammar routeGrammar = new Grammar(GB_route);
            routeGrammar.Name = ("Route Search");
            Grammar pointGrammar = new Grammar(GB_point);
            pointGrammar.Name = ("Show a place");

            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.LoadGrammar(SudeepGrammer);
            _recognizer.LoadGrammar(routeGrammar);
            _recognizer.LoadGrammar(pointGrammar);
            _recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(_recognizer_SpeechRecognized);
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            InitializeComponent();
        }
        void _recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string speech = e.Result.Text;
            String[] splitSpeech = speech.Split(' ');
            
            if (speech.Contains("Zoom-in"))
            {
                zoominFactor = Array.FindIndex(zoomlevel, value=> value.Equals(splitSpeech[2]));
                Console.WriteLine(" Test: " + speech + " : " + zoominFactor);
                zoominMap();
            }
            else if (speech.Contains("Zoom-out"))
            {
                zoomoutFactor = Array.FindIndex(zoomlevel, value => value.Equals(splitSpeech[2]));
                Console.WriteLine(" Test: " + speech + " : " + zoomoutFactor);
                zoomoutMap();
            }
        }
    }
}
