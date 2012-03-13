using System;
using System.Collections.Generic;
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
    public class Home_Adapter : BaseAdapter<Sample> {
        Activity context = null;
        Dictionary<string, List<Sample>> samples;
        private readonly IList<object> rows;

        public Home_Adapter(Activity context)
            : base()
        {
            this.context = context;
            samples = new Dictionary<string, List<Sample>>();

            samples.Add("Cell Styles", new List<Sample>() { 
                   new Sample ("SimpleListItem1", typeof(SimpleListItem1))
                ,  new Sample ("SimpleListItem2", typeof(SimpleListItem2))
                ,  new Sample ("ActivityListItem", typeof(ActivityListItem))
                ,  new Sample ("TwoLineListItem", typeof(TwoLineListItem))
            });
            samples.Add("Accessory Styles", new List<Sample>() {
                   new Sample ("SimpleListItemChecked", typeof(SimpleListItemChecked))
                ,  new Sample ("SimpleListItemSingleChoice", typeof(SimpleListItemSingleChoice))
                ,  new Sample ("SimpleListItemMultipleChoice", typeof(SimpleListItemMultipleChoice))
            });
            samples.Add("Custom Cells", new List<Sample>() {
                   new Sample ("ImageAndSubtitle", typeof(ImageAndSubtitle))
                ,  new Sample ("DateList", typeof(DateList))
                
            }); 
            samples.Add("Table Styles", new List<Sample>() { 
                   new Sample ("Fast Scroll", typeof(FastScroll))
            });
            samples.Add("Custom Tables", new List<Sample>() {
                   new Sample ("Labelled Sections", typeof(LabelledSections))
                ,  new Sample ("Labelled Sections with Indexer", typeof(LabelledSectionsIndexer))
                ,  new Sample ("Gradient Background", typeof(GradientBackground))
                ,  new Sample ("Custom Fast Scroll (API11)", typeof(CustomFastScroll))

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
