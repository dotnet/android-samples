// Copyright 2013 Google Inc. All Rights Reserved.

package com.google.android.gms.cast.samples.democastplayer;

import android.content.DialogInterface;
import android.os.AsyncTask;
import android.os.Bundle;
import android.support.v4.app.DialogFragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.ProgressBar;

import java.util.List;

/**
 * Abstract base class for a dialog that displays a lazily-loaded list of items
 * for single-selection.
 *
 * @param <T> The type of object being listed.
 */
public abstract class ListSelectionDialog<T> extends DialogFragment {
    private final String mTitle;
    private ListView mListView;
    private ArrayAdapter<T> mListAdapter;
    private LoadListTask mLoadListTask;
    private ProgressBar mProgressBar;
    private View mEmptyView;
    private boolean mListLoaded;

    public ListSelectionDialog(String title) {
        mTitle = title;
    }

    @Override
    public void onActivityCreated(Bundle savedState) {
        super.onActivityCreated(savedState);

        getDialog().setTitle(mTitle);
        setStyle(DialogFragment.STYLE_NORMAL, android.R.style.Theme_Holo_Dialog_MinWidth);
        if (mListAdapter == null) {
            mListAdapter = buildAdapter();
        }
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.list_selection_dialog, null);

        mListView = (ListView) view.findViewById(R.id.list_view);

        mListView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
                @Override
                public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                    T item = mListAdapter.getItem(position);
                    if (item != null) {
                        onItemSelected(item);
                    }
                    dismiss();
                }
            });

        mProgressBar = (ProgressBar) view.findViewById(R.id.progress_bar);
        mEmptyView = view.findViewById(R.id.empty);

        mListView.setEmptyView(mProgressBar);

        return view;
    }

    @Override
    public void onStart() {
        super.onStart();
        if (!mListLoaded) {
            mProgressBar.setVisibility(View.VISIBLE);
            mListView.setEmptyView(mProgressBar);
            mLoadListTask = new LoadListTask();
            mLoadListTask.execute();
        } else {
            mListView.setAdapter(mListAdapter);
        }
    }

    @Override
    public void onStop() {
        if (mLoadListTask != null) {
            mLoadListTask.cancel(false);
            mLoadListTask = null;
        }

        super.onStop();
    }

    @Override
    public void onCancel(DialogInterface dialog) {
        onCanceled();
        super.onCancel(dialog);
    }

    public void invalidateData() {
        mListLoaded = false;
        mListAdapter.clear();
    }

    private class LoadListTask extends AsyncTask<Void, Void, List<T>> {

        @Override
        protected List<T> doInBackground(Void... params) {
            return loadItems();
        }

        @Override
        protected void onPostExecute(List<T> result) {
            mListLoaded = true;
            if (result != null) {
                for (T item : result) {
                    mListAdapter.add(item);
                }
            }
            mProgressBar.setVisibility(View.GONE);
            mListView.setEmptyView(mEmptyView);
            mListView.setAdapter(mListAdapter);

            mLoadListTask = null;
        }

        @Override
        protected void onCancelled(List<T> result) {
            mProgressBar.setVisibility(View.GONE);
            mLoadListTask = null;
        }
    }

    /**
     * Called to construct an (empty) adapter for the list.
     */
    protected abstract ArrayAdapter<T> buildAdapter();

    /**
     * Called from an async task to load the list of items to be displayed.
     */
    protected abstract List<T> loadItems();

    /**
     * Called when the user selects an item.
     */
    protected abstract void onItemSelected(final T item);

    /**
     * Called when the dialog is canceled.
     */
    protected void onCanceled() { }
}

