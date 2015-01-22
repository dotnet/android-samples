using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using System.Collections.Generic;

namespace RecyclerViewer
{
    // Photo album, holds image resource IDs and title text:
    public class PhotoAlbum
    {
        // Resource IDs to the photos that make up my photo album.
        static int[] m_imageIds 
            = new int[] { Resource.Drawable.before_mobile_phones,
                          Resource.Drawable.la_tour_eiffel,
                          Resource.Drawable.buckingham_guards,
                          Resource.Drawable.big_ben_1,
                          Resource.Drawable.louvre_1,
                          Resource.Drawable.big_ben_2,
                          Resource.Drawable.london_eye,
                          Resource.Drawable.eurostar,
                          Resource.Drawable.arc_de_triomphe,
                          Resource.Drawable.louvre_2,
                          Resource.Drawable.versailles_fountains,
                          Resource.Drawable.modest_accomodations,
                          Resource.Drawable.notre_dame,
                          Resource.Drawable.inside_notre_dame,
                          Resource.Drawable.seine_river,
                          Resource.Drawable.rue_cler,
                          Resource.Drawable.champ_elysees,
                          Resource.Drawable.seine_barge,
                          Resource.Drawable.versailles_gates,
                          Resource.Drawable.edinburgh_castle_2,
                          Resource.Drawable.edinburgh_castle_1,
                          Resource.Drawable.old_meets_new,
                          Resource.Drawable.edinburgh_from_on_high,
                          Resource.Drawable.edinburgh_station,
                          Resource.Drawable.scott_monument,
                          Resource.Drawable.view_from_holyrood_park,
                          Resource.Drawable.tower_of_london,
                          Resource.Drawable.tower_visitors,
                          Resource.Drawable.one_o_clock_gun,
                          Resource.Drawable.victoria_albert,
                          Resource.Drawable.royal_mile,
                          Resource.Drawable.museum_and_castle,
                          Resource.Drawable.portcullis_gate,
                          Resource.Drawable.to_notre_dame,
                          Resource.Drawable.pompidou_centre,
                          Resource.Drawable.heres_lookin_at_ya};

        // Titles for the photos in the above list. The order must
        // match m_imageIds.
        static string[] m_imageTitles 
            = new string[] { "Before mobile phones",
                             "The Eiffel Tower",
                             "Buckingham Palace",
                             "Big Ben skyline",
                             "The Lourve",
                             "Big Ben from below",
                             "The London Eye",
                             "Eurostar Train",
                             "Arc de Triomphe",
                             "Inside the Lourve",
                             "Versailles fountains",
                             "Modest accomodations",
                             "Notre Dame",
                             "Inside Notre Dame",
                             "The Seine",
                             "Rue Cler",
                             "The Avenue des Champs-Elysees",
                             "Seine barge",
                             "Gates of Versailles",
                             "Edinburgh Castle",
                             "Edinburgh Castle up close",
                             "Old meets new",
                             "Edinburgh from on high",
                             "Edinburgh station",
                             "Scott Monument",
                             "View from Holyrood Park",
                             "Outside the Tower of London",
                             "Tower of London visitors",
                             "One O'Clock Gun",
                             "Victoria and Albert Museum",
                             "The Royal Mile",
                             "Edinburgh Museum and Castle",
                             "Portcullis Gate",
                             "Left or right?",
                             "Pompidou Centre",
                             "Here's Lookin' at Ya!"};

        // Returns the number of photos in the photo album.
        public static int NumPhotos 
        { 
            get { return m_imageIds.Length; } 
        }

        // Returns the array of photo album titles.
        public static string[] Titles
        {
            get { return m_imageTitles; }
        }

        // Returns the array of photo album image IDs.
        public static int[] ImageIds
        {
            get { return m_imageIds; }
        }
    }
}
