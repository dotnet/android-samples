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

import com.google.android.gms.cast.Cast;
import com.google.android.gms.cast.CastDevice;
import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.common.api.ResultCallback;
import com.google.android.gms.common.api.Status;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import android.util.Log;

/**
 * An abstract class which encapsulates control and game logic for sending and receiving messages
 * during a TicTacToe game.
 */
public abstract class GameChannel implements Cast.MessageReceivedCallback {
    private static final String TAG = GameChannel.class.getSimpleName();

    private static final String GAME_NAMESPACE = "urn:x-cast:com.google.cast.demo.tictactoe";

    public static final String END_STATE_X_WON = "X-won";
    public static final String END_STATE_O_WON = "O-won";
    public static final String END_STATE_DRAW = "draw";
    public static final String END_STATE_ABANDONED = "abandoned";

    public static final String PLAYER_X = "X";
    public static final String PLAYER_O = "O";

    // Receivable event types
    private static final String KEY_BOARD_LAYOUT_RESPONSE = "board_layout_response";
    private static final String KEY_EVENT = "event";
    private static final String KEY_JOINED = "joined";
    private static final String KEY_MOVED = "moved";
    private static final String KEY_ENDGAME = "endgame";
    private static final String KEY_ERROR = "error";

    // Commands
    private static final String KEY_BOARD_LAYOUT_REQUEST = "board_layout_request";
    private static final String KEY_COMMAND = "command";
    private static final String KEY_JOIN = "join";
    private static final String KEY_MOVE = "move";
    private static final String KEY_LEAVE = "leave";

    private static final String KEY_BOARD = "board";
    private static final String KEY_COLUMN = "column";
    private static final String KEY_END_STATE = "end_state";
    private static final String KEY_GAME_OVER = "game_over";
    private static final String KEY_MESSAGE = "message";
    private static final String KEY_NAME = "name";
    private static final String KEY_OPPONENT = "opponent";
    private static final String KEY_PLAYER = "player";
    private static final String KEY_ROW = "row";
    private static final String KEY_WINNING_LOCATION = "winning_location";

    /**
     * An enum representing board rows, columns, and diagonals as numerical values.
     */
    public enum WinningLocation {
        ROW_0(0),
        ROW_1(1),
        ROW_2(2),
        COL_0(3),
        COL_1(4),
        COL_2(5),
        DIAGONAL_TOPLEFT(6),
        DIAGONAL_BOTTOMLEFT(7),
        UNKNOWN(-1);

        int mValue;

        private WinningLocation(int value) {
            mValue = value;
        }

        public int getValue() {
            return mValue;
        }

        /**
         * Returns a WinningLocation, given an int value.
         */
        public static WinningLocation fromIntValue(int value) {
            if (ROW_0.getValue() == value) {
                return ROW_0;
            } else if (ROW_1.getValue() == value) {
                return ROW_1;
            } else if (ROW_2.getValue() == value) {
                return ROW_2;
            } else if (COL_0.getValue() == value) {
                return COL_0;
            } else if (COL_1.getValue() == value) {
                return COL_1;
            } else if (COL_2.getValue() == value) {
                return COL_2;
            } else if (DIAGONAL_TOPLEFT.getValue() == value) {
                return DIAGONAL_TOPLEFT;
            } else if (DIAGONAL_BOTTOMLEFT.getValue() == value) {
                return DIAGONAL_BOTTOMLEFT;
            } else {
                return UNKNOWN;
            }
        }
    }

    /**
     * Constructs a new GameChannel m with GAME_NAMESPACE as the namespace used by
     * the superclass.
     */
    protected GameChannel() {
    }

    /**
     * Performs some action upon a player joining the game.
     *
     * @param playerSymbol either X or O
     * @param opponentName the name of the player who just joined an existing game, or the opponent
     */
    protected abstract void onGameJoined(String playerSymbol, String opponentName);

    /**
     * Performs some action, or updates the game display upon a move.
     *
     * @param playerSymbol either X or O
     * @param row the row index of the move
     * @param column the column index of the move
     * @param isGameOver whether or not the game ended as a result of the move
     */
    protected abstract void onGameMove(
            String playerSymbol, int row, int column, boolean isGameOver);

    /**
     * Performs some action upon game end, depending on game's end state and the position of the
     * winning pieces.
     *
     * @param endState likely to be END_STATE_X_WON, END_STATE_O_WON, or END_STATE_ABANDONED
     * @param location an int value corresponding to the enum WinningLocation's values
     */
    protected abstract void onGameEnd(String endState, int location);

    /**
     * Performs some action upon an int[][] board layout being sent.
     *
     * @param boardLayout a 2-D array of ints, likely to be 3x3
     */
    protected abstract void onGameBoardLayout(int[][] boardLayout);

    /**
     * Performs some action upon a game error.
     *
     * @param errorMessage the string description of the error
     */
    protected abstract void onGameError(String errorMessage);

    /**
     * Returns the namespace for this cast channel.
     */
    public String getNamespace() {
        return GAME_NAMESPACE;
    }

    /**
     * Attempts to connect to an existing session of the game by sending a join command.
     *
     * @param name the name of the player that is joining
     */
    public final void join(GoogleApiClient apiClient, String name) {
        try {
            Log.d(TAG, "join: " + name);
            JSONObject payload = new JSONObject();
            payload.put(KEY_COMMAND, KEY_JOIN);
            payload.put(KEY_NAME, name);
            sendMessage(apiClient, payload.toString());
        } catch (JSONException e) {
            Log.e(TAG, "Cannot create object to join a game", e);
        }
    }

    /**
     * Attempts to make a move by sending a command to place a piece in the given row and column.
     */
    public final void move(GoogleApiClient apiClient, final int row, final int column) {
        Log.d(TAG, "move: row:" + row + " column:" + column);
        try {
            JSONObject payload = new JSONObject();
            payload.put(KEY_COMMAND, KEY_MOVE);
            payload.put(KEY_ROW, row);
            payload.put(KEY_COLUMN, column);
            sendMessage(apiClient, payload.toString());
        } catch (JSONException e) {
            Log.e(TAG, "Cannot create object to send a move", e);
        }
    }

    /**
     * Sends a command to leave the current game.
     */
    public final void leave(GoogleApiClient apiClient) {
        try {
            Log.d(TAG, "leave");
            JSONObject payload = new JSONObject();
            payload.put(KEY_COMMAND, KEY_LEAVE);
            sendMessage(apiClient, payload.toString());
        } catch (JSONException e) {
            Log.e(TAG, "Cannot create object to leave a game", e);
        }
    }

    /**
     * Sends a command requesting the current layout of the board.
     */
    public final void requestBoardLayout(GoogleApiClient apiClient) {
        try {
            Log.d(TAG, "requestBoardLayout");
            JSONObject payload = new JSONObject();
            payload.put(KEY_COMMAND, KEY_BOARD_LAYOUT_REQUEST);
            sendMessage(apiClient, payload.toString());
        } catch (JSONException e) {
            Log.e(TAG, "Cannot create object to request board layout", e);
        }
    }

    /**
     * Processes all Text messages received from the receiver device and performs the appropriate
     * action for the message. Recognizable messages are of the form:
     *
     * <ul>
     * <li> KEY_JOINED: a player joined the current game
     * <li> KEY_MOVED: a player made a move
     * <li> KEY_ENDGAME: the game has ended in one of the END_STATE_* states
     * <li> KEY_ERROR: a game error has occurred
     * <li> KEY_BOARD_LAYOUT_RESPONSE: the board has been laid out in some new configuration
     * </ul>
     *
     * <p>No other messages are recognized.
     */
    @Override
    public void onMessageReceived(CastDevice castDevice, String namespace, String message) {
        try {
            Log.d(TAG, "onTextMessageReceived: " + message);
            JSONObject payload = new JSONObject(message);
            Log.d(TAG, "payload: " + payload);
            if (payload.has(KEY_EVENT)) {
                String event = payload.getString(KEY_EVENT);
                if (KEY_JOINED.equals(event)) {
                    Log.d(TAG, "JOINED");
                    try {
                        String player = payload.getString(KEY_PLAYER);
                        String opponentName = payload.getString(KEY_OPPONENT);
                        onGameJoined(player, opponentName);
                    } catch (JSONException e) {
                        e.printStackTrace();
                    }
                } else if (KEY_MOVED.equals(event)) {
                    Log.d(TAG, "MOVED");
                    try {
                        String player = payload.getString(KEY_PLAYER);
                        int row = payload.getInt(KEY_ROW);
                        int column = payload.getInt(KEY_COLUMN);
                        boolean isGameOver = payload.getBoolean(KEY_GAME_OVER);
                        onGameMove(player, row, column, isGameOver);
                    } catch (JSONException e) {
                        e.printStackTrace();
                    }
                } else if (KEY_ENDGAME.equals(event)) {
                    Log.d(TAG, "ENDGAME");
                    try {
                        String endState = payload.getString(KEY_END_STATE);
                        int winningLocation = -1;
                        if (END_STATE_ABANDONED.equals(endState) == false) {
                            winningLocation = payload.getInt(KEY_WINNING_LOCATION);
                        }
                        onGameEnd(endState, winningLocation);
                    } catch (JSONException e) {
                        e.printStackTrace();
                    }
                } else if (KEY_ERROR.equals(event)) {
                    Log.d(TAG, "ERROR");
                    try {
                        String errorMessage = payload.getString(KEY_MESSAGE);
                        onGameError(errorMessage);
                    } catch (JSONException e) {
                        e.printStackTrace();
                    }
                } else if (KEY_BOARD_LAYOUT_RESPONSE.equals(event)) {
                    Log.d(TAG, "Board Layout");
                    int[][] boardLayout = new int[3][3];
                    try {
                        JSONArray boardJSONArray = payload.getJSONArray(KEY_BOARD);
                        for (int i = 0; i < 3; ++i) {
                            for (int j = 0; j < 3; ++j) {
                                boardLayout[i][j] = boardJSONArray.getInt(i * 3 + j);
                            }
                        }
                        onGameBoardLayout(boardLayout);
                    } catch (JSONException e) {
                        e.printStackTrace();
                    }
                }
            } else {
                Log.w(TAG, "Unknown payload: " + payload);
            }
        } catch (JSONException e) {
            Log.w(TAG, "Message doesn't contain an expected key.", e);
        }
    }

    private final void sendMessage(GoogleApiClient apiClient, String message) {
        Log.d(TAG, "Sending message: (ns=" + GAME_NAMESPACE + ") " + message);
        Cast.CastApi.sendMessage(apiClient, GAME_NAMESPACE, message).setResultCallback(
                new SendMessageResultCallback(message));
    }

    private final class SendMessageResultCallback implements ResultCallback<Status> {
        String mMessage;

        SendMessageResultCallback(String message) {
            mMessage = message;
        }

        @Override
        public void onResult(Status result) {
            if (!result.isSuccess()) {
                Log.d(TAG, "Failed to send message. statusCode: " + result.getStatusCode()
                        + " message: " + mMessage);
            }
        }
    }

}
