/*
 * Copyright (C) 2013 The Android Open Source Project
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BluetoothLeGatt
{
	/**
 	* This class includes a small subset of standard GATT attributes for demonstration purposes.
 	*/
	public class SampleGattAttributes
	{
		public static String HEART_RATE_MEASUREMENT = "00002a37-0000-1000-8000-00805f9b34fb";
		public static String CLIENT_CHARACTERISTIC_CONFIG = "00002902-0000-1000-8000-00805f9b34fb";

		private static Dictionary <String, String> Attributes = new Dictionary <String, String> ()
		{
			// Sample Services.
			{"0000180d-0000-1000-8000-00805f9b34fb", "Heart Rate Service"},
			{"0000180a-0000-1000-8000-00805f9b34fb", "Device Information Service"},

			// Sample Characteristics.
			{HEART_RATE_MEASUREMENT, "Heart Rate Measurement"},
			{"00002a29-0000-1000-8000-00805f9b34fb", "Manufacturer Name String"},
		};

		public static String Lookup (String key, String defaultName) 
		{
			String name = Attributes [key];
			return name == null ? defaultName : name;
		}
	}
}

