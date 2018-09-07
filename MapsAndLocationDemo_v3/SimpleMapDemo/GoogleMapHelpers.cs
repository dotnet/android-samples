using System;
using System.Collections.Generic;
using System.Linq;

using Android;
using Android.App;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;

namespace SimpleMapDemo
{
    public static class GoogleMapHelpers
    {
        static readonly string TAG = "GoogleMapHelpers";

        static readonly string[] PERMISSIONS_LOCATION =
        {
            Manifest.Permission.AccessFineLocation,
            Manifest.Permission.AccessCoarseLocation
        };

        /// <summary>
        ///     Adds the map fragment to layout and calls GetMapAsync.
        /// </summary>
        /// <remarks>
        ///     This helper will try to add and initialize a <c>MapFragment</c> to a container
        ///     widget on the form. An optional <c>IOnMapReadyCallback</c> object can be provided. If this
        ///     parameter is <c>null</c>, then the method will check to see if the <c>activity</c>
        ///     implements <c>IMapOnReadyCallback</c>. If it does, then the <c>activity</c>
        ///     will be used as the callback. Otherwise, the method will throw an
        ///     <c>ArgumentException</c>.
        /// </remarks>
        /// <returns>The map framgment to layout.</returns>
        /// <param name="activity">Activity that will host the MapFragment. If </param>
        /// <param name="resourceId">The <c>@+id</c> of the container widget that will hold the fragment.</param>
        /// <param name="tag">A tag to assign the fragment.  Defaults to <c>map_frag>.</c></param>
        /// <param name="onMapReadyCallback">
        ///     An <c>IOnMapReadyCallback</c> object. If this is null, then the helper will check to
        ///     see if the <c>activity</c> implements the interface and use that.
        /// </param>
        public static MapFragment AddMapFragmentToLayout(this AppCompatActivity activity, int resourceId, string tag = "map_frag",
                                                         IOnMapReadyCallback onMapReadyCallback = null)
        {
            var options = new GoogleMapOptions();
            options.InvokeMapType(GoogleMap.MapTypeHybrid)
                   .InvokeCompassEnabled(true);

            var mapFrag = MapFragment.NewInstance(options);

            activity.FragmentManager.BeginTransaction()
                    .Add(resourceId, mapFrag, tag)
                    .Commit();

            if (onMapReadyCallback == null)
            {
                if (activity is IOnMapReadyCallback callback)
                {
                    mapFrag.GetMapAsync(callback);
                }
                else
                {
                    throw new
                        ArgumentException("If the onMapReadyCallback is null, then the activity must implement the interface IOnMapReadyCallback.",
                                          nameof(activity));
                }
            }
            else
            {
                mapFrag.GetMapAsync(onMapReadyCallback);
            }

            return mapFrag;
        }

        /// <summary>
        ///     This method will check to see if the activity has permission to
        ///     access the device location. If it does not, then it will display the
        ///     permission rationale (if necessary) and then request permission. It
        ///     is up to the calling activity to handle the results of the
        ///     permisssion request.
        /// </summary>
        /// <param name="activity">Activity.</param>
        /// <param name="requestCode">
        ///     A request code that will be returned
        ///     to the activity via the <c>OnRequestPermissionsResult</c> method.
        /// </param>
        public static bool PerformRuntimePermissionCheckForLocation(this AppCompatActivity activity, int requestCode)
        {
            if (activity.HasLocationPermissions())
            {
                return true;
            }

            if (activity.MustShowPermissionRationale())
            {
                var layoutForSnackbar = activity.FindViewById(Android.Resource.Id.Content);

                var requestPermissionAction = new Action<View>(delegate
                                                               {
                                                                   ActivityCompat.RequestPermissions(activity, PERMISSIONS_LOCATION, requestCode);
                                                               });

                Snackbar.Make(layoutForSnackbar, Resource.String.location_permission_rationale, Snackbar.LengthIndefinite)
                        .SetAction(Resource.String.ok, requestPermissionAction);
            }
            else
            {
                ActivityCompat.RequestPermissions(activity, PERMISSIONS_LOCATION, requestCode);
            }

            return false;
        }

        /// <summary>
        ///     Performs a check to see if the app should display the permissions
        ///     rationale dialog to the user before requesting permissions.
        /// </summary>
        /// <returns>
        ///     <c>true</c>, it is necessary to show the permissions rationale
        ///     dialog, <c>false</c> otherwise.
        /// </returns>
        /// <param name="activity">Activity.</param>
        public static bool MustShowPermissionRationale(this Activity activity)
        {
            var showShowRationale = false;

            foreach (var perm in PERMISSIONS_LOCATION)
            {
                if (ActivityCompat.ShouldShowRequestPermissionRationale(activity, perm))
                {
                    showShowRationale = true;
                    Log.Debug(TAG, $"Need to show permission rational for {perm}.");
                }
            }

            return showShowRationale;
        }

        /// <summary>
        ///     Performs a check to see if the activity has been granted the
        ///     <c>ACCESS_FINE_LOCATION</c> and <c>ACCESS_COARSE_LOCATION</c>
        ///     permissions.
        /// </summary>
        /// <returns><c>true</c>, if location permissions was granted, <c>false</c> otherwise.</returns>
        /// <param name="activity">Activity.</param>
        public static bool HasLocationPermissions(this Activity activity)
        {
            var hasPermissions = true;
            foreach (var p in PERMISSIONS_LOCATION)
            {
                if (ContextCompat.CheckSelfPermission(activity, p) != (int) Permission.Granted)
                {
                    Log.Warn(TAG, $"App was not granted the {p} permission.");
                    hasPermissions = false;
                }
            }

            return hasPermissions;
        }

        /// <summary>
        /// Does a quick check to make sure that all of the <c>grantResults</c>
        /// are <c>Permission.Granted</c>.
        /// </summary>
        /// <param name="grantResults"></param>
        /// <returns></returns>
        public static bool AllPermissionsGranted(this IEnumerable<Permission> grantResults)
        {
            return grantResults.All(result => result != Permission.Denied);
        }
    }
}
