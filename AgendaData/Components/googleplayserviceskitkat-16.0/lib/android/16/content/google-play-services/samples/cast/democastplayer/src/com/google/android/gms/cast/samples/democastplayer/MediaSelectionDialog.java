// Copyright 2013 Google Inc. All Rights Reserved.

package com.google.android.gms.cast.samples.democastplayer;

import com.google.android.gms.cast.MediaInfo;
import com.google.android.gms.cast.MediaMetadata;
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
    private static final String XML_ATTR_ARTIST = "artist";
    private static final String XML_ATTR_IMAGE_URL = "imageUrl";
    private static final String XML_ATTR_MIME_TYPE = "mimeType";
    private static final String XML_ATTR_PHOTOGRAPHER = "photographer";
    private static final String XML_ATTR_SERIES_TITLE = "seriesTitle";
    private static final String XML_ATTR_STUDIO = "studio";
    private static final String XML_ATTR_TITLE = "title";
    private static final String XML_ATTR_TYPE = "type";
    private static final String XML_ATTR_URL = "url";
    private static final String XML_ATTR_VERSION = "version";
    private static final String TYPE_MOVIE = "movie";
    private static final String TYPE_MUSIC = "music";
    private static final String TYPE_PHOTO = "photo";
    private static final String TYPE_TV = "tv";
    private static final String FILE_FORMAT_VERSION = "2";

    private static final String TAG = "MediaSelectionDialog";

    private final Context mContext;

    public MediaSelectionDialog(Context context) {
        super(context.getString(R.string.select_media));
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
            Log.w(TAG, "Failed to read XML file: " + mediaListUrl);
        } catch (IOException e) {
            Log.w(TAG, "Failed to read XML file: " + mediaListUrl);
        } catch (XmlPullParserException e) {
            Log.w(TAG, "Failed to read XML file: " + mediaListUrl);
        }

        return list;
    }

    private void readFile(XmlPullParser parser, List<MediaInfo> list)
            throws XmlPullParserException, IOException {
        for (int type = parser.getEventType(); type != XmlPullParser.END_DOCUMENT;
                type = parser.next()) {
            if ((type == XmlPullParser.START_TAG) && XML_TAG_MEDIAS.equals(parser.getName())) {
                AttributeSet attrs = Xml.asAttributeSet(parser);

                String version = attrs.getAttributeValue(null, XML_ATTR_VERSION);
                if (!FILE_FORMAT_VERSION.equals(version)) {
                    throw new IOException("Incompatible format");
                }
            } else if ((type == XmlPullParser.START_TAG)
                    && XML_TAG_MEDIA.equals(parser.getName())) {
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

                MediaInfo mediaInfo = new MediaInfo.Builder(url)
                        .setStreamType(MediaInfo.STREAM_TYPE_BUFFERED)
                        .setContentType(mimeType).setMetadata(metadata).build();

                list.add(mediaInfo);
            }
        }
    }

}
