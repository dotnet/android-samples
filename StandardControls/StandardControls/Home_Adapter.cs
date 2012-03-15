using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Content;

namespace StandardControls {
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

            samples.Add("Form Controls", new List<Sample>() { 
                   new Sample ("TextView", typeof(TextViewScreen))
                ,  new Sample ("EditText", typeof(EditTextScreen))
                ,  new Sample ("AutoCompleteTextView", typeof(AutoCompleteTextViewScreen))
                ,  new Sample ("Button", typeof(ButtonScreen))
                ,  new Sample ("CheckBox", typeof(CheckBoxScreen))
                ,  new Sample ("RadioGroup", typeof(RadioGroupScreen))
                ,  new Sample ("RatingBar", typeof(RatingBarScreen))
            });
            samples.Add("Content Controls", new List<Sample>() {
                   new Sample ("ImageView", typeof(ImageViewScreen))
                ,  new Sample ("Gallery", typeof(GalleryScreen))
                ,  new Sample ("ScrollView", typeof(ScrollViewScreen))
                ,  new Sample ("HorizontalScrollView", typeof(HorizontalScrollViewScreen))
                ,  new Sample ("GridView", typeof(GridViewScreen))
            });
            samples.Add("Progress Controls", new List<Sample>() {
                   new Sample ("ProgressDialog", typeof(ProgressDialogScreen))
                ,  new Sample ("ProgressBar", typeof(ProgressBarScreen))
            });
            samples.Add("Popups", new List<Sample>() {
                   new Sample ("Toast", typeof(ToastScreen))
                ,  new Sample ("Alert", typeof(AlertScreen))
            });
            samples.Add("Pickers", new List<Sample>() { 
                   new Sample ("Spinner", typeof(SpinnerScreen))
                ,  new Sample ("DatePickerDialog", typeof(DatePickerDialogScreen))
                ,  new Sample ("TimePickerDialogScreen", typeof(TimePickerDialogScreen))
            });
            samples.Add("Menus", new List<Sample>() {
                   new Sample ("Options Menu", typeof(OptionsMenuScreen))
                ,  new Sample ("Many Options", typeof(OptionsLongMenuScreen))
                ,  new Sample ("Context Menu", typeof(ContextMenuScreen))
                
            });
            samples.Add("Layouts", new List<Sample>() {
                    new Sample ("Tab Layout", typeof(TabLayoutScreen))
                ,   new Sample ("RelativeLayout", typeof(RelativeLayoutScreen))
                ,   new Sample ("LinearLayout (Vertical)", typeof(LinearLayoutScreen))
                ,   new Sample ("LinearLayout (Horizontal)", typeof(HorizontalLinearLayoutScreen))
                //,   new Sample ("LinearLayout (Vertical+Weight)", typeof(LinearLayoutWeightScreen))
                //,   new Sample ("LinearLayout (Horizontal+Weight)", typeof(HorizontalLinearLayoutWeightScreen))
                ,   new Sample ("FrameLayout", typeof(FrameLayoutScreen))
                ,   new Sample ("TableLayout", typeof(TableLayoutScreen))
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
