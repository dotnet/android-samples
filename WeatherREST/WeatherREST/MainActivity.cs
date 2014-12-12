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
using System.Threading.Tasks;

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
            Button button = FindViewById<Button>(Resource.Id.getWeatherButton);

            // When the user clicks the button, send the REST request to geonames.org,
            button.Click += async (sender, e) => {

                // Get the latitude and longitude entered by the user and create a query. 
                // Note that input error checking is ignored here.

                string url = "http://api.geonames.org/findNearByWeatherJSON?lat=" +
                             latitude.Text +
                             "&lng=" +
                             longitude.Text +
                             "&username=demo";

                // Fetch the weather information asynchronously, then update the screen:
                await FetchAndDisplayAsync(url);
            };
        }

        // Gets weather data from the passed URL, parses the results, then writes 
        // temperature, humidity, conditions, and location to the screen.

        private async Task FetchAndDisplayAsync(string url)
        {
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response:
            using (WebResponse response = await request.GetResponseAsync())
            {
                // Get a stream representation of the HTTP web response:
                using (Stream stream = response.GetResponseStream())
                {
                    // Use this stream to build a JSON object:
                    JsonValue jsonDoc = await Task.Run (() => JsonObject.Load(stream));
                    Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());

                    // Extract the array of name/value results for the field name "weatherObservation":
                    // Note that there is no exception handling for when this field is not found.
                    JsonValue weatherResults = jsonDoc["weatherObservation"];

                    //.......................................................................
                    // The remainder of this example formats the resulting weather
                    // information to the lower half of the screen.

                    // Get the weather reporting fields from the layout resource: 
                    TextView location = FindViewById<TextView>(Resource.Id.locationText);
                    TextView temperature = FindViewById<TextView>(Resource.Id.tempText);
                    TextView humidity = FindViewById<TextView>(Resource.Id.humidText);
                    TextView conditions = FindViewById<TextView>(Resource.Id.condText);

                    // Extract the "stationName" (location string) and write it to the location TextBox:
                    location.Text = weatherResults["stationName"];

                    // The temperature is expressed in Celsius:
                    double temp = weatherResults["temperature"];
                    // Convert to Fahrenheit:
                    temp = ((9.0 / 5.0) * temp) + 32;
                    // Write the temperature (one decimal place) to the temperature TextBox:
                    temperature.Text = String.Format("{0:F1}", temp) + "° F";

                    // Get the percent humidity and write it to the humidity TextBox:
                    double humidPercent = weatherResults["humidity"];
                    humidity.Text = humidPercent.ToString() + "%";

                    // Get the "clouds" and "weatherConditions" strings and combine them.
                    // Ignore strings that are reported as "n/a":
                    string cloudy = weatherResults["clouds"];
                    if (cloudy.Equals("n/a"))
                        cloudy = "";
                    string cond = weatherResults["weatherCondition"];
                    if (cond.Equals("n/a"))
                        cond = "";

                    // Write the result to the conditions TextBox:
                    conditions.Text = cloudy + " " + cond;
                }
            }
        }
    }
}
