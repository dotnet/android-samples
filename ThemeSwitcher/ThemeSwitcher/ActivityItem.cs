using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;

namespace ThemeSwitcher
{
    // Container used to hold an intent with its title
    class ActivityItem
    {
        public string Title { get; set; }
        public Intent Intent { get; set; }
        public override string ToString()
        {
            return Title;
        }
    }
}
