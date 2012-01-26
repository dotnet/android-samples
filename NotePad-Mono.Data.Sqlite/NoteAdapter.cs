/*
 * Copyright (C) 2012 Xamarin Inc.
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

using System;
using Android.App;
using Android.Content;
using Android.Widget;

namespace Mono.Samples.Notepad
{
	class NoteAdapter : ArrayAdapter
	{
		private Activity activity;

		public NoteAdapter (Activity activity, Context context, int textViewResourceId, object[] objects)
			: base (context, textViewResourceId, objects)
		{
			this.activity = activity;
		}

		public override Android.Views.View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			//Get our object for this position
			var item = (Note)this.GetItem (position);

			// Try to reuse convertView if it's not null, otherwise inflate it from our item layout
			// This gives us some performance gains by not always inflating a new view
			var view = (convertView ?? activity.LayoutInflater.Inflate (Resource.Layout.NoteListRow, parent, false)) as LinearLayout;

			view.FindViewById<TextView> (Resource.Id.body).Text = Left (item.Body.Replace ("\n", " "), 25);
			view.FindViewById<TextView> (Resource.Id.modified).Text = item.ModifiedTime.ToString ();

			return view;
		}

		private string Left (string text, int length)
		{
			if (text.Length <= length)
				return text;

			return text.Substring (0, length);
		}
	}
}