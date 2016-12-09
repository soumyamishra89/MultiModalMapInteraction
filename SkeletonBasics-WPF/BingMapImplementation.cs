
// This class file contains related to Bing Maps
namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Maps.MapControl.WPF.Design;
    using Microsoft.Kinect;
    public partial class MainWindow 
    {

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
            myMap.ZoomLevel = myMap.ZoomLevel - zoominFactor;
        }
    }
}
