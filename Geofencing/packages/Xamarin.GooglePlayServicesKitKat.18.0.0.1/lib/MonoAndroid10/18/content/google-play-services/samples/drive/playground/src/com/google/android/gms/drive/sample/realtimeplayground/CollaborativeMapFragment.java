package com.google.android.gms.drive.sample.realtimeplayground;

import com.google.android.gms.drive.realtime.CollaborativeMap;
import com.google.android.gms.drive.realtime.RealtimeDocument;
import com.google.android.gms.drive.realtime.RealtimeEvent;

import android.content.Context;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ListView;
import android.widget.TextView;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

/**
 * A fragment that displays the sample CollaborativeMap.
 */
public class CollaborativeMapFragment extends PlaygroundFragment {

    private static final String TAG = "CollaborativeMapFragment";

    private RealtimeDocument mRealtimeDocument;
    private CollaborativeMap mCollaborativeMap;

    private EditText mKeyEditText;
    private EditText mValueEditText;
    private ListView mItemsList;
    private MapAdapter mMapAdapter;

    private RealtimeEvent.Listener<CollaborativeMap.ValueChangedEvent> mListener;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle
            savedInstanceState) {
        super.onCreateView(inflater, container, savedInstanceState);
        View view = inflater.inflate(R.layout.fragment_collaborative_map, container, false);
        ImageButton removeButton = (ImageButton) view.findViewById(R.id.remove_item_button);
        removeButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                doRemoveSelected();
            }
        });

        ImageButton clearButton = (ImageButton) view.findViewById(R.id.clear_button);
        clearButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                doClearList();
            }
        });

        ImageButton setButton = (ImageButton) view.findViewById(R.id.set_item_button);
        setButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                doSetItem();
            }
        });

        mKeyEditText = (EditText) view.findViewById(R.id.key_edit_text);
        mValueEditText = (EditText) view.findViewById(R.id.value_edit_text);
        mItemsList = (ListView) view.findViewById(R.id.items_list);

        disableViewUntilLoaded(removeButton);
        disableViewUntilLoaded(clearButton);
        disableViewUntilLoaded(setButton);
        disableViewUntilLoaded(mKeyEditText);
        disableViewUntilLoaded(mValueEditText);
        disableViewUntilLoaded(mItemsList);

        return view;
    }

    private void doRemoveSelected() {
        if (mItemsList.getCheckedItemPosition() != AdapterView.INVALID_POSITION) {
            Log.d(TAG, "removing " + mItemsList.getCheckedItemPosition());
            Map.Entry<String, Object> entry =
                    mMapAdapter.getItem(mItemsList.getCheckedItemPosition());
            mCollaborativeMap.remove(entry.getKey());
        }
    }

    private void doClearList() {
        mCollaborativeMap.clear();
    }

    private void doSetItem() {
        if (mKeyEditText.length() > 0 && mValueEditText.length() > 0) {
            mCollaborativeMap.put(
                    mKeyEditText.getText().toString(), mValueEditText.getText().toString());
        }
    }

    @Override
    public String getTitle() {
        return "Collaborative Map";
    }

    @Override
    void onLoaded(RealtimeDocument document) {
        mRealtimeDocument = document;
        mCollaborativeMap =
                (CollaborativeMap) mRealtimeDocument.getModel().getRoot().get(
                        PlaygroundDocumentActivity.COLLAB_MAP_NAME);
        mMapAdapter = new MapAdapter(
                getActivity(), android.R.layout.simple_list_item_single_choice, mCollaborativeMap);
        mItemsList.setAdapter(mMapAdapter);

        mListener = new RealtimeEvent.Listener<CollaborativeMap.ValueChangedEvent>() {
            @Override
            public void onEvent(CollaborativeMap.ValueChangedEvent event) {
                mMapAdapter.notifyDataSetChanged();
            }
        };
        mCollaborativeMap.addValueChangedListener(mListener);
    }

    @Override
    public void onStop() {
        super.onStop();
        if (mListener != null) {
            mCollaborativeMap.removeValueChangedListener(mListener);
        }
    }

    private static class MapAdapter extends ArrayAdapter<Map.Entry<String, Object>> {
        private final List<Map.Entry<String, Object>> mEntries;
        private final CollaborativeMap mMap;
        private final Context mContext;

        public MapAdapter(Context context, int resource, CollaborativeMap map) {
            super(context, resource);
            mContext = context;
            mMap = map;
            mEntries = new ArrayList<Map.Entry<String, Object>>(mMap.size());
            mEntries.addAll(mMap.entrySet());
        }

        @Override
        public void notifyDataSetChanged() {
            super.notifyDataSetChanged();
            mEntries.clear();
            mEntries.addAll(mMap.entrySet());
        }

        @Override
        public int getCount() {
            return mEntries.size();
        }

        @Override
        public Map.Entry<String, Object> getItem(int i) {
            return mEntries.get(i);
        }

        @Override
        public long getItemId(int i) {
            return i;
        }

        @Override
        public View getView(int i, View view, ViewGroup viewGroup) {
            TextView result = (TextView) super.getView(i, view, viewGroup);

            Map.Entry<String, Object> entry = mEntries.get(i);
            result.setText(entry.getKey() + " -> " + entry.getValue());
            return result;
        }

    }
}
