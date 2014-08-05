package com.google.android.gms.drive.sample.realtimeplayground;

import com.google.android.gms.common.api.ResultCallback;
import com.google.android.gms.drive.Drive;
import com.google.android.gms.drive.DriveFile;
import com.google.android.gms.drive.DriveId;
import com.google.android.gms.drive.realtime.CollaborativeList;
import com.google.android.gms.drive.realtime.CollaborativeMap;
import com.google.android.gms.drive.realtime.CollaborativeObject.ObjectChangedEvent;
import com.google.android.gms.drive.realtime.CollaborativeObjectEvent;
import com.google.android.gms.drive.realtime.CollaborativeString;
import com.google.android.gms.drive.realtime.Model;
import com.google.android.gms.drive.realtime.Model.UndoRedoStateChangedEvent;
import com.google.android.gms.drive.realtime.RealtimeDocument;
import com.google.android.gms.drive.realtime.RealtimeDocument.ErrorEvent;
import com.google.android.gms.drive.realtime.RealtimeDocument.DocumentSaveStateChangedEvent;
import com.google.android.gms.drive.realtime.RealtimeEvent.Listener;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentPagerAdapter;
import android.support.v4.view.ViewPager;
import android.util.Log;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import java.util.ArrayList;
import java.util.List;

/**
 * An activity that displays a RealtimePlayground document.
 */
public class PlaygroundDocumentActivity extends BaseDriveActivity {
    private static final String TAG = "PlaygroundDocumentActivity";

    static final String EXTRA_DRIVE_ID = "driveId";

    static final String COLLAB_STRING_NAME = "demo_string";
    static final String COLLAB_LIST_NAME = "demo_list";
    static final String COLLAB_MAP_NAME = "demo_map";
    private static final int MAX_EVENTS = 50;

    private DriveId mDriveId;
    private PagerAdapter mAdapter;
    private RealtimeDocument mRealtimeDocument;
    private TextView mSavingTextView;
    private Listener<DocumentSaveStateChangedEvent> mSaveStateListener;
    private List<CollaborativeObjectEvent> mEvents = new ArrayList<>();

    private Button mUndoButton;
    private Button mRedoButton;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_playground_document);
        mUndoButton = (Button) findViewById(R.id.undo_button);
        mUndoButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View view) {
                if (mRealtimeDocument != null) {
                    mRealtimeDocument.getModel().undo();
                }
            }
        });
        mRedoButton = (Button) findViewById(R.id.redo_button);
        mRedoButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View view) {
                if (mRealtimeDocument != null) {
                    mRealtimeDocument.getModel().redo();
                }
            }
        });
        mSavingTextView = (TextView) findViewById(R.id.saving_text_view);
    }

    @Override
    protected void onStop() {
        super.onStop();
        mUndoButton.setEnabled(false);
        mRedoButton.setEnabled(false);
        mRealtimeDocument = null;
    }

    @Override
    protected void onClientConnected() {
        Log.i(TAG, "onClientConnected");
        if (mRealtimeDocument != null) {
            return;
        }
        mDriveId = getIntent().getParcelableExtra(EXTRA_DRIVE_ID);
        DriveFile driveFile = Drive.DriveApi.getFile(mGoogleApiClient, mDriveId);
        driveFile.loadRealtimeDocument(
                mGoogleApiClient,
                new DriveFile.InitializeRealtimeDocumentListener() {

                    @Override
                    public void onInitialize(Model model) {
                        CollaborativeMap root = model.getRoot();
                        CollaborativeString string = model.createString();
                        string.setText("Edit Me!");
                        root.put(COLLAB_STRING_NAME, string);

                        CollaborativeList list = model.createList();
                        list.add("Cat");
                        list.add("Dog");
                        list.add("Sheep");
                        list.add("Chicken");
                        root.put(COLLAB_LIST_NAME, list);

                        CollaborativeMap map = model.createMap();
                        for (int i = 1; i < 5; i++) {
                            map.put("Key " + i, "Value " + i);
                        }
                        root.put(COLLAB_MAP_NAME, map);
                    }
                },
                null).setResultCallback(new ResultCallback<DriveFile.RealtimeLoadResult>() {
            @Override
            public void onResult(DriveFile.RealtimeLoadResult result) {
                if (result.getStatus().isSuccess()) {
                    mRealtimeDocument = result.getRealtimeDocument();
                    mRealtimeDocument.addErrorListener(new Listener<ErrorEvent>() {
                        @Override
                        public void onEvent(ErrorEvent event) {
                            Toast.makeText(
                                    PlaygroundDocumentActivity.this,
                                    "Realtime error: " + event.getError(),
                                    Toast.LENGTH_LONG).show();
                        }
                    });
                    onLoaded();
                } else {
                    Toast.makeText(
                            PlaygroundDocumentActivity.this,
                            "Failed to load Realtime document " + result.getStatus(),
                            Toast.LENGTH_LONG).show();
                }
            }
        });
    }

    private void onLoaded() {
        Model model = mRealtimeDocument.getModel();

        mUndoButton.setEnabled(model.canUndo());
        mRedoButton.setEnabled(model.canRedo());
        model.addUndoRedoStateChangedListener(new Listener<UndoRedoStateChangedEvent>() {
            @Override
            public void onEvent(UndoRedoStateChangedEvent event) {
                mUndoButton.setEnabled(event.canUndo());
                mRedoButton.setEnabled(event.canRedo());
            }
        });

        ViewPager pager = (ViewPager) findViewById(R.id.pager);
        mAdapter = new PagerAdapter(getSupportFragmentManager());
        pager.setAdapter(mAdapter);

        mSaveStateListener = new Listener<DocumentSaveStateChangedEvent>() {
            @Override
            public void onEvent(DocumentSaveStateChangedEvent event) {
                if (event.isPending()) {
                    mSavingTextView.setVisibility(View.VISIBLE);
                    mSavingTextView.setText("Waiting to save..");
                } else if(event.isSaving()) {
                    mSavingTextView.setVisibility(View.VISIBLE);
                    mSavingTextView.setText("Saving..");
                } else {
                    mSavingTextView.setVisibility(View.INVISIBLE);
                }

            }
        };
        mRealtimeDocument.addDocumentSaveStateChangedListener(mSaveStateListener);
        Listener<ObjectChangedEvent> listener = new Listener<ObjectChangedEvent>() {
            @Override
            public void onEvent(ObjectChangedEvent event) {
                Log.d(TAG, "Got event " + event);
                mEvents.addAll(0, event.getCauses());
                while (mEvents.size() > MAX_EVENTS) {
                    mEvents.remove(mEvents.size() - 1);
                }
            }
        };
        mRealtimeDocument.getModel().getRoot().addObjectChangedListener(listener);
    }

    public List<CollaborativeObjectEvent> getRecentEvents() {
        return mEvents;
    }

    @Override
    protected void onPause() {
        super.onPause();
        if (mSaveStateListener != null) {
            mRealtimeDocument.removeDocumentSaveStateChangedListener(mSaveStateListener);
        }
    }

    /**
     * Gets the open RealtimeDocument.
     */
    public RealtimeDocument getRealtimeDocument() {
        return mRealtimeDocument;
    }

    private static class PagerAdapter extends FragmentPagerAdapter {
        private final List<PlaygroundFragment> mFragments;

        public PagerAdapter(FragmentManager fm) {
            super(fm);
            mFragments = new ArrayList<>();
            mFragments.add(new CollaborativeStringFragment());
            mFragments.add(new CollaborativeListFragment());
            mFragments.add(new CollaborativeMapFragment());
            mFragments.add(new CollaboratorsFragment());
            mFragments.add(new EventsFragment());
        }

        @Override
        public Fragment getItem(int i) {
            return mFragments.get(i);
        }

        @Override
        public int getCount() {
            return mFragments.size();
        }

        @Override
        public CharSequence getPageTitle(int position) {
            return mFragments.get(position).getTitle();
        }
    }
}
