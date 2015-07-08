using System;
using System.Collections.Generic;
using Android.Media.Session;
using Android.Media;

namespace MediaBrowserService
{
	public static class QueueHelper
	{
		static readonly string Tag = LogHelper.MakeLogTag(typeof(QueueHelper));

		public static List<MediaSession.QueueItem> GetPlayingQueue(string mediaId,
			MusicProvider musicProvider) {

			// extract the browsing hierarchy from the media ID:
			var hierarchy = MediaIDHelper.GetHierarchy(mediaId);

			if (hierarchy.Length != 2) {
				LogHelper.Error(Tag, "Could not build a playing queue for this mediaId: ", mediaId);
				return null;
			}

			var categoryType = hierarchy[0];
			var categoryValue = hierarchy[1];
			LogHelper.Debug(Tag, "Creating playing queue for ", categoryType, ",  ", categoryValue);

			IEnumerable<MediaMetadata> tracks = null;
			// This sample only supports genre and by_search category types.
			if (categoryType == MediaIDHelper.MediaIdMusicsByGenre) {
				tracks = musicProvider.GetMusicsByGenre(categoryValue);
			} else if (categoryType == MediaIDHelper.MediaIdMusicsBySearch) {
				tracks = musicProvider.SearchMusic(categoryValue);
			}

			if (tracks == null) {
				LogHelper.Error(Tag, "Unrecognized category type: ", categoryType, " for mediaId ", mediaId);
				return null;
			}

			return ConvertToQueue(tracks, hierarchy[0], hierarchy[1]);
		}

		public static List<MediaSession.QueueItem> GetPlayingQueueFromSearch(String query,
			MusicProvider musicProvider) {

			LogHelper.Debug(Tag, "Creating playing queue for musics from search ", query);

			return ConvertToQueue(musicProvider.SearchMusic(query), MediaIDHelper.MediaIdMusicsBySearch, query);
		}


		public static int GetMusicIndexOnQueue(IEnumerable<MediaSession.QueueItem> queue, string mediaId) {
			var index = 0;
			foreach (var item in queue) {
				if (mediaId == item.Description.MediaId) {
					return index;
				}
				index++;
			}
			return -1;
		}

		public static int GetMusicIndexOnQueue(IEnumerable<MediaSession.QueueItem> queue,
			long queueId) {
			var index = 0;
			foreach (var item in queue) {
				if (queueId == item.QueueId) {
					return index;
				}
				index++;
			}
			return -1;
		}

		static List<MediaSession.QueueItem> ConvertToQueue(
			IEnumerable<MediaMetadata> tracks, params string[] categories) {
			var queue = new List<MediaSession.QueueItem>();
			var count = 0;
			foreach (var track in tracks) {
				var hierarchyAwareMediaID = MediaIDHelper.CreateMediaID(
					track.Description.MediaId, categories);

				var trackCopy = new MediaMetadata.Builder(track)
					.PutString(MediaMetadata.MetadataKeyMediaId, hierarchyAwareMediaID)
					.Build();

				var item = new MediaSession.QueueItem(trackCopy.Description, count++);
				queue.Add(item);
			}
			return queue;

		}
			
		public static List<MediaSession.QueueItem> GetRandomQueue(MusicProvider musicProvider) {
			var genres = musicProvider.Genres;
			if (genres.Count <= 1)
				return new List<MediaSession.QueueItem> ();
			var genre = genres [1];
			var tracks = musicProvider.GetMusicsByGenre (genre);

			return ConvertToQueue (tracks, MediaIDHelper.MediaIdMusicsByGenre, genre);
		}

		public static bool isIndexPlayable(int index, List<MediaSession.QueueItem> queue) {
			return (queue != null && index >= 0 && index < queue.Count);
		}
	}
}

