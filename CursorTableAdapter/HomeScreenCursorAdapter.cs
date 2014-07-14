using Android.App;
using Android.Content;
using Android.Database;
using Android.Views;
using Android.Widget;

namespace CursorTableAdapter {
    public class HomeScreenCursorAdapter : CursorAdapter {
        Activity context;
        public HomeScreenCursorAdapter(Activity context, ICursor c, bool autoRequery)
            : base(context, c, autoRequery)
        {
            this.context = context;
        }
        public override void BindView(View view, Context context, ICursor cursor)
        {
            var textView = view.FindViewById<TextView>(Android.Resource.Id.Text1);
            textView.Text = cursor.GetString(1); // 'name' is column 1
        }
        public override View NewView(Context context, ICursor cursor, ViewGroup parent)
        {
            return this.context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, parent, false);
        }
    }
}