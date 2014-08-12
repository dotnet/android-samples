// Copyright 2013 Google Inc. All Rights Reserved.

package com.google.android.gms.cast.samples.democastplayer;

import com.google.android.gms.cast.MediaInfo;
import com.google.android.gms.cast.MediaMetadata;

import android.app.Activity;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ImageView;
import android.widget.TextView;

class MediaAdapter extends ArrayAdapter<MediaInfo> {

    public MediaAdapter(Context context) {
        super(context, R.layout.media_item, R.id.text);
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        View view = convertView;

        if (view == null) {
            LayoutInflater inflater = ((Activity) getContext()).getLayoutInflater();
            view = inflater.inflate(R.layout.media_item, parent, false);
        }

        ImageView imageView = (ImageView) view.findViewById(R.id.image);
        TextView textView = (TextView) view.findViewById(R.id.text);

        MediaInfo item = getItem(position);
        MediaMetadata metadata = item.getMetadata();
        if (metadata == null) {
            textView.setText("Unsupported item");
            imageView.setImageBitmap(null);
        } else {
            switch (metadata.getMediaType()) {
                case MediaMetadata.MEDIA_TYPE_PHOTO:
                    imageView.setImageResource(R.drawable.type_image);
                    break;
                case MediaMetadata.MEDIA_TYPE_MUSIC_TRACK:
                    imageView.setImageResource(R.drawable.type_audio);
                    break;
                case MediaMetadata.MEDIA_TYPE_MOVIE:
                case MediaMetadata.MEDIA_TYPE_TV_SHOW:
                    imageView.setImageResource(R.drawable.type_video);
                    break;
                default:
                    imageView.setImageBitmap(null);
            }

            textView.setText(metadata.getString(MediaMetadata.KEY_TITLE));
        }

        return view;
    }

}
