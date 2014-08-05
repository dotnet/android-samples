/*
 * Copyright (C) 2013 Google Inc. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package com.google.android.gms.cast.samples.tictactoe;

import com.google.android.gms.cast.ApplicationMetadata;
import com.google.android.gms.cast.Cast;
import com.google.android.gms.cast.Cast.ApplicationConnectionResult;
import com.google.android.gms.cast.CastDevice;
import com.google.android.gms.cast.CastMediaControlIntent;
import com.google.android.gms.cast.samples.tictactoe.GameView.ICellListener;
import com.google.android.gms.cast.samples.tictactoe.GameView.State;
import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.GooglePlayServicesUtil;
import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.common.api.ResultCallback;
import com.google.android.gms.common.api.Status;

import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.pm.ActivityInfo;
import android.content.res.Configuration;
import android.os.Bundle;
import android.support.v4.view.MenuItemCompat;
import android.support.v7.app.ActionBarActivity;
import android.support.v7.app.MediaRouteActionProvider;
import android.support.v7.media.MediaRouteSelector;
import android.support.v7.media.MediaRouter;
import android.support.v7.media.MediaRouter.RouteInfo;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;

import java.io.IOException;

/**
 * An activity which both presents a UI on the first screen and casts the TicTacToe game board to
 * the selected Cast device and its attached second screen.
 */
public class GameActivity extends ActionBarActivity {
    private static final String TAG = GameActivity.class.getSimpleName();
    private static final int REQUEST_GMS_ERROR = 0;

    private static final String APP_ID = "BFEBD3F1";

    private Button mJoinGameButton;
    private GameView mGameView;
    private TextView mInfoView;
    private TextView mPlayerNameView;

    private CastDevice mSelectedDevice;
    private GoogleApiClient mApiClient;
    private Cast.Listener mCastListener;
    private ConnectionCallbacks mConnectionCallbacks;
    private ConnectionFailedListener mConnectionFailedListener;
    private MediaRouter mMediaRouter;
    private MediaRouteSelector mMediaRouteSelector;
    private MediaRouter.Callback mMediaRouterCallback;
    private TicTacToeChannel mGameChannel;

    /**
     * Called when the activity is first created. Initializes the game with necessary listeners
     * for player interaction, and creates a new cast channel.
     */
    @Override
    public void onCreate(Bundle bundle) {
        super.onCreate(bundle);
        setContentView(R.layout.game);

        mJoinGameButton = (Button) findViewById(R.id.join_game);
        mGameView = (GameView) findViewById(R.id.game_view);
        mInfoView = (TextView) findViewById(R.id.info_turn);
        mPlayerNameView = (TextView) findViewById(R.id.player_name);

        mGameView.setFocusable(true);
        mGameView.setFocusableInTouchMode(true);
        mGameView.setCellListener(new CellListener());

        mGameChannel = new TicTacToeChannel();

        mMediaRouter = MediaRouter.getInstance(getApplicationContext());
        mMediaRouteSelector = new MediaRouteSelector.Builder()
                .addControlCategory(CastMediaControlIntent.categoryForCast(APP_ID))
                .build();

        mMediaRouterCallback = new MediaRouterCallback();
        mCastListener = new CastListener();
        mConnectionCallbacks = new ConnectionCallbacks();
        mConnectionFailedListener = new ConnectionFailedListener();

        mJoinGameButton.setEnabled(false);
        mJoinGameButton.setOnClickListener(new View.OnClickListener() {

            @Override
            public void onClick(View view) {
                if (mApiClient != null) {
                    mGameChannel.join(mApiClient, "MyName");
                    mInfoView.setText(R.string.waiting_for_player_assignment);
                    mJoinGameButton.setEnabled(false);
                }
            }
        });
    }

    /**
     * Called when the options menu is first created.
     */
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        super.onCreateOptionsMenu(menu);
        getMenuInflater().inflate(R.menu.main, menu);
        MenuItem mediaRouteMenuItem = menu.findItem(R.id.media_route_menu_item);
        MediaRouteActionProvider mediaRouteActionProvider =
                (MediaRouteActionProvider) MenuItemCompat.getActionProvider(mediaRouteMenuItem);
        mediaRouteActionProvider.setRouteSelector(mMediaRouteSelector);
        return true;
    }

    /**
     * Called on application start. Using the previously selected Cast device, attempts to begin a
     * session using the application name TicTacToe.
     */
    @Override
    protected void onStart() {
        super.onStart();
        mMediaRouter.addCallback(mMediaRouteSelector, mMediaRouterCallback,
                MediaRouter.CALLBACK_FLAG_PERFORM_ACTIVE_SCAN);
    }

    @Override
    protected void onResume() {
        super.onResume();
        int errorCode = GooglePlayServicesUtil.isGooglePlayServicesAvailable(this);
        if (errorCode != ConnectionResult.SUCCESS) {
            GooglePlayServicesUtil.getErrorDialog(errorCode, this, REQUEST_GMS_ERROR).show();
        }
    }

    /**
     * Removes the activity from memory when the activity is paused.
     */
    @Override
    protected void onPause() {
        finish();
        super.onPause();
    }

    /**
     * Attempts to end the current game session when the activity stops.
     */
    @Override
    protected void onStop() {
        setSelectedDevice(null);
        mMediaRouter.removeCallback(mMediaRouterCallback);
        super.onStop();
    }

    /**
     * Returns the screen configuration to portrait mode whenever changed.
     */
    @Override
    public void onConfigurationChanged(Configuration newConfig) {
        super.onConfigurationChanged(newConfig);
        setRequestedOrientation(ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);
    }

    /**
     * Returns the string representation of a State object representing a player, or null if the
     * passed player does not correspond to an X or O player.
     */
    private String convertGameStateToPlayer(State player) {
        if (player == State.PLAYER_X) {
            return GameChannel.PLAYER_X;
        }
        if (player == State.PLAYER_O) {
            return GameChannel.PLAYER_O;
        }
        return null;
    }

    /**
     * Builds and displays a dialog indicating the completion of the game, whether by forfeit or
     * by one player winning.
     */
    private void setFinished(
            State player, int row, int column, int diagonal, boolean wasAbandoned) {
        String text;
        if (wasAbandoned) {
            if (mGameView.getAssignedPlayer() == State.EMPTY) {
                text = getString(R.string.other_players_abandoned);
            } else {
                text = getString(R.string.abandoned);
            }
        } else if (player == State.EMPTY) {
            text = getString(R.string.tie);
        } else {
            text = String.format(getResources().getString(R.string.player_wins),
                    convertGameStateToPlayer(player));
        }
        mGameView.setFinished(row, column, diagonal);

        new AlertDialog.Builder(GameActivity.this)
                .setTitle(R.string.game_over)
                .setMessage(text)
                .setCancelable(false)
                .setPositiveButton(R.string.play_again, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int id) {
                        mPlayerNameView.setText(null);
                        mGameChannel.join(mApiClient, "MyName");
                        mInfoView.setText(R.string.waiting_for_player_assignment);
                    }
                })
                .setNegativeButton(R.string.leave, new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int id) {
                        finish();
                    }
                })
                .create()
                .show();
    }

    private void setSelectedDevice(CastDevice device) {
        Log.d(TAG, "setSelectedDevice: " + device);
        mSelectedDevice = device;

        if (mSelectedDevice != null) {
            try {
                disconnectApiClient();
                connectApiClient();
            } catch (IllegalStateException e) {
                Log.w(TAG, "Exception while connecting API client", e);
                disconnectApiClient();
            }
        } else {
            if (mApiClient != null) {
                if (mApiClient.isConnected()) {
                    mGameChannel.leave(mApiClient);
                }
                disconnectApiClient();
            }
            mJoinGameButton.setEnabled(false);

            mPlayerNameView.setText(null);
            mInfoView.setText(R.string.select_device_text);
            mMediaRouter.selectRoute(mMediaRouter.getDefaultRoute());
        }
    }

    private void connectApiClient() {
        Cast.CastOptions apiOptions = Cast.CastOptions.builder(mSelectedDevice, mCastListener)
                .build();
        mApiClient = new GoogleApiClient.Builder(this)
                .addApi(Cast.API, apiOptions)
                .addConnectionCallbacks(mConnectionCallbacks)
                .addOnConnectionFailedListener(mConnectionFailedListener)
                .build();
        mApiClient.connect();
    }

    private void disconnectApiClient() {
        if (mApiClient != null) {
            mApiClient.disconnect();
            mApiClient = null;
        }
    }

    /**
     * Called when a user selects a route.
     */
    private void onRouteSelected(RouteInfo route) {
        Log.d(TAG, "onRouteSelected: " + route.getName());

        CastDevice device = CastDevice.getFromBundle(route.getExtras());
        setSelectedDevice(device);
    }

    /**
     * Called when a user unselects a route.
     */
    private void onRouteUnselected(RouteInfo route) {
        Log.d(TAG, "onRouteUnselected: " + route.getName());
        setSelectedDevice(null);
    }

    /**
     * A class which listens for the selection of a certain cell and attempts to place a mark in
     * that cell.
     */
    private class CellListener implements ICellListener {
        @Override
        public void onCellSelected(int row, int column) {
            if ((mGameView.getAssignedPlayer() == State.PLAYER_O)
                    || (mGameView.getAssignedPlayer() == State.PLAYER_X)) {
                mGameChannel.move(mApiClient, row, column);
            }
        }
    }

    /**
     * An extension of the GameChannel specifically for the TicTacToe game.
     */
    private class TicTacToeChannel extends GameChannel {
        /**
         * Sets displays accordingly when a new player joins the game.
         *
         * @param playerSymbol either X or O
         * @param opponentName the name of the player who just joined an existing game
         */
        @Override
        protected void onGameJoined(String playerSymbol, String opponentName) {
            State newPlayer = State.EMPTY;
            if (GameChannel.PLAYER_X.equals(playerSymbol)) {
                newPlayer = State.PLAYER_X;
            } else if (GameChannel.PLAYER_O.equals(playerSymbol)) {
                newPlayer = State.PLAYER_O;
            }

            mGameView.setAssignedPlayer(newPlayer);
            mPlayerNameView.setText(
                    String.format(getResources().getString(R.string.player_name), playerSymbol));
            mInfoView.setText(String.format(
                    getResources().getString(R.string.player_turn), GameChannel.PLAYER_X));
        }

        /**
         * Updates the game display upon a move.
         */
        @Override
        protected void onGameMove(String playerSymbol, int row, int column, boolean isGameOver) {
            State player = State.PLAYER_O;
            String otherPlayerName = GameChannel.PLAYER_X;
            if (GameChannel.PLAYER_X.equals(playerSymbol)) {
                player = State.PLAYER_X;
                otherPlayerName = GameChannel.PLAYER_O;
            }

            mGameView.setCell(row, column, player);
            mInfoView.setText(
                    String.format(getResources().getString(R.string.player_turn), otherPlayerName));
        }

        /**
         * At the end of the game, obtains the winning player or whether the game was forfeited, and
         * if a player won, which board position was the winning cell. Passes this information to
         * {@code setFinished()}.
         */
        @Override
        protected void onGameEnd(String endState, int location) {
            State winningPlayer;
            boolean wasGameAbandoned = false;
            if (END_STATE_X_WON.equals(endState)) {
                winningPlayer = State.PLAYER_X;
            } else if (END_STATE_O_WON.equals(endState)) {
                winningPlayer = State.PLAYER_O;
            } else if (END_STATE_ABANDONED.equals(endState)) {
                winningPlayer = mGameView.getAssignedPlayer();
                wasGameAbandoned = true;
            } else {
                winningPlayer = State.EMPTY;
            }

            int winningRow = -1;
            int winningColumn = -1;
            int winningDiagonal = -1;
            if ((location >= WinningLocation.ROW_0.getValue())
                    && (location <= WinningLocation.ROW_2.getValue())) {
                winningRow = location;
            } else if ((location >= WinningLocation.COL_0.getValue())
                    && (location <= WinningLocation.COL_2.getValue())) {
                winningColumn = location - WinningLocation.COL_0.getValue();
            } else if (location == WinningLocation.DIAGONAL_TOPLEFT.getValue()) {
                winningDiagonal = 0;
            } else if (location == WinningLocation.DIAGONAL_BOTTOMLEFT.getValue()) {
                winningDiagonal = 1;
            }

            setFinished(
                    winningPlayer, winningRow, winningColumn, winningDiagonal, wasGameAbandoned);
        }

        /**
         * Updates the game board's layout based on a passed 2-D int array.
         */
        @Override
        protected void onGameBoardLayout(int[][] boardLayout) {
            mGameView.updateBoard(boardLayout);
        }

        /**
         * Clears the game board upon a game error being detected, and displays an error dialog.
         */
        @Override
        protected void onGameError(String errorMessage) {
            mJoinGameButton.setEnabled(false);
            if (getResources().getString(R.string.full_game).equals(errorMessage)) {
                mPlayerNameView.setText(R.string.full_game);
                mInfoView.setText(R.string.observing);
                mGameView.clearBoard();
                mGameView.setAssignedPlayer(State.EMPTY);
                mGameChannel.requestBoardLayout(mApiClient);
            }

            new AlertDialog.Builder(GameActivity.this)
                    .setTitle(R.string.error)
                    .setMessage(errorMessage)
                    .setCancelable(false)
                    .setPositiveButton(R.string.ok, new DialogInterface.OnClickListener() {
                        @Override
                        public void onClick(DialogInterface dialog, int id) {
                            dialog.cancel();
                        }
                    })
                    .create()
                    .show();
        }
    }

    /**
     * An extension of the MediaRoute.Callback specifically for the TicTacToe game.
     */
    private class MediaRouterCallback extends MediaRouter.Callback {
        @Override
        public void onRouteSelected(MediaRouter router, RouteInfo route) {
            Log.d(TAG, "onRouteSelected: " + route);
            GameActivity.this.onRouteSelected(route);
        }

        @Override
        public void onRouteUnselected(MediaRouter router, RouteInfo route) {
            Log.d(TAG, "onRouteUnselected: " + route);
            GameActivity.this.onRouteUnselected(route);
        }
    }

    private class CastListener extends Cast.Listener {
        @Override
        public void onApplicationDisconnected(int statusCode) {
            Log.d(TAG, "Cast.Listener.onApplicationDisconnected: " + statusCode);
            try {
                Cast.CastApi.removeMessageReceivedCallbacks(mApiClient,
                        mGameChannel.getNamespace());
            } catch (IOException e) {
                Log.w(TAG, "Exception while launching application", e);
            }
        }
    }

    private class ConnectionCallbacks implements GoogleApiClient.ConnectionCallbacks {
        @Override
        public void onConnectionSuspended(int cause) {
            Log.d(TAG, "ConnectionCallbacks.onConnectionSuspended");
        }

        @Override
        public void onConnected(Bundle connectionHint) {
            Log.d(TAG, "ConnectionCallbacks.onConnected");
            Cast.CastApi.launchApplication(mApiClient, APP_ID).setResultCallback(
                    new ConnectionResultCallback());
        }
    }

    private class ConnectionFailedListener implements GoogleApiClient.OnConnectionFailedListener {
        @Override
        public void onConnectionFailed(ConnectionResult result) {
            Log.d(TAG, "ConnectionFailedListener.onConnectionFailed");
            setSelectedDevice(null);
        }
    }

    private final class ConnectionResultCallback implements
            ResultCallback<ApplicationConnectionResult> {
        @Override
        public void onResult(ApplicationConnectionResult result) {
            Status status = result.getStatus();
            ApplicationMetadata appMetaData = result.getApplicationMetadata();

            if (status.isSuccess()) {
                Log.d(TAG, "ConnectionResultCallback: " + appMetaData.getName());
                mJoinGameButton.setEnabled(true);
                try {
                    Cast.CastApi.setMessageReceivedCallbacks(mApiClient,
                            mGameChannel.getNamespace(), mGameChannel);
                } catch (IOException e) {
                    Log.w(TAG, "Exception while launching application", e);
                }
            } else {
                Log.d(TAG, "ConnectionResultCallback. Unable to launch the game. statusCode: "
                        + status.getStatusCode());
                mJoinGameButton.setEnabled(false);
            }
        }
    }

}
