using System;
using Android.App;
using Android.OS;
using Android.Widget;
using System.IO;
using System.Collections.Generic;

namespace ContentControls {

    [Activity(Label = "AutoCompleteTextView")]
    public class AutoCompleteTextViewScreen : Activity {  // NOTE: subclasses MapActivity (and implement IsRouteDisplayed)
       
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.AutoCompleteTextView);

            AutoCompleteTextView act = FindViewById<AutoCompleteTextView>(Resource.Id.AutoCompleteInput);


            Stream seedDataStream = Assets.Open(@"WordList.txt");
            //StreamReader reader = new StreamReader(seedDataStream);

            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(seedDataStream)) {
                // 3
                // Use while != null pattern for loop
                string line;
                while ((line = reader.ReadLine()) != null) {
                    // 4
                    // Insert logic here.
                    // ...
                    // "line" is a line in the file. Add it to our List.
                    lines.Add(line);
                }
            }

            string[] wordlist = lines.ToArray();


            ArrayAdapter arr = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, wordlist);
            act.Adapter = arr;
        }
    }
}

