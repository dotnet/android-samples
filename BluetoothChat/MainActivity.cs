/*
* Copyright 2013 The Android Open Source Project
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using Android.App;
using Android.OS;
using Android.Support.V7.App;

namespace com.xamarin.samples.bluetooth.bluetoothchat
{
    /// <summary>
    /// This is the main Activity that displays the current chat session.
    /// </summary>
    [Activity(Label = "@string/app_name", MainLauncher = true,
               ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation)]
    public class MainActivity : AppCompatActivity
    {
        new const string TAG = "BluetoothChat.MainActivity";
        bool logShown;
        BluetoothChatFragment chatFrag; 
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            if (savedInstanceState == null)
            {
                var tx = FragmentManager.BeginTransaction();
                chatFrag = new BluetoothChatFragment();
                tx.Replace(Resource.Id.sample_content_fragment, chatFrag);
                tx.Commit();

            }
        }

        public override bool OnCreateOptionsMenu(Android.Views.IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main, menu);
            return true;
        }
    }
}


