using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;

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
    
    class Header {
        public string Name;
        public int SectionIndex;
    }

    public class Home_Adapter : BaseAdapter<Sample> {

        static Dictionary<string, List<Sample>> samples = new Dictionary<string, List<Sample>>() {
            { "Browsers", new List<Sample>() {
                new Sample ("WebView",                          typeof(WebViewScreen)),
                new Sample ("WebView Browser",                  typeof(WebViewBrowserScreen)),
                new Sample ("Local Content",                    typeof(WebViewLocalContentScreen)),
                new Sample ("Generated Content",                typeof(WebViewGeneratedContentScreen)),
                new Sample ("Javascript interop",               typeof(WebViewInteropScreen)),
            } },
            { "Maps", new List<Sample>() {
                new Sample ("Basic MapView",                    typeof(MapViewScreen)),
                new Sample ("MapView with Annotation",          typeof(MapViewAnnotationScreen)),
//                new Sample ("MapView with Current Location", typeof(MapViewCurrentLocationScreen)),   // can't test
                new Sample ("MapView with Overlay",             typeof(MapViewOverlayScreen)),
            } },
            { "Search", new List<Sample>() {
                new Sample ("AutoCompleteTextView",             typeof(AutoCompleteTextViewScreen)),
            } },
            { "Nav", new List<Sample>() {
                new Sample ("Activity Fade",                    typeof(ActivityFadeScreen)),
                new Sample ("Activity Zoom",                    typeof(ActivityZoomScreen)),
                new Sample ("Theme",                            typeof(LightThemeScreen)),
            } },
        };

        const int TypeSectionHeader = 0;
        const int TypeSectionSample = 1;

        readonly Activity context;
        readonly IList<object> rows = new List<object>();

        readonly ArrayAdapter<string> headers;
        readonly Dictionary<string, IAdapter> sections = new Dictionary<string, IAdapter>();

        public Home_Adapter(Activity context)
            : base()
        {
            this.context = context;
            headers = new ArrayAdapter<string>(context, Resource.Layout.HomeSectionHeader, Resource.Id.Text1);
            
            rows = new List<object>();
            foreach (var section in samples.Keys) {
                headers.Add(section);
                sections.Add(section, new ArrayAdapter<Sample>(context, Android.Resource.Layout.SimpleListItem1, samples[section]));
                rows.Add(new Header { Name = section, SectionIndex = sections.Count - 1 });
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

        public override int ViewTypeCount
        {
            get
            {
                return 1 + sections.Values.Sum(adapter => adapter.ViewTypeCount);
            }
        }

        public override int GetItemViewType(int position)
        {
            return rows[position] is Header
                ? TypeSectionHeader
                : TypeSectionSample;
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
            return !(rows[position] is Header);
        }

        /// <summary>
        /// Grouped list: view could be a 'section heading' or a 'data row'
        /// </summary>
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            // Get our object for this position
            var item = this.rows[position];

            View view;

            if (item is Header) {
                view = headers.GetView(((Header)item).SectionIndex, convertView, parent);
                view.Clickable = false;
                view.LongClickable = false;
                return view;
            }

            int i = position - 1;
            while (i > 0 && rows[i] is Sample)
                i--;
            Header h = (Header)rows[i];
            view = sections[h.Name].GetView(position - i - 1, convertView, parent);
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = ((Sample)item).Name;
            return view;
        }
    }
}
