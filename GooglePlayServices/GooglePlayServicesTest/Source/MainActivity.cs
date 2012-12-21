/*
 * Copyright (C) 2012 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// C# Port by Atsushi Eno
// Copyright (C) 2012 Xamarin, Inc. (Apache License, Version 2.0 too)

using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace GooglePlayServicesTest
{
	[Activity (Label = "Mono GoogleMapsV2 Demo", MainLauncher = true)]
	public class MainActivity : ListActivity
	{
		class DemoDetails : Java.Lang.Object
		{
			/**
         * The resource id of the title of the demo.
         */
			internal readonly int titleId;
			
			/**
         * The resources id of the description of the demo.
         */
			internal readonly int descriptionId;
			
			/**
         * The demo activity's class.
         */
			internal readonly Type activityClass;
			
			public DemoDetails (int titleId, int descriptionId, Type activityClass) 
			{
				this.titleId = titleId;
				this.descriptionId = descriptionId;
				this.activityClass = activityClass;
			}
		}

		class CustomArrayAdapter : ArrayAdapter<DemoDetails> 
		{
			/**
         * @param demos An array containing the details of the demos to be displayed.
         */
			public CustomArrayAdapter (Context context, DemoDetails[] demos) 
				: base (context, Resource.Layout.feature, Resource.Id.title, demos)
			{
			}
			
			public override View GetView (int position, View convertView, ViewGroup parent) 
			{
				FeatureView featureView;
				if (convertView is FeatureView) {
					featureView = (FeatureView) convertView;
				} else {
					featureView = new FeatureView (Context);
				}
				
				DemoDetails demo = GetItem (position);
				
				featureView.SetTitleId (demo.titleId);
				featureView.SetDescriptionId (demo.descriptionId);
				
				return featureView;
			}
		}
		private static DemoDetails [] demos = new DemoDetails [] {
			new DemoDetails(
				Resource.String.basic_map, Resource.String.basic_description,
				typeof (BasicMapActivity)),
			/*
			new DemoDetails(Resource.String.camera_demo, Resource.String.camera_description,
			                typeof (CameraDemoActivity)),
			new DemoDetails(Resource.String.events_demo, Resource.String.events_description,
			                typeof (EventsDemoActivity)),
			new DemoDetails(Resource.String.layers_demo, Resource.String.layers_description,
			                typeof (LayersDemoActivity)),
			new DemoDetails(
				Resource.String.locationsource_demo, Resource.String.locationsource_description,
				typeof (LocationSourceDemoActivity)),
			new DemoDetails(Resource.String.uisettings_demo, Resource.String.uisettings_description,
			                typeof (UiSettingsDemoActivity)),
			new DemoDetails(Resource.String.groundoverlay_demo, Resource.String.groundoverlay_description,
			                typeof (GroundOverlayDemoActivity)),
			                */
			new DemoDetails(Resource.String.marker_demo, Resource.String.marker_description,
			                typeof (MarkerDemoActivity)),
			/*
			new DemoDetails(Resource.String.polygon_demo, Resource.String.polygon_description,
			                typeof (PolygonDemoActivity)),
			new DemoDetails(Resource.String.polyline_demo, Resource.String.polyline_description,
			                typeof (PolylineDemoActivity)),
			new DemoDetails(Resource.String.tile_overlay_demo, Resource.String.tile_overlay_description,
			                typeof (TileOverlayDemoActivity)),
			new DemoDetails(Resource.String.options_demo, Resource.String.options_description,
			                typeof (OptionsDemoActivity)),
			new DemoDetails(Resource.String.multi_map_demo, Resource.String.multi_map_description,
			                typeof (MultiMapDemoActivity)),
			new DemoDetails(Resource.String.retain_map, Resource.String.retain_map_description,
			                typeof (RetainMapActivity)),
			new DemoDetails(Resource.String.raw_mapview_demo, Resource.String.raw_mapview_description,
			                typeof (RawMapViewDemoActivity)),
			new DemoDetails(Resource.String.programmatic_demo, Resource.String.programmatic_description,
			                typeof (ProgrammaticDemoActivity))
			*/
		};
		
		protected override void OnCreate (Bundle savedInstanceState) 
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.main);
			IListAdapter adapter = new CustomArrayAdapter (this, demos);
			
			ListAdapter = adapter;
		}
		
		protected override void OnListItemClick (ListView l, View v, int position, long id) 
		{
			DemoDetails demo = (DemoDetails) ListAdapter.GetItem (position);
			StartActivity (new Intent (this, demo.activityClass));
		}
	}
}


