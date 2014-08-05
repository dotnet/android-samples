// Copyright 2013 Google Inc. All Rights Reserved.

package com.google.android.gms.cast.samples.democastplayer;

import com.google.android.gms.cast.MediaTrack;

import android.app.Activity;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.ImageView;
import android.widget.TextView;

import java.util.HashSet;
import java.util.Set;

class MediaTrackAdapter extends ArrayAdapter<MediaTrack> {
    private Set<Long> mSelectedTracks = new HashSet<Long>();

    public MediaTrackAdapter(Context context) {
        super(context, R.layout.media_track_item, R.id.text);
    }

    @Override
    public View getView(final int position, View convertView, ViewGroup parent) {
        View view = convertView;

        if (view == null) {
            LayoutInflater inflater = ((Activity) getContext()).getLayoutInflater();
            view = inflater.inflate(R.layout.media_track_item, parent, false);

            CheckBox checkbox = (CheckBox) view.findViewById(R.id.checkbox);
            checkbox.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
                @Override
                public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                    MediaTrack item = getItem(position);
                    if (isChecked) {
                        mSelectedTracks.add(item.getId());
                    } else {
                        mSelectedTracks.remove(item.getId());
                    }
                }
            });
        }

        ImageView imageView = (ImageView) view.findViewById(R.id.image);
        TextView textView = (TextView) view.findViewById(R.id.text);

        MediaTrack item = getItem(position);

        switch (item.getType()) {
            case MediaTrack.TYPE_AUDIO:
                imageView.setImageResource(R.drawable.type_audio);
                break;
            case MediaTrack.TYPE_VIDEO:
                imageView.setImageResource(R.drawable.type_video);
                break;
            case MediaTrack.TYPE_TEXT:
                imageView.setImageResource(R.drawable.type_text);
                break;
            default:
                imageView.setImageBitmap(null);
        }

        textView.setText(item.getName());

        return view;
    }

    public long[] getSelectedTracks() {
        long ids[] = new long[mSelectedTracks.size()];
        int i = 0;
        for (Long selectedTrack : mSelectedTracks) {
            ids[i++] = selectedTrack;
        }
        return ids;
    }

    public void setSelectedTracks(long[] selectedTracks) {
        mSelectedTracks.clear();
        if (selectedTracks != null) {
            for (long selectedTrack : selectedTracks) {
                mSelectedTracks.add(selectedTrack);
            }
        }
        notifyDataSetChanged();
    }

    @Override
    public void clear() {
        super.clear();
        mSelectedTracks.clear();
    }

}
