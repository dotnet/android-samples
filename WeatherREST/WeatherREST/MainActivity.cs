using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net;
using System.IO;
using System.Json;

namespace WeatherREST
{
    [Activity(Label = "WeatherREST", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get each input EditBox (for entering longitude and latitude) and
            // the button from the layout resource:

            EditText latitude = FindViewById<EditText>(Resource.Id.latText);
            EditText longitude = FindViewById<EditText>(Resource.Id.longText);
            Button button = FindViewById<Button>(Resource.Id.GetWeatherButton);

            // Get the weather reporting fields from the layout resource: 
            TextView location = FindViewById<TextView>(Resource.Id.locationText);
            TextView temperature = FindViewById<TextView>(Resource.Id.tempText);
            TextView humidity = FindViewById<TextView>(Resource.Id.humidText);
            TextView conditions = FindViewById<TextView>(Resource.Id.condText);

            // When the user clicks the button, use REST to send the query to geonames,
            // the display the result.
            button.Click += delegate
            {
                // Get the user's entered latitude and longitude and create a query. Note
                // that input error checking is ignored here.
                string url = "http://api.geonames.org/findNearByWeatherJSON?lat=" +
                             latitude.Text +
                             "&lng=" +
                             longitude.Text +
                             "&username=demo";

                // Create an HTTP web request using this URL:
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
                request.ContentType = "application/json";
                request.Method = "GET";

                // Wait for a response from the server. Note that, in a production app,
                // you would implement this to be an asychronous wait (so the UI isn't
                // frozen during the wait). To keep the REST example straightforward, 
                // this sample app waits synchronously for the HTTP response before
                // updating the screen.

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // If there is an error, print out why.
                    if (response.StatusCode != HttpStatusCode.OK)
                        Console.Out.WriteLine("Error fetching weather data. Server returned status code: {0}", response.StatusCode);

                    // Get a stream representation of the HTTP web response:
                    using (Stream stream = response.GetResponseStream())
                    {
                        // Use this stream to build a JSON object:
                        JsonObject jsonDoc = (JsonObject)JsonObject.Load(stream);

                        // Extract the value for the field name "weatherObservation":
                        JsonValue results = jsonDoc["weatherObservation"];

                        //.......................................................................
                        // The remainder of this example formats the resulting weather
                        // information to the lower half of the screen.
                        
                        // The resulting value is an array of name/value pairs. First, extract
                        // the "stationName" (location string), and fill the location TextBox:
                        location.Text = results["stationName"];

                        // The temperature is expressed in Celsius:
                        double temp = results["temperature"];
                        // Convert to Fahrenheit:
                        temp = ((9.0 / 5.0) * temp) + 32;

                        // Write the temperature (one decimal place) into the temperature TextBox:
                        temperature.Text = String.Format("{0:F1}", temp) + "° F";

                        // Get the percent humidity and write it to the humidity TextBox:
                        double humidPercent = results["humidity"];
                        humidity.Text = humidPercent.ToString() + "%";

                        // Get the "clouds" and "weatherConditions" strings and combine them.
                        // Ignore strings that are reported as "n/a":
                        string cloudy = results["clouds"];
                        if (cloudy.Equals("n/a"))
                            cloudy = "";

                        string cond = results["weatherCondition"];
                        if (cond.Equals("n/a"))
                            cond = "";

                        // Write the result to the conditions TextBox:
                        conditions.Text = cloudy + " " + cond;
                    }
                }
            };
        }
    }
}
