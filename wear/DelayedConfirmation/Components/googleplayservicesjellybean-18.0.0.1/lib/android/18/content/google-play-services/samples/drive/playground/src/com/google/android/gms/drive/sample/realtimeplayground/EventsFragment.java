package com.google.android.gms.drive.sample.realtimeplayground;

import com.google.android.gms.drive.realtime.CollaborativeObject.ObjectChangedEvent;
import com.google.android.gms.drive.realtime.CollaborativeObjectEvent;
import com.google.android.gms.drive.realtime.RealtimeDocument;
import com.google.android.gms.drive.realtime.RealtimeEvent.Listener;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ListView;

import java.util.LinkedList;
import java.util.List;

/**
 * A fragment that displays ObjectChangedEvents
 */
public class EventsFragment extends PlaygroundFragment {

    private static final String TAG = "EventsFragment";

    private ListView mEventsListView;
    private ArrayAdapter<CollaborativeObjectEvent> mEventsArrayAdapter;
    private Listener<ObjectChangedEvent> mListener;
    private RealtimeDocument mRealtimeDocument;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle
            savedInstanceState) {
        super.onCreateView(inflater, container, savedInstanceState);
        View view = inflater.inflate(R.layout.fragment_events, container, false);
        mEventsListView = (ListView) view.findViewById(R.id.events_list);
        return view;
    }

    @Override
    void onLoaded(RealtimeDocument document) {
        PlaygroundDocumentActivity activity = (PlaygroundDocumentActivity) getActivity();

        mRealtimeDocument = document;
        mEventsArrayAdapter =
                new ArrayAdapter<>(getActivity(), android.R.layout.simple_list_item_1,
                        activity.getRecentEvents());
        mEventsListView.setAdapter(mEventsArrayAdapter);
        mListener =  new Listener<ObjectChangedEvent>() {
            @Override
            public void onEvent(ObjectChangedEvent event) {
                mEventsArrayAdapter.notifyDataSetChanged();
            }
        };
        mRealtimeDocument.getModel().getRoot().addObjectChangedListener(mListener);

    }

    @Override
    public void onStop() {
        super.onStop();
        if (mListener != null) {
            mRealtimeDocument.getModel().getRoot().removeObjectChangedListener(mListener);
        }
    }

    @Override
    public String getTitle() {
        return "Events";
    }

}
