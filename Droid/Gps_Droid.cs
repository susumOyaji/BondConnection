
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Xamarin.Forms;
using BondConnection.Droid;

using Android.Locations;




[assembly: Dependency(typeof(GeoLocator_Android))]

namespace BondConnection.Droid
{
    public class GeoLocator_Android : IGeolocator
    {
        public event LocationEventHandler LocationReceived;

        public void StartGps()
        {
            var context = Forms.Context;
            var locationMan = context.GetSystemService(Context.LocationService)
              as LocationManager;

            locationMan.RequestLocationUpdates(LocationManager.GpsProvider, 0, 0,
              new MyLocationListener(l =>
              {
                  if (this.LocationReceived != null)
                  {
                      this.LocationReceived(this, new LocationEventArgs
                      {
                          Latitude = l.Latitude,
                          Longitude = l.Longitude
                      });
                  }
              }));
        }



        ////Getting the IP Address of the device fro Android.
        public string getIPAddress()
        {
            string ipaddress = "";

            IPHostEntry ipentry = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in ipentry.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ipaddress = ip.ToString();
                    break;
                }
            }
            return ipaddress;
        }



    }








    class MyLocationListener : Java.Lang.Object, ILocationListener
    {
                private readonly Action<Location> _onLocationChanged;

                public MyLocationListener(Action<Location> onLocationChanged)
                {
                    _onLocationChanged = onLocationChanged;
                }

                public void OnLocationChanged(Location location)
                {
                    _onLocationChanged(location);
                }

                public void OnProviderDisabled(string provider) { }
                public void OnProviderEnabled(string provider) { }
                public void OnStatusChanged(string provider,
                  Availability status, Bundle extras)
                { }
   }

}
       


