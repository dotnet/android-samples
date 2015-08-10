/*
 * Copyright (C) 2014 The Android Open Source Project
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
using Android.OS;

namespace PermissionRequest
{
	/// <summary>
	/// Prompts the user to confirm permission request.
	/// </summary>
	public class ConfirmationDialogFragment : DialogFragment
	{
		const string ARG_RESOURCES = "resources";

		/// <summary>
		/// Creates a new instance of ConfirmationDialogFragment.
		/// </summary>
		/// <returns>A new instance.</returns>
		/// <param name="resources">The list of resources requested by PermissionRequeste.</param>
		public static ConfirmationDialogFragment NewInstance (String[] resources)
		{
			var fragment = new ConfirmationDialogFragment ();
			var args = new Bundle ();
			args.PutStringArray (ARG_RESOURCES, resources);
			fragment.Arguments = args;
			return fragment;
		}

		public override Dialog OnCreateDialog (Bundle savedInstanceState)
		{
			String[] resources = Arguments.GetStringArray (ARG_RESOURCES);

			var builder = new AlertDialog.Builder (Activity);
			builder.SetMessage (GetString (Resource.String.confirmation, string.Join ("\n", resources)));
			builder.SetNegativeButton (Resource.String.deny, delegate {
				((Listener)ParentFragment).OnConfirmation (false);
			});
			builder.SetPositiveButton (Resource.String.allow, delegate {
				((Listener)ParentFragment).OnConfirmation (true);
			});
			var dialog = builder.Create ();

			return dialog;
		}
	}

	/// <summary>
	/// Callback for the user's response.
	/// </summary>
	public interface Listener
	{
		/// <summary>
		/// Called when the PermissionRequest is allowed or denied by the user.
		/// </summary>
		/// <param name="allowed">If set to <c>true</c>, the user allowed the request.</param>
		void OnConfirmation (bool allowed);
	}
}

