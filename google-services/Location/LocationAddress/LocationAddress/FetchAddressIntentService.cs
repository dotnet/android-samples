using Android.App;
using Android.OS;
using Android.Util;
using Android.Locations;
using System.Collections.Generic;
using System.Linq;
using Java.IO;
using Java.Lang;

namespace LocationAddress
{
    [Service(Exported = false)]
    public class FetchAddressIntentService : IntentService
    {
        const string TAG = "FetchAddressIS";

        protected ResultReceiver mReceiver;

        public FetchAddressIntentService()
            : base(TAG)
        {
        }

        protected override void OnHandleIntent(Android.Content.Intent intent)
        {
            var errorMessage = string.Empty;

            mReceiver = intent.GetParcelableExtra(Constants.Receiver) as ResultReceiver;

            if (mReceiver == null)
            {
                Log.Wtf(TAG, "No receiver received. There is nowhere to send the results.");
                return;
            }

            var location = (Location)intent.GetParcelableExtra(Constants.LocationDataExtra);

            if (location == null)
            {
                errorMessage = GetString(Resource.String.no_location_data_provided);
                Log.Wtf(TAG, errorMessage);
                DeliverResultToReceiver(Result.FirstUser, errorMessage);
                return;
            }

            var geocoder = new Geocoder(this, Java.Util.Locale.Default);

            List<Address> addresses = null;

            try
            {
                addresses = new List<Address>(geocoder.GetFromLocation(location.Latitude, location.Longitude, 1));
            }
            catch (IOException ioException)
            {
                errorMessage = GetString(Resource.String.service_not_available);
                Log.Error(TAG, errorMessage, ioException);
            }
            catch (IllegalArgumentException illegalArgumentException)
            {
                errorMessage = GetString(Resource.String.invalid_lat_long_used);
                Log.Error(TAG, string.Format("{0}. Latitude = {1}, Longitude = {2}", errorMessage,
                    location.Latitude, location.Longitude), illegalArgumentException);
            }

            if (addresses == null || addresses.Count == 0)
            {
                if (string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = GetString(Resource.String.no_address_found);
                    Log.Error(TAG, errorMessage);
                }
                DeliverResultToReceiver(Result.FirstUser, errorMessage);
            }
            else
            {
                Address address = addresses.FirstOrDefault();
                var addressFragments = new List<string>();

                for (int i = 0; i <= address.MaxAddressLineIndex; i++)
                {
                    addressFragments.Add(address.GetAddressLine(i));
                }
                Log.Info(TAG, GetString(Resource.String.address_found));
                DeliverResultToReceiver(Result.Canceled, string.Join("\n", addressFragments));
            }
        }

        void DeliverResultToReceiver(Result resultCode, string message)
        {
            var bundle = new Bundle();
            bundle.PutString(Constants.ResultDataKey, message);
            mReceiver.Send(resultCode, bundle);
        }
    }
}

