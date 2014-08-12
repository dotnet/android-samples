package com.google.android.gms.drive.sample.realtimeplayground;

import com.google.android.gms.drive.realtime.CollaborativeList;
import com.google.android.gms.drive.realtime.RealtimeDocument;
import com.google.android.gms.drive.realtime.RealtimeEvent;

import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ListView;
import android.widget.Toast;

/**
 * A fragment that displays the sample CollaborativeList.
 */
public class CollaborativeListFragment extends PlaygroundFragment {
    private static final String TAG = "CollaborativeListFragment";

    private RealtimeDocument mRealtimeDocument;
    private CollaborativeList mCollaborativeList;

    private ArrayAdapter<Object> mItemsArrayAdapter;

    private RealtimeEvent.Listener<CollaborativeList.ValuesSetEvent> mValuesSetListener;
    private RealtimeEvent.Listener<CollaborativeList.ValuesAddedEvent> mValuesAddedListener;
    private RealtimeEvent.Listener<CollaborativeList.ValuesRemovedEvent> mValuesRemovedListener;

    private ListView mItemsList;
    private EditText mAddItemEditText;
    private EditText mSetItemEditText;
    private EditText mNewIndexEditText;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle
            savedInstanceState) {
        super.onCreateView(inflater, container, savedInstanceState);
        View view = inflater.inflate(R.layout.fragment_collaborative_list, container, false);
        Button removeButton = (Button) view.findViewById(R.id.remove_item_button);
        removeButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                doRemoveSelected();
            }
        });

        Button addButton = (Button) view.findViewById(R.id.add_item_button);
        addButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                doAddItem();
            }
        });

        Button clearButton = (Button) view.findViewById(R.id.clear_button);
        clearButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                doClearList();
            }
        });

        Button setButton = (Button) view.findViewById(R.id.set_item_button);
        setButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                doSetItem();
            }
        });

        Button moveButton = (Button) view.findViewById(R.id.move_button);
        moveButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                doMoveItem();
            }
        });

        mAddItemEditText = (EditText) view.findViewById(R.id.add_item_edit_text);
        mSetItemEditText = (EditText) view.findViewById(R.id.set_item_edit_text);
        mNewIndexEditText = (EditText) view.findViewById(R.id.new_index_edit_text);
        mItemsList = (ListView) view.findViewById(R.id.items_list);

        disableViewUntilLoaded(removeButton);
        disableViewUntilLoaded(addButton);
        disableViewUntilLoaded(clearButton);
        disableViewUntilLoaded(setButton);
        disableViewUntilLoaded(moveButton);
        disableViewUntilLoaded(mAddItemEditText);
        disableViewUntilLoaded(mSetItemEditText);
        disableViewUntilLoaded(mItemsList);

        return view;
    }

    private void doRemoveSelected() {
        if (mItemsList.getCheckedItemPosition() != AdapterView.INVALID_POSITION) {
            Log.d(TAG, "removing " + mItemsList.getCheckedItemPosition());
            mCollaborativeList.remove(mItemsList.getCheckedItemPosition());
        }
    }

    private void doAddItem() {
        if (mAddItemEditText.length() > 0) {
            mCollaborativeList.add(mAddItemEditText.getText().toString());
            mAddItemEditText.setText("");
        }
    }

    private void doClearList() {
        mCollaborativeList.clear();
    }

    private void doSetItem() {
        if (mItemsList.getCheckedItemPosition() != AdapterView.INVALID_POSITION
                && mSetItemEditText.length() > 0) {
            mCollaborativeList.set(
                    mItemsList.getCheckedItemPosition(), mSetItemEditText.getText().toString());
        }
    }

    private void doMoveItem() {
        if (mItemsList.getCheckedItemPosition() != AdapterView.INVALID_POSITION) {
            int newIndex = Integer.parseInt(mNewIndexEditText.getText().toString());
            if (newIndex >= 0 && newIndex <= mCollaborativeList.size()) {
                mCollaborativeList.move(mItemsList.getCheckedItemPosition(), newIndex);
            } else {
                Toast.makeText(getActivity(), "Invalid index.", Toast.LENGTH_LONG).show();
            }
        }
    }

    @Override
    public String getTitle() {
        return "Collaborative List";
    }

    @Override
    void onLoaded(RealtimeDocument document) {
        mRealtimeDocument = document;
        mCollaborativeList =
                (CollaborativeList) mRealtimeDocument.getModel().getRoot().get(
                        PlaygroundDocumentActivity.COLLAB_LIST_NAME);
        mItemsArrayAdapter = new ArrayAdapter<Object>(
                getActivity(), android.R.layout.simple_list_item_single_choice, mCollaborativeList);
        mItemsList.setAdapter(mItemsArrayAdapter);
        // TODO: listen for events
        mValuesAddedListener = new RealtimeEvent.Listener<CollaborativeList.ValuesAddedEvent>() {
            @Override
            public void onEvent(CollaborativeList.ValuesAddedEvent event) {
                mItemsArrayAdapter.notifyDataSetChanged();
            }
        };
        mCollaborativeList.addValuesAddedListener(mValuesAddedListener);
        mValuesRemovedListener =
                new RealtimeEvent.Listener<CollaborativeList.ValuesRemovedEvent>() {
                    @Override
                    public void onEvent(CollaborativeList.ValuesRemovedEvent event) {
                        mItemsArrayAdapter.notifyDataSetChanged();
                    }
                };
        mCollaborativeList.addValuesRemovedListener(mValuesRemovedListener);
        mValuesSetListener = new RealtimeEvent.Listener<CollaborativeList.ValuesSetEvent>() {
            @Override
            public void onEvent(CollaborativeList.ValuesSetEvent event) {
                mItemsArrayAdapter.notifyDataSetChanged();
            }
        };
        mCollaborativeList.addValuesSetListener(mValuesSetListener);
    }

    @Override
    public void onStop() {
        super.onStop();
        if (mValuesAddedListener != null) {
            mCollaborativeList.removeValuesAddedListener(mValuesAddedListener);
        }
        if (mValuesSetListener != null) {
            mCollaborativeList.removeValuesSetListener(mValuesSetListener);
        }
        if (mValuesRemovedListener != null) {
            mCollaborativeList.removeValuesRemovedListener(mValuesRemovedListener);
        }
    }
}
