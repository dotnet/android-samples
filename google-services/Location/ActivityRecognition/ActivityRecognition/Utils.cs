using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Gms.Location;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ActivityRecognition
{
    public class Utils
    {
        private Utils() { }

        public static string GetActivityString(Context context, int detectedActivityType)
        {
            var resources = context.Resources;
            switch (detectedActivityType)
            {
                case DetectedActivity.InVehicle:
                    return resources.GetString(Resource.String.in_vehicle);
                case DetectedActivity.OnBicycle:
                    return resources.GetString(Resource.String.on_bicycle);
                case DetectedActivity.OnFoot:
                    return resources.GetString(Resource.String.on_foot);
                case DetectedActivity.Running:
                    return resources.GetString(Resource.String.running);
                case DetectedActivity.Still:
                    return resources.GetString(Resource.String.still);
                case DetectedActivity.Tilting:
                    return resources.GetString(Resource.String.tilting);
                case DetectedActivity.Unknown:
                    return resources.GetString(Resource.String.unknown);
                case DetectedActivity.Walking:
                    return resources.GetString(Resource.String.walking);
                default:
                    return resources.GetString(Resource.String.unidentifiable_activity, new[] { new Java.Lang.Integer(detectedActivityType) });
            }
        }

        public static String DetectedActivitiesToJson(IList<DetectedActivity> detectedActivitiesList)
        {
            var covertedList = detectedActivitiesList.Select(s => new { Type = s.Type, Confidence = s.Confidence }).ToList();
            return JsonConvert.SerializeObject(covertedList);
        }

        public static List<DetectedActivity> DetectedActivitiesFromJson(String jsonArray)
        {
            var result = new List<DetectedActivity>();
            if (jsonArray == string.Empty) return result;
            foreach (JObject jObject in JArray.Parse(jsonArray).Children<JObject>())
            {
                var dictionary = new Dictionary<string, int>();
                foreach (JProperty p in jObject.Properties())
                {
                    dictionary.Add(p.Name, (int)p.Value);
                }
                result.Add(new DetectedActivity(dictionary["Type"], dictionary["Confidence"]));
            }
            return result;
        }
    }
}