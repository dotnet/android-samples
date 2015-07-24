using System;
using Android.Widget;
using System.Collections.Generic;
using Android.Content.Res;
using Android.Views;
using Android.App;
using Android.Graphics.Drawables;
using Topeka.Helpers;
using Android.Graphics;
using Topeka.Persistence;

namespace Topeka.Adapters
{
    public class CategoryAdapter : BaseAdapter
    {
        public const string Drawable = "drawable";
        const string IconCategory = "icon_category_";
        readonly Resources resources;
        readonly string packageName;
        readonly LayoutInflater layoutInflater;
        readonly Activity activity;
        List<Category> categories;
        public override int Count
        {
            get
            {
                return categories.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return categories[position].Id.GetHashCode();
        }

        public override bool HasStableIds
        {
            get
            {
                return true;
            }
        }

        public CategoryAdapter(Activity activity)
        {
            resources = activity.Resources;
            this.activity = activity;
            packageName = this.activity.PackageName;
            layoutInflater = LayoutInflater.From(activity.ApplicationContext);
            UpdateCategories(activity);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                convertView = layoutInflater.Inflate(Resource.Layout.item_category, parent, false);
                convertView.Tag = new CategoryViewHolder((LinearLayout)convertView);
            }
            var holder = (CategoryViewHolder)convertView.Tag;
            var category = (Category)GetItem(position);
            var theme = category.Theme;
            SetCategoryIcon(category, holder.Icon);
            convertView.SetBackgroundColor(GetColor(theme.WindowBackgroundColor));
            holder.Title.Text = category.Name;
            holder.Title.SetTextColor(GetColor((theme.TextPrimaryColor)));
            holder.Title.SetBackgroundColor(GetColor(theme.PrimaryColor));
            return convertView;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return categories[position];
        }

        public override bool AreAllItemsEnabled()
        {
            return false;
        }

        void SetCategoryIcon(Category category, ImageView icon)
        {
            var categoryImageResource = resources.GetIdentifier(IconCategory + category.Id, Drawable, packageName);
            var solved = category.Solved;
            if (solved)
            {
                var solvedIcon = LoadSolvedIcon(category, categoryImageResource);
                icon.SetImageDrawable(solvedIcon);
            }
            else
            {
                icon.SetImageResource(categoryImageResource);
            }
        }

        public override void NotifyDataSetChanged()
        {
            base.NotifyDataSetChanged();
            UpdateCategories(activity);
        }

        void UpdateCategories(Activity activity)
        {
            categories = TopekaDatabaseHelper.GetCategories(activity, true);
        }

        private LayerDrawable LoadSolvedIcon(Category category, int categoryImageResource)
        {
            var done = LoadTintedDoneDrawable();
            var categoryIcon = LoadTintedCategoryDrawable(category, categoryImageResource);
            var layers = new Drawable[] { categoryIcon, done };
            return new LayerDrawable(layers);
        }

        Drawable LoadTintedCategoryDrawable(Category category, int categoryImageResource)
        {
            var categoryIcon = activity.GetDrawable(categoryImageResource);
            TintDrawable(category.Theme.PrimaryColor, ref categoryIcon);
            return categoryIcon;
        }

        Drawable LoadTintedDoneDrawable()
        {
            var done = activity.GetDrawable(Resource.Drawable.ic_tick);
            TintDrawable(Android.Resource.Color.White, ref done);
            return done;
        }

        void TintDrawable(int colorRes, ref Drawable drawable)
        {
            drawable.SetTint(GetColor(colorRes));
        }

        Color GetColor(int colorRes)
        {
            return resources.GetColor(colorRes);
        }
    }
}
