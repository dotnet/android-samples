package com.google.android.gms.drive.sample.realtimeplayground;

import com.google.android.gms.drive.realtime.Collaborator;
import com.google.android.gms.drive.realtime.RealtimeDocument;
import com.google.android.gms.drive.realtime.RealtimeEvent;

import android.content.Context;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;

import java.util.List;

/**
 * A fragment that displays the active collaborators.
 */
public class CollaboratorsFragment extends PlaygroundFragment {
    private ListView mCollaboratorList;
    private List<Collaborator> mCollaborators;
    private RealtimeDocument mRealtimeDocument;
    private CollaboratorsAdapter mAdapter;
    private TextView mReadonlyTextView;

    private RealtimeEvent.Listener<RealtimeDocument.CollaboratorJoinedEvent> mJoinListener;
    private RealtimeEvent.Listener<RealtimeDocument.CollaboratorLeftEvent> mLeftListener;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle
            savedInstanceState) {
        super.onCreateView(inflater, container, savedInstanceState);
        View view = inflater.inflate(R.layout.fragment_collaborators, container, false);
        mCollaboratorList = (ListView) view.findViewById(R.id.collaborators_list);
        mReadonlyTextView = (TextView) view.findViewById(R.id.readonly_text_view);
        return view;
    }

    @Override
    public String getTitle() {
        return "Collaborators";
    }

    @Override
    void onLoaded(RealtimeDocument document) {
        mRealtimeDocument = document;
        mCollaborators = mRealtimeDocument.getCollaborators();
        mAdapter = new CollaboratorsAdapter(getActivity(), mCollaborators);
        mCollaboratorList.setAdapter(mAdapter);
        mJoinListener = new RealtimeEvent.Listener<RealtimeDocument.CollaboratorJoinedEvent>() {
            @Override
            public void onEvent(RealtimeDocument.CollaboratorJoinedEvent event) {
                mCollaborators.add(event.getCollaborator());
                mAdapter.notifyDataSetChanged();
            }
        };
        mRealtimeDocument.addCollaboratorJoinedListener(mJoinListener);

        mLeftListener = new RealtimeEvent.Listener<RealtimeDocument.CollaboratorLeftEvent>() {
            @Override
            public void onEvent(RealtimeDocument.CollaboratorLeftEvent event) {
                mCollaborators.remove(event.getCollaborator());
                mAdapter.notifyDataSetChanged();
            }
        };
        mRealtimeDocument.addCollaboratorLeftListener(mLeftListener);

        String modeText = mRealtimeDocument.getModel().isReadonly() ? "View" : "Edit";
        mReadonlyTextView.setText(modeText);
    }

    @Override
    public void onStop() {
        super.onStop();
        try {
            if (mJoinListener != null) {
                mRealtimeDocument.removeCollaboratorJoinedListener(mJoinListener);
            }
            if (mLeftListener != null) {
                mRealtimeDocument.removeCollaboratorLeftListener(mLeftListener);
            }
        } catch (RuntimeException e) {
            // Ignore failures while pausing
        }
    }

    private static class CollaboratorsAdapter extends ArrayAdapter<Collaborator> {
        public CollaboratorsAdapter(
                Context context, List<Collaborator> collaborators) {
            super(context, R.layout.row_collaborator, R.id.name_text_view, collaborators);
        }

        @Override
        public View getView(int i, View convertView, ViewGroup viewGroup) {
            convertView = super.getView(i, convertView, viewGroup);
            TextView name = (TextView) convertView.findViewById(R.id.name_text_view);
            ImageView photo = (ImageView) convertView.findViewById(R.id.photo_image_view);

            Collaborator collaborator = getItem(i);
            name.setText(getName(collaborator));
            name.setTextColor(collaborator.getColor());

            // TODO: actually populate the photo. This is complicated since it requires downloading
            // all the images in a background thread.
            return convertView;
        }

        /**
         * Gets the display name with indicator of "me" and "anonymous" state.
         */
        private String getName(Collaborator collaborator) {
            String name = collaborator.getDisplayName();
            if (collaborator.isMe()) {
                name = name + " (me)";
            }
            if (collaborator.isAnonymous()) {
                name = name + "*";
            }
            return name;
        }

    }
}
