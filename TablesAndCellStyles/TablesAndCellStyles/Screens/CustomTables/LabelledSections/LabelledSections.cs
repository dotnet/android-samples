using System;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Widget;
using System.Collections.Generic;

// see also Xamarin Sample 
// https://github.com/xamarin/monodroid-samples/tree/master/LabelledSections

namespace TablesAndCellStyles {
    [Activity(Label = "LabelledSections")]
    public class LabelledSections : ListActivity {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ListAdapter = new ArrayAdapter<String> (this, Android.Resource.Layout.SimpleListItem1, COUNTRIES);

            ListView.FastScrollEnabled = true;
            
            var data = new ListItemCollection<ListItemValue>();
            foreach (var c in COUNTRIES) data.Add(new ListItemValue(c));
            var sortedContacts = data.GetSortedData(); 
            ListAdapter = CreateAdapter(sortedContacts); 
        }
        protected override void OnListItemClick(Android.Widget.ListView l, Android.Views.View v, int position, long id)
        {
            var t = l.Adapter.GetItem(position);
            Android.Widget.Toast.MakeText(this, t.ToString(), Android.Widget.ToastLength.Short).Show();
            Console.WriteLine("Clicked on " + t);
        }
        SeparatedListAdapter CreateAdapter<T> (Dictionary<string, List<T>> sortedObjects) where T : IHasLabel, IComparable<T> { 
            var adapter = new SeparatedListAdapter(this); 
            foreach (var e in sortedObjects.OrderBy(de => de.Key)) { 
                var label = e.Key; var section = e.Value; 
                adapter.AddSection(label, new ArrayAdapter<T>(this, Resource.Layout.SectionListItem, section)); 
            } return adapter; 
        }
        static string[] COUNTRIES = new string[] { "Afghanistan", "Albania", "Algeria", "American Samoa", "Andorra",    "Angola", "Anguilla", "Antarctica", "Antigua and Barbuda", "Argentina",    "Armenia", "Aruba", "Australia", "Austria", "Azerbaijan",    "Bahrain", "Bangladesh", "Barbados", "Belarus", "Belgium",    "Belize", "Benin", "Bermuda", "Bhutan", "Bolivia",    "Bosnia and Herzegovina", "Botswana", "Bouvet Island", "Brazil", "British Indian Ocean Territory",    "British Virgin Islands", "Brunei", "Bulgaria", "Burkina Faso", "Burundi",    "Cote d'Ivoire", "Cambodia", "Cameroon", "Canada", "Cape Verde",    "Cayman Islands", "Central African Republic", "Chad", "Chile", "China",    "Christmas Island", "Cocos (Keeling) Islands", "Colombia", "Comoros", "Congo",    "Cook Islands", "Costa Rica", "Croatia", "Cuba", "Cyprus", "Czech Republic",    "Democratic Republic of the Congo", "Denmark", "Djibouti", "Dominica", "Dominican Republic",    "East Timor", "Ecuador", "Egypt", "El Salvador", "Equatorial Guinea", "Eritrea",    "Estonia", "Ethiopia", "Faeroe Islands", "Falkland Islands", "Fiji", "Finland",    "Former Yugoslav Republic of Macedonia", "France", "French Guiana", "French Polynesia",    "French Southern Territories", "Gabon", "Georgia", "Germany", "Ghana", "Gibraltar",    "Greece", "Greenland", "Grenada", "Guadeloupe", "Guam", "Guatemala", "Guinea", "Guinea-Bissau",    "Guyana", "Haiti", "Heard Island and McDonald Islands", "Honduras", "Hong Kong", "Hungary",    "Iceland", "India", "Indonesia", "Iran", "Iraq", "Ireland", "Israel", "Italy", "Jamaica",    "Japan", "Jordan", "Kazakhstan", "Kenya", "Kiribati", "Kuwait", "Kyrgyzstan", "Laos",    "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein", "Lithuania", "Luxembourg",    "Macau", "Madagascar", "Malawi", "Malaysia", "Maldives", "Mali", "Malta", "Marshall Islands",    "Martinique", "Mauritania", "Mauritius", "Mayotte", "Mexico", "Micronesia", "Moldova",    "Monaco", "Mongolia", "Montserrat", "Morocco", "Mozambique", "Myanmar", "Namibia",    "Nauru", "Nepal", "Netherlands", "Netherlands Antilles", "New Caledonia", "New Zealand",    "Nicaragua", "Niger", "Nigeria", "Niue", "Norfolk Island", "North Korea", "Northern Marianas",    "Norway", "Oman", "Pakistan", "Palau", "Panama", "Papua New Guinea", "Paraguay", "Peru",    "Philippines", "Pitcairn Islands", "Poland", "Portugal", "Puerto Rico", "Qatar",    "Reunion", "Romania", "Russia", "Rwanda", "Sqo Tome and Principe", "Saint Helena",    "Saint Kitts and Nevis", "Saint Lucia", "Saint Pierre and Miquelon",    "Saint Vincent and the Grenadines", "Samoa", "San Marino", "Saudi Arabia", "Senegal",    "Seychelles", "Sierra Leone", "Singapore", "Slovakia", "Slovenia", "Solomon Islands",    "Somalia", "South Africa", "South Georgia and the South Sandwich Islands", "South Korea",    "Spain", "Sri Lanka", "Sudan", "Suriname", "Svalbard and Jan Mayen", "Swaziland", "Sweden",    "Switzerland", "Syria", "Taiwan", "Tajikistan", "Tanzania", "Thailand", "The Bahamas",    "The Gambia", "Togo", "Tokelau", "Tonga", "Trinidad and Tobago", "Tunisia", "Turkey",    "Turkmenistan", "Turks and Caicos Islands", "Tuvalu", "Virgin Islands", "Uganda",    "Ukraine", "United Arab Emirates", "United Kingdom",    "United States", "United States Minor Outlying Islands", "Uruguay", "Uzbekistan",    "Vanuatu", "Vatican City", "Venezuela", "Vietnam", "Wallis and Futuna", "Western Sahara",    "Yemen", "Yugoslavia", "Zambia", "Zimbabwe"  };
    }
}
