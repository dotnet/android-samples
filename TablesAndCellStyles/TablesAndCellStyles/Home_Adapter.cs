using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Content;

namespace TablesAndCellStyles {

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
            { "Cell Styles", new List<Sample>() {
                new Sample ("SimpleListItem1",                  typeof(SimpleListItem1)),
                new Sample ("SimpleListItem2",                  typeof(SimpleListItem2)),
                new Sample ("ActivityListItem",                 typeof(ActivityListItem)),
                new Sample ("TwoLineListItem",                  typeof(TwoLineListItem)),
            } },
            { "Accessory Styles", new List<Sample>() {
                new Sample ("SimpleListItemChecked",            typeof(SimpleListItemChecked)),
                new Sample ("SimpleListItemSingleChoice",       typeof(SimpleListItemSingleChoice)),
                new Sample ("SimpleListItemMultipleChoice",     typeof(SimpleListItemMultipleChoice)),
            } },
            { "Custom Cells", new List<Sample>() {
                new Sample ("ImageAndSubtitle",                 typeof(ImageAndSubtitle)),
                new Sample ("DateList",                         typeof(DateList)),
            } },
            { "Table Styles", new List<Sample>() {
                new Sample ("Fast Scroll",                      typeof(FastScroll)),
            } },
            { "Custom Tables", new List<Sample>() {
                new Sample ("Labelled Sections",                typeof(LabelledSections)),
                new Sample ("Labelled Sections with Indexer",   typeof(LabelledSectionsIndexer)),
                new Sample ("Gradient Background",              typeof(GradientBackground)),
                new Sample ("Custom Fast Scroll (API11)",       typeof(CustomFastScroll)),
            } },
        };

        const int TypeSectionHeader = 0;
        const int TypeSectionSample = 1;

        readonly Activity context;
        readonly IList<object> rows = new List<object>();

        readonly ArrayAdapter<string> headers;
        readonly Dictionary<string, IAdapter> sections = new Dictionary<string, IAdapter>();

        public Home_Adapter(Activity context)
        {
            this.context = context;
            headers = new ArrayAdapter<string>(context, Resource.Layout.HomeSectionHeader, Resource.Id.Text1);

            rows = new List<object>();
            foreach (var section in samples.Keys) {
                headers.Add(section);
                sections.Add(section, new ArrayAdapter<Sample>(context, Android.Resource.Layout.SimpleListItem1, samples [section]));
                rows.Add(new Header { Name = section, SectionIndex = sections.Count-1});
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

        public override int ViewTypeCount {
            get {
                return 1 + sections.Values.Sum (adapter => adapter.ViewTypeCount);
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
                view = headers.GetView(((Header) item).SectionIndex, convertView, parent);
                view.Clickable = false;
                view.LongClickable = false;
                return view;
            }

            int i = position-1;
            while (i > 0 && rows[i] is Sample)
                i--;
            Header h = (Header) rows[i];
            view = sections[h.Name].GetView(position-i-1, convertView, parent);
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = ((Sample) item).Name;
            return view;
        }
    }
}
