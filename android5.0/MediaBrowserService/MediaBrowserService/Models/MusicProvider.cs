using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Android.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MediaBrowserService
{
	public class MusicProvider
	{
		static readonly string Tag = LogHelper.MakeLogTag (typeof(MusicProvider));
		const string CatalogUrl = "http://storage.googleapis.com/automotive-media/music.json";

		public const string CustomMetadataTrackSource = "__SOURCE__";

		const string JsonMusic = "music";
		const string JsonTitle = "title";
		const string JsonAlbum = "album";
		const string JsonArtist = "artist";
		const string JsonGenre = "genre";
		const string JsonSource = "source";
		const string JsonImage = "image";
		const string JsonTrackNumber = "trackNumber";
		const string JsonTotalTrackCount = "totalTrackCount";
		const string JsonDuration = "duration";

		Dictionary<string, List<MediaMetadata>> musicListByGenre;
		readonly Dictionary<string, MutableMediaMetadata> musicListById;

		readonly HashSet<string> favoriteTracks;

		enum State
		{
			NonInitialized,
			Initializing,
			Initialized
		};

		volatile State currentState = State.NonInitialized;

		public MusicProvider ()
		{
			musicListByGenre = new Dictionary<string, List<MediaMetadata>> ();
			musicListById = new Dictionary<string, MutableMediaMetadata> ();
			favoriteTracks = new HashSet<string> ();
		}

		public List<string> Genres { 
			get {
				return currentState != State.Initialized ? new List<string> () : new List<string> (musicListByGenre.Keys);
			}
		}

		public IEnumerable<MediaMetadata> GetMusicsByGenre (string genre)
		{
			if (currentState != State.Initialized || !musicListByGenre.ContainsKey (genre)) {
				return new List<MediaMetadata> ();
			}
			return musicListByGenre [genre];
		}

		public IEnumerable<MediaMetadata> SearchMusic (string titleQuery)
		{
			if (currentState != State.Initialized) {
				return new List<MediaMetadata> ();
			}
			var result = new List<MediaMetadata> ();
			titleQuery = titleQuery.ToLower ();
			foreach (var track in musicListById.Values) {
				if (track.Metadata.GetString (MediaMetadata.MetadataKeyTitle).ToLower ().Contains (titleQuery)) {
					result.Add (track.Metadata);
				}
			}
			return result;
		}

		public MediaMetadata GetMusic (string musicId)
		{
			return musicListById.ContainsKey (musicId) ? musicListById [musicId].Metadata : null;
		}

		public void UpdateMusic (string musicId, MediaMetadata metadata)
		{
			var track = musicListById [musicId];
			if (track == null)
				return;
			var oldGenre = track.Metadata.GetString (MediaMetadata.MetadataKeyGenre);
			var newGenre = metadata.GetString (MediaMetadata.MetadataKeyGenre);

			track.Metadata = metadata;

			if (oldGenre != newGenre)
				BuildListsByGenre ();
		}

		public void SetFavorite (string musicId, bool favorite)
		{
			if (favorite)
				favoriteTracks.Add (musicId);
			else
				favoriteTracks.Remove (musicId);
		}

		public bool IsFavorite (string musicId)
		{
			return favoriteTracks.Contains (musicId);
		}

		public bool IsInitialized {
			get {
				return currentState == State.Initialized;
			}
		}

		public async Task RetrieveMediaAsync (Action<bool> callback)
		{
			LogHelper.Debug (Tag, "retrieveMediaAsync called");
			if (currentState == State.Initialized) {
				callback (true);
				return;
			}

			await RetrieveMediaAsync ();
			callback (currentState == State.Initialized);
		}

		void BuildListsByGenre ()
		{
			var newMusicListByGenre = new Dictionary<string, List<MediaMetadata>> ();
			foreach (var m in musicListById.Values) {
				var genre = m.Metadata.GetString (MediaMetadata.MetadataKeyGenre);
				List<MediaMetadata> list;
				newMusicListByGenre.TryGetValue(genre, out list);
				if (list == null) {
					list = new List<MediaMetadata> ();
					newMusicListByGenre.Add (genre, list);
				}
				list.Add (m.Metadata);
			}
			musicListByGenre = newMusicListByGenre;
		}

		async Task RetrieveMediaAsync ()
		{
			try {
				if (currentState == State.NonInitialized) {
					currentState = State.Initializing;
					using (var client = new HttpClient ()) {
						var slashPos = CatalogUrl.LastIndexOf('/');
						var path = CatalogUrl.Substring(0, slashPos + 1);
						var jsonObj = await client.GetStringAsync (CatalogUrl);
						if (string.IsNullOrEmpty (jsonObj))
							return;
						var jObj = JObject.Parse (jsonObj);
						if (jObj == null)
							return;
						var tracks = (JArray)jObj [JsonMusic];
						if (tracks != null) {
							for (int j = 0; j < tracks.Count; j++) {
								var item = BuildFromJson (tracks [j], path);
								var musicId = item.GetString (MediaMetadata.MetadataKeyMediaId);
								musicListById.Add (musicId, new MutableMediaMetadata (musicId, item));
							}
							BuildListsByGenre ();
						}
					}
					currentState = State.Initialized;
				}
			} catch (JsonException e) {
				LogHelper.Error (Tag, e, "Could not retrieve music list");
			} catch (HttpRequestException e) {
				LogHelper.Error (Tag, e, "Could not retrieve music list");
			}finally {
				if (currentState != State.Initialized)
					currentState = State.NonInitialized;
			}
		}

		static MediaMetadata BuildFromJson (JToken json, string basePath)
		{
			var title = (string)json [JsonTitle];
			var album = (string)json [JsonAlbum];
			var artist = (string)json [JsonArtist];
			var genre = (string)json [JsonGenre];
			var source = (string)json [JsonSource];
			var iconUrl = (string)json [JsonImage];
			var trackNumber = (int)json [JsonTrackNumber];
			var totalTrackCount = (int)json [JsonTotalTrackCount];
			var duration = (int)json [JsonDuration] * 1000;

			LogHelper.Debug (Tag, "Found music track: ", json);

			if (!source.StartsWith ("http"))
				source = basePath + source;
			if (!iconUrl.StartsWith ("http"))
				iconUrl = basePath + iconUrl;

			var id = source.GetHashCode ().ToString ();

			return new MediaMetadata.Builder ()
				.PutString (MediaMetadata.MetadataKeyMediaId, id)
				.PutString (CustomMetadataTrackSource, source)
				.PutString (MediaMetadata.MetadataKeyAlbum, album)
				.PutString (MediaMetadata.MetadataKeyArtist, artist)
				.PutLong (MediaMetadata.MetadataKeyDuration, duration)
				.PutString (MediaMetadata.MetadataKeyGenre, genre)
				.PutString (MediaMetadata.MetadataKeyAlbumArtUri, iconUrl)
				.PutString (MediaMetadata.MetadataKeyTitle, title)
				.PutLong (MediaMetadata.MetadataKeyTrackNumber, trackNumber)
				.PutLong (MediaMetadata.MetadataKeyNumTracks, totalTrackCount)
				.Build ();
		}
	}
}

