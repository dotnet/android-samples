using System;
using System.Collections.Generic;
using System.IO;
using Android.App;
using Android.OS;
using Android.Widget;

namespace ContentControls {
	/// <summary>
	/// Demonstrates writing an Adapter to support custom filtering in the AutoComplete list
	/// </summary>
    [Activity(Label = "AutoCompleteCustomAdapter")]
    public class AutoCompleteCustomAdapterScreen : Activity {
       
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.AutoCompleteTextView);

            AutoCompleteTextView act = FindViewById<AutoCompleteTextView>(Resource.Id.AutoCompleteInput);

            Stream seedDataStream = Assets.Open(@"WordList.txt");
          
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(seedDataStream)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    lines.Add(line);
                }
            }

            string[] wordlist = lines.ToArray();

            act.Adapter = new AutoCompleteCustomAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, wordlist);
        }
    }
}