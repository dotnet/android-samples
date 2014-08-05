// Copyright 2013 Google Inc. All Rights Reserved.

package com.google.android.gms.cast.samples.democastplayer;

import com.google.android.gms.cast.MediaInfo;
import com.google.android.gms.cast.MediaMetadata;
import com.google.android.gms.cast.MediaTrack;
import com.google.android.gms.common.images.WebImage;

import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserException;

import android.content.Context;
import android.net.Uri;
import android.preference.PreferenceManager;
import android.text.TextUtils;
import android.util.AttributeSet;
import android.util.Log;
import android.util.Xml;
import android.widget.ArrayAdapter;

import java.io.IOException;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.URLConnection;
import java.util.ArrayList;
import java.util.List;

abstract class MediaSelectionDialog extends ListSelectionDialog<MediaInfo> {
    private static final String XML_TAG_MEDIA = "media";
    private static final String XML_TAG_MEDIAS = "medias";
    private static final String XML_TAG_TRACK = "track";
    private static final String XML_ATTR_ARTIST = "artist";
    private static final String XML_ATTR_CONTENT_ID = "contentId";
    private static final String XML_ATTR_ID = "id";
    private static final String XML_ATTR_IMAGE_URL = "imageUrl";
    private static final String XML_ATTR_LANGUAGE = "language";
    private static final String XML_ATTR_MIME_TYPE = "mimeType";
    private static final String XML_ATTR_NAME = "name";
    private static final String XML_ATTR_SERIES_TITLE = "seriesTitle";
    private static final String XML_ATTR_STUDIO = "studio";
    private static final String XML_ATTR_SUBTYPE = "subtype";
    private static final String XML_ATTR_TITLE = "title";
    private static final String XML_ATTR_TYPE = "type";
    private static final String XML_ATTR_URL = "url";
    private static final String XML_ATTR_VERSION = "version";
    private static final String TYPE_MOVIE = "movie";
    private static final String TYPE_MUSIC = "music";
    private static final String TYPE_PHOTO = "photo";
    private static final String TYPE_TV = "tv";
    private static final String TRACK_TYPE_AUDIO = "audio";
    private static final String TRACK_TYPE_VIDEO = "video";
    private static final String TRACK_TYPE_TEXT = "text";
    private static final String TRACK_SUBTYPE_SUBTITLES = "subtitles";
    private static final String TRACK_SUBTYPE_CAPTIONS = "captions";
    private static final String TRACK_SUBTYPE_DESCRIPTIONS = "descriptions";
    private static final String TRACK_SUBTYPE_CHAPTERS = "chapters";
    private static final String TRACK_SUBTYPE_METADATA = "metadata";
    private static final String FILE_FORMAT_VERSION = "3";

    private static final String TAG = "MediaSelectionDialog";

    private final Context mContext;

    public MediaSelectionDialog(Context context) {
        super(context.getString(R.string.select_media_title));
        mContext = context;
    }

    @Override
    protected ArrayAdapter<MediaInfo> buildAdapter() {
        return new MediaAdapter(mContext);
    }

    @Override
    protected List<MediaInfo> loadItems() {
        String mediaListUrl = PreferenceManager.getDefaultSharedPreferences(
                mContext.getApplicationContext()).getString(AppConstants.PREF_KEY_MEDIA_URL,
                mContext.getString(R.string.media_list_url));

        Log.d(TAG, "fetching the media from " + mediaListUrl);
        List<MediaInfo> list = null;

        try {
            XmlPullParser parser;
            if (!TextUtils.isEmpty(mediaListUrl)) {
                URL url = new URL(mediaListUrl);
                URLConnection connection = url.openConnection();

                parser = Xml.newPullParser();
                parser.setInput(connection.getInputStream(), connection.getContentEncoding());
                parser.nextTag();
            } else {
                parser = getResources().getXml(R.xml.media);
            }

            list = new ArrayList<MediaInfo>();
            readFile(parser, list);
        } catch (MalformedURLException e) {
            Log.w(TAG, "Failed to read XML file: " + mediaListUrl, e);
        } catch (IOException e) {
            Log.w(TAG, "Failed to read XML file: " + mediaListUrl, e);
        } catch (XmlPullParserException e) {
            Log.w(TAG, "Failed to read XML file: " + mediaListUrl, e);
        }

        return list;
    }

    private void readFile(XmlPullParser parser, List<MediaInfo> list)
            throws XmlPullParserException, IOException {
        boolean inMediaTag = false;
        MediaInfo mediaInfo = null;
        MediaInfo.Builder mediaInfoBuilder = null;
        List<MediaTrack> mediaTracks = null;

        for (int type = parser.getEventType(); type != XmlPullParser.END_DOCUMENT;
                type = parser.next()) {
            if ((type == XmlPullParser.START_TAG) && XML_TAG_MEDIAS.equals(parser.getName())) {
                AttributeSet attrs = Xml.asAttributeSet(parser);

                String version = attrs.getAttributeValue(null, XML_ATTR_VERSION);
                if (!FILE_FORMAT_VERSION.equals(version)) {
                    throw new IOException("Incompatible format");
                }
            } else if ((type == XmlPullParser.START_TAG) && !inMediaTag
                    && XML_TAG_MEDIA.equals(parser.getName())) {
                inMediaTag = true;
                AttributeSet attrs = Xml.asAttributeSet(parser);

                String url = attrs.getAttributeValue(null, XML_ATTR_URL);
                String mimeType = attrs.getAttributeValue(null, XML_ATTR_MIME_TYPE);
                String imageUrlText = attrs.getAttributeValue(null, XML_ATTR_IMAGE_URL);
                Uri imageUrl = (imageUrlText != null) ? Uri.parse(imageUrlText) : null;

                String metadataTypeToken = attrs.getAttributeValue(null, XML_ATTR_TYPE);
                int metadataType = MediaMetadata.MEDIA_TYPE_GENERIC;

                if (TYPE_MOVIE.equals(metadataTypeToken)) {
                    metadataType = MediaMetadata.MEDIA_TYPE_MOVIE;
                } else if (TYPE_MUSIC.equals(metadataTypeToken)) {
                    metadataType = MediaMetadata.MEDIA_TYPE_MUSIC_TRACK;
                } else if (TYPE_PHOTO.equals(metadataTypeToken)) {
                    metadataType = MediaMetadata.MEDIA_TYPE_PHOTO;
                } else if (TYPE_TV.equals(metadataTypeToken)) {
                    metadataType = MediaMetadata.MEDIA_TYPE_TV_SHOW;
                }

                MediaMetadata metadata = new MediaMetadata(metadataType);

                String title = attrs.getAttributeValue(null, XML_ATTR_TITLE);
                if (title != null) {
                    metadata.putString(MediaMetadata.KEY_TITLE, title);
                }

                String studio = attrs.getAttributeValue(null, XML_ATTR_STUDIO);
                if (studio != null) {
                    metadata.putString(MediaMetadata.KEY_STUDIO, studio);
                }

                String artist = attrs.getAttributeValue(null, XML_ATTR_ARTIST);
                if (artist != null) {
                    metadata.putString(MediaMetadata.KEY_ARTIST, artist);
                }

                String seriesTitle = attrs.getAttributeValue(null, XML_ATTR_SERIES_TITLE);
                if (seriesTitle != null) {
                    metadata.putString(MediaMetadata.KEY_SERIES_TITLE, seriesTitle);
                }

                if (imageUrl != null) {
                    metadata.addImage(new WebImage(imageUrl));
                }

                mediaInfoBuilder = new MediaInfo.Builder(url)
                        .setStreamType(MediaInfo.STREAM_TYPE_BUFFERED)
                        .setContentType(mimeType).setMetadata(metadata);
            } else if ((type == XmlPullParser.END_TAG) && XML_TAG_MEDIA.equals(parser.getName())
                    && inMediaTag) {
                list.add(mediaInfoBuilder.setMediaTracks(mediaTracks).build());
                inMediaTag = false;
                mediaInfoBuilder = null;
                mediaTracks = null;
            } else if ((type == XmlPullParser.START_TAG) && XML_TAG_TRACK.equals(parser.getName())
                    && inMediaTag) {
                AttributeSet attrs = Xml.asAttributeSet(parser);

                String idToken = attrs.getAttributeValue(null, XML_ATTR_ID);
                String contentId = attrs.getAttributeValue(null, XML_ATTR_CONTENT_ID);
                String typeToken = attrs.getAttributeValue(null, XML_ATTR_TYPE);
                String subtypeToken = attrs.getAttributeValue(null, XML_ATTR_SUBTYPE);
                String name = attrs.getAttributeValue(null, XML_ATTR_NAME);
                String language = attrs.getAttributeValue(null, XML_ATTR_LANGUAGE);

                if (idToken == null) {
                    continue;
                }

                long id = 0;
                try {
                    id = Long.parseLong(idToken);
                } catch (NumberFormatException e) {
                    // TODO: log error
                    continue;
                }

                int trackType = MediaTrack.TYPE_UNKNOWN;

                if (TRACK_TYPE_AUDIO.equals(typeToken)) {
                    trackType = MediaTrack.TYPE_AUDIO;
                } else if (TRACK_TYPE_VIDEO.equals(typeToken)) {
                    trackType = MediaTrack.TYPE_VIDEO;
                } else if (TRACK_TYPE_TEXT.equals(typeToken)) {
                    trackType = MediaTrack.TYPE_TEXT;
                }

                if ((trackType == MediaTrack.TYPE_UNKNOWN) || (name == null)) {
                    // TODO: log error
                    continue;
                }

                int subtype = MediaTrack.SUBTYPE_NONE;
                if (TRACK_SUBTYPE_SUBTITLES.equals(subtypeToken)) {
                    subtype = MediaTrack.SUBTYPE_SUBTITLES;
                } else if (TRACK_SUBTYPE_CAPTIONS.equals(subtypeToken)) {
                    subtype = MediaTrack.SUBTYPE_CAPTIONS;
                } else if (TRACK_SUBTYPE_DESCRIPTIONS.equals(subtypeToken)) {
                    subtype = MediaTrack.SUBTYPE_DESCRIPTIONS;
                } else if (TRACK_SUBTYPE_CHAPTERS.equals(subtypeToken)) {
                    subtype = MediaTrack.SUBTYPE_CHAPTERS;
                } else if (TRACK_SUBTYPE_METADATA.equals(subtypeToken)) {
                    subtype = MediaTrack.SUBTYPE_METADATA;
                }

                MediaTrack.Builder builder = new MediaTrack.Builder(id, trackType)
                        .setName(name)
                        .setSubtype(subtype);

                if (contentId != null) {
                    builder.setContentId(contentId);
                }

                if (language != null) {
                    builder.setLanguage(language);
                }

                MediaTrack track = builder.build();

                if (mediaTracks == null) {
                    mediaTracks = new ArrayList<MediaTrack>();
                }
                mediaTracks.add(track);
            }
        }
    }

}
