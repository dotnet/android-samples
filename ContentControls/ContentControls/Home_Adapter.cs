using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Content;

namespace ContentControls {
    public class Sample {
        public Sample(string name, Type screen)
        {
            Name = name;
            Screen = screen;
        }
        public string Name;
        public Type Screen;
    }
    public class Home_Adapter : BaseAdapter<Sample> {
        Activity context = null;
        Dictionary<string, List<Sample>> samples;
        private readonly IList<object> rows;

        public Home_Adapter(Activity context)
            : base()
        {
            this.context = context;
            samples = new Dictionary<string, List<Sample>>();

            samples.Add("Browsers", new List<Sample>() { 
                   new Sample ("WebView", typeof(WebViewScreen))
                ,  new Sample ("WebView Browser", typeof(WebViewBrowserScreen))
                ,  new Sample ("Local Content", typeof(WebViewLocalContentScreen))
                ,  new Sample ("Generated Content", typeof(WebViewGeneratedContentScreen))
                ,  new Sample ("Javascript interop", typeof(WebViewInteropScreen))
            });
            samples.Add("Maps", new List<Sample>() {
                   new Sample ("Basic MapView", typeof(MapViewScreen))
                ,  new Sample ("MapView with Annotation", typeof(MapViewAnnotationScreen))
//                ,  new Sample ("MapView with Current Location", typeof(MapViewCurrentLocationScreen))   // can't test
                ,  new Sample ("MapView with Overlay", typeof(MapViewOverlayScreen))

            });
            samples.Add("Search", new List<Sample>() {
                   new Sample ("AutoCompleteTextView", typeof(AutoCompleteTextViewScreen))
            });
            samples.Add("Nav", new List<Sample>() {
                   new Sample ("Activity Fade", typeof(ActivityFadeScreen))
                ,  new Sample ("Activity Zoom", typeof(ActivityZoomScreen))
                ,  new Sample ("Theme", typeof(LightThemeScreen))
            });
            

            // flatten groups into single 'list'
            rows = new List<object>();
            foreach (var section in samples.Keys) {
                rows.Add(section);
                foreach (var session in samples[section]) {
                    rows.Add(session);
                }
            }
        }
        public Sample GetSample (int position) {
            return (Sample)rows[position];
        }
        public override Sample this[int position]
        {
            get
            { // this'll break if called with a 'header' position
                return (Sample)rows[position];
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {
            get { return rows.Count; }
        }
        public override bool AreAllItemsEnabled()
        {
            return true;
        }
        public override bool IsEnabled(int position)
        {
            return !(rows[position] is string);
        }

        /// <summary>
        /// Grouped list: view could be a 'section heading' or a 'data row'
        /// </summary>
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            // Get our object for this position
            var item = this.rows[position];
            View view = null;

            if (item is string) {   // header
                view = context.LayoutInflater.Inflate(Resource.Layout.HomeSectionHeader, null);
                view.Clickable = false;
                view.LongClickable = false;
                view.SetOnClickListener(null);
                view.FindViewById<TextView>(Resource.Id.Text1).Text = (string)item;
            } else {   //session
                view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);

                view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = ((Sample)item).Name;
            }
            //Finally return the view
            return view;
        }
    }
}
