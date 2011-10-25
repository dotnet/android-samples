/*
 * Copyright (C) 2007 The Android Open Source Project
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
using Android.Content;
using Android.Text;
using Android.Text.Method;
using Android.Util;
using Android.Widget;

namespace MonoDroid.ApiDemo
{
	public class LogTextBox : TextView
	{
		public LogTextBox (IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
			: base (handle, transfer)
		{
		}

		public LogTextBox (Context context) : this (context, null)
		{
		}

		public LogTextBox (Context context, IAttributeSet attrs) : this (context, attrs, Android.Resource.Attribute.TextViewStyle)
		{
		}

		public LogTextBox (Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle)
		{
		}

		protected override Android.Text.Method.IMovementMethod DefaultMovementMethod {
			get { return ScrollingMovementMethod.Instance; }
		}

		// TODO: Crashes due to bug #640194 
		//public override Java.Lang.CharSequence Text {
		//        get { return base.Text; }
		//        set { base.SetText (Text, BufferType.Editable); }
		//}
	}
}