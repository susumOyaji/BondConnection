using System;

using Xamarin.Forms;
using BondConnection.iOS;
using CoreLocation;



[assembly: Dependency(typeof(GeoLocator_iOS))]
namespace BondConnection.iOS
{
    public class GeoLocator_iOS : IGeolocator
    {
        public event LocationEventHandler LocationReceived;

        private readonly CLLocationManager _locationMan = new CLLocationManager();

        public void StartGps()
        {
            _locationMan.LocationsUpdated += (sender, e) =>
            {
                if (this.LocationReceived != null)
                {
                    var l = e.Locations[e.Locations.Length - 1];

                    this.LocationReceived(this, new LocationEventArgs
                    {
                        Latitude = l.Coordinate.Latitude,
                        Longitude = l.Coordinate.Longitude
                    });
                }
            };

            _locationMan.StartUpdatingLocation();
        }
    }
}

