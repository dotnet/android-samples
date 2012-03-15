using System;
using Android.App;
using Android.OS;
using Android.Widget;

namespace StandardControls {

    [Activity(Label = "AutoCompleteTextView")]
    public class AutoCompleteTextViewScreen : Activity {
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.AutoCompleteTextView);

            AutoCompleteTextView act = FindViewById < AutoCompleteTextView>(Resource.Id.AutoCompleteInput);
            ArrayAdapter arr = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line
                , new String[] { "Hello", "Hi", "Hola", "Bonjour", "Gday", "Goodbye", "Sayonara", "Farewell", "Adios"});
            act.Adapter = arr;
        }
    }
}

