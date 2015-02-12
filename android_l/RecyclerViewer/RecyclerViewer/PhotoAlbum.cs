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
    // Photo: contains image resource ID and caption:
    public class Photo
    {
        // Photo ID for this photo:
        public int m_photoId;

        // Caption text for this photo:
        public string m_caption;

        // Return the ID of the photo:
        public int PhotoID 
        { 
            get { return m_photoId; } 
        }

        // Return the Caption of the photo:
        public string Caption 
        { 
            get { return m_caption; } 
        }
    }

    // Photo album: holds image resource IDs and caption:
    public class PhotoAlbum
    {
        // Built-in photo collection - this could be replaced with
        // a photo database:

        static Photo[] m_builtIn_photos = {
            new Photo { m_photoId = Resource.Drawable.before_mobile_phones,
                        m_caption = "Before mobile phones" },
            new Photo { m_photoId = Resource.Drawable.louvre_1,
                        m_caption = "The Louvre" },
            new Photo { m_photoId = Resource.Drawable.la_tour_eiffel,
                        m_caption = "The Eiffel Tower" },
            new Photo { m_photoId = Resource.Drawable.buckingham_guards,
                        m_caption = "Buckingham Palace" },
            new Photo { m_photoId = Resource.Drawable.big_ben_1,
                        m_caption = "Big Ben skyline" },
            new Photo { m_photoId = Resource.Drawable.big_ben_2,
                        m_caption = "Big Ben from below" },
            new Photo { m_photoId = Resource.Drawable.london_eye,
                        m_caption = "The London Eye" },
            new Photo { m_photoId = Resource.Drawable.eurostar,
                        m_caption = "Eurostar Train" },
            new Photo { m_photoId = Resource.Drawable.arc_de_triomphe,
                        m_caption = "Arc de Triomphe" },
            new Photo { m_photoId = Resource.Drawable.louvre_2,
                        m_caption = "Inside the Louvre" },
            new Photo { m_photoId = Resource.Drawable.versailles_fountains,
                        m_caption = "Versailles fountains" },
            new Photo { m_photoId = Resource.Drawable.modest_accomodations,
                        m_caption = "Modest accomodations" },
            new Photo { m_photoId = Resource.Drawable.notre_dame,
                        m_caption = "Notre Dame" },
            new Photo { m_photoId = Resource.Drawable.inside_notre_dame,
                        m_caption = "Inside Notre Dame" },
            new Photo { m_photoId = Resource.Drawable.seine_river,
                        m_caption = "The Seine" },
            new Photo { m_photoId = Resource.Drawable.rue_cler,
                        m_caption = "Rue Cler" },
            new Photo { m_photoId = Resource.Drawable.champ_elysees,
                        m_caption = "The Avenue des Champs-Elysees" },
            new Photo { m_photoId = Resource.Drawable.seine_barge,
                        m_caption = "Seine barge" },
            new Photo { m_photoId = Resource.Drawable.versailles_gates,
                        m_caption = "Gates of Versailles" },
            new Photo { m_photoId = Resource.Drawable.edinburgh_castle_2,
                        m_caption = "Edinburgh Castle" },
            new Photo { m_photoId = Resource.Drawable.edinburgh_castle_1,
                        m_caption = "Edinburgh Castle up close" },
            new Photo { m_photoId = Resource.Drawable.old_meets_new,
                        m_caption = "Old meets new" },
            new Photo { m_photoId = Resource.Drawable.edinburgh_from_on_high,
                        m_caption = "Edinburgh from on high" },
            new Photo { m_photoId = Resource.Drawable.edinburgh_station,
                        m_caption = "Edinburgh station" },
            new Photo { m_photoId = Resource.Drawable.scott_monument,
                        m_caption = "Scott Monument" },
            new Photo { m_photoId = Resource.Drawable.view_from_holyrood_park,
                        m_caption = "View from Holyrood Park" },
            new Photo { m_photoId = Resource.Drawable.tower_of_london,
                        m_caption = "Outside the Tower of London" },
            new Photo { m_photoId = Resource.Drawable.tower_visitors,
                        m_caption = "Tower of London visitors" },
            new Photo { m_photoId = Resource.Drawable.one_o_clock_gun,
                        m_caption = "One O'Clock Gun" },
            new Photo { m_photoId = Resource.Drawable.victoria_albert,
                        m_caption = "Victoria and Albert Museum" },
            new Photo { m_photoId = Resource.Drawable.royal_mile,
                        m_caption = "The Royal Mile" },
            new Photo { m_photoId = Resource.Drawable.museum_and_castle,
                        m_caption = "Edinburgh Museum and Castle" },
            new Photo { m_photoId = Resource.Drawable.portcullis_gate,
                        m_caption = "Portcullis Gate" },
            new Photo { m_photoId = Resource.Drawable.to_notre_dame,
                        m_caption = "Left or right?" },
            new Photo { m_photoId = Resource.Drawable.pompidou_centre,
                        m_caption = "Pompidou Centre" },
            new Photo { m_photoId = Resource.Drawable.heres_lookin_at_ya,
                        m_caption = "Here's Lookin' at Ya!" },
            };

        // Array of photos that make up the album:
        private Photo[] m_photos;

        // Random number generator for shuffling the photos:
        Random m_random;

        // Create an instance copy of the built-in photo list and
        // create the random number generator:
        public PhotoAlbum ()
        {
            m_photos = m_builtIn_photos;
            m_random = new Random();
        }

        // Return the number of photos in the photo album:
        public int NumPhotos 
        { 
            get { return m_photos.Length; } 
        }

        // Indexer (read only) for accessing a photo:
        public Photo this[int i] 
        {
            get { return m_photos[i]; }
        }

        // Shuffle the order of the photos:
        public void Shuffle ()
        {  
            // Use the Fisher-Yates shuffle algorithm:
            for (int idx = 0; idx < m_photos.Length; ++idx)
            {
                // Save the photo at idx:
                Photo tmpPhoto = m_photos[idx];

                // Generate a next random index between idx (inclusive) to 
                // Length (noninclusive):
                int rnd = m_random.Next(idx, m_photos.Length);

                // Exchange photo at idx with randomly-chosen (later) photo:
                m_photos[idx] = m_photos[rnd];
                m_photos[rnd] = tmpPhoto;
            }
        }
    }
}
