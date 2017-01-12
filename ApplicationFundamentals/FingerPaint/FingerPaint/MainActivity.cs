using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace FingerPaint
{
    [Activity(Label = "FingerPaint", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        FingerPaintCanvasView fingerPaintCanvasView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set the view from the Main.axml layout resource
            SetContentView(Resource.Layout.Main);

            // Get a reference to the FingerPaintCanvasView from the Main.axml file
            fingerPaintCanvasView = FindViewById<FingerPaintCanvasView>(Resource.Id.canvasView);

            // Set up the Spinner to select color
            Spinner colorSpinner = FindViewById<Spinner>(Resource.Id.colorSpinner);
            colorSpinner.ItemSelected += OnColorSpinnerItemSelected;

            var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.colors_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            colorSpinner.Adapter = adapter;

            // Set up the Spinner to select line width

            Spinner widthSpinner = FindViewById<Spinner>(Resource.Id.widthSpinner);
            //    spinner.ItemSelected

            var widthsAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.widths_array, Android.Resource.Layout.SimpleSpinnerItem);
            widthsAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            widthSpinner.Adapter = widthsAdapter;


            // Get the Clear button
            Button clearButton = FindViewById<Button>(Resource.Id.clearButton);
            clearButton.Click += OnClearButtonClick;

        }


        private void OnColorSpinnerItemSelected(object sender, AdapterView.ItemSelectedEventArgs args)
        {
            Spinner spinner = (Spinner)sender;

            var x = spinner.GetItemAtPosition(args.Position);
        }

        private void OnClearButtonClick(object sender, EventArgs args)
        {

        }

    }
}

