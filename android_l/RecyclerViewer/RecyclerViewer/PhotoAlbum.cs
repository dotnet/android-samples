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
    // Photo album, holds image resource IDs and caption:
    public class PhotoAlbum
    {
        // Resource IDs to the photos that make up my photo album.
        static int[] m_builtInImageIds 
            = new int[] { Resource.Drawable.before_mobile_phones,
                          Resource.Drawable.louvre_1,
                          Resource.Drawable.la_tour_eiffel,
                          Resource.Drawable.buckingham_guards,
                          Resource.Drawable.big_ben_1,
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

        // Captions for the photos in the above list. The order must
        // match m_imageIds.
        static string[] m_builtInCaptions 
            = new string[] { "Before mobile phones",
                             "The Lourve",
                             "The Eiffel Tower",
                             "Buckingham Palace",
                             "Big Ben skyline",
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

        // The following code simulates a photo album database: 

        // List of image IDs for this instance:
        private int[] m_imageIds; 

        // List of captions for this instance:
        private string[] m_captions; 

        // Random number generator for shuffling the photos:
        Random m_random;

        // Create instance copies of the two static arrays and 
        // the random number generator:
        public PhotoAlbum ()
        {
            m_imageIds = m_builtInImageIds;
            m_captions = m_builtInCaptions;
            m_random = new Random();
        }

        // Return the number of photos in the photo album:
        public int NumPhotos 
        { 
            get { return m_imageIds.Length; } 
        }

        // Get the caption for the specified photo position:
        public string GetCaption(int position)
        {
            return m_captions[position];
        }

        // Get the imaged ID for the specified photo position:
        public int GetImage(int position)
        {
            return m_imageIds[position];
        }

        // Shuffle the order of the photos:
        public void Shuffle ()
        {  
            // Use the Fisher-Yates shuffle algorithm:
            for (int idx = 0; idx < m_imageIds.Length; ++idx)
            {
                // Get the temporary caption and image ID at idx:
                string tmpCaption = m_captions[idx];
                int tmpId = m_imageIds[idx];

                // Generate a next random index between idx (inclusive) to 
                // Length (noninclusive):
                int rnd = m_random.Next(idx, m_imageIds.Length);

                // Exchange with temporary caption/ID:
                m_captions[idx] = m_captions[rnd];
                m_captions[rnd] = tmpCaption;
                m_imageIds[idx] = m_imageIds[rnd];
                m_imageIds[rnd] = tmpId;
            }
        }
    }
}
