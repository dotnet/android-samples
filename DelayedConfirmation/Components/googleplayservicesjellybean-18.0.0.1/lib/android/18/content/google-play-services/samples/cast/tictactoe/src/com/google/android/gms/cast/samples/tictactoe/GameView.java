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

import android.content.Context;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.Bitmap.Config;
import android.graphics.BitmapFactory;
import android.graphics.BitmapFactory.Options;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.graphics.Paint.Style;
import android.graphics.Rect;
import android.graphics.drawable.Drawable;
import android.util.AttributeSet;
import android.view.MotionEvent;
import android.view.View;

/**
 * A View containing board-drawing logic for the TicTacToe application.
 */
public class GameView extends View {
    /**
     * An enum representing symbols on the board, either X, O, or empty, as int values.
     */
    public enum State {
        UNKNOWN(-1),
        EMPTY(0),
        PLAYER_X(1),
        PLAYER_O(2);

        private int mValue;

        private State(int value) {
            mValue = value;
        }

        public int getValue() {
            return mValue;
        }

        /**
         * Creates and returns a State object based on a passed int value.
         */
        public static State fromInt(int i) {
            for (State s : values()) {
                if (s.getValue() == i) {
                    return s;
                }
            }
            return EMPTY;
        }
    }

    /**
     * A class which listens to cell events with a given row and column, and performs some action.
     */
    public interface ICellListener {
        abstract void onCellSelected(int row, int column);
    }

    private static final int MARGIN = 4;

    private final Rect mSrcRect = new Rect();
    private final Rect mDstRect = new Rect();

    private int mSxy;
    private int mOffetX;
    private int mOffetY;
    private Paint mWinPaint;
    private Paint mLinePaint;
    private Paint mBmpPaint;
    private Bitmap mBmpPlayerX;
    private Bitmap mBmpPlayerO;
    private ICellListener mCellListener;

    /**
     * Contains one of {@link State#EMPTY}, {@link State#PLAYER_X} or {@link State#PLAYER_O}.
     */
    private final State[][] mBoard = new State[3][3];
    private State mAssignedPlayer = State.UNKNOWN;

    private int mWinCol;
    private int mWinRow;
    private int mWinDiag;

    /**
     * Creates a new GameView object and initializes board drawing tools.
     *
     * @param context the application context this object lies within
     * @param attrs this object's specified attributes, whether through xml or otherwise
     */
    public GameView(Context context, AttributeSet attrs) {
        super(context, attrs);
        requestFocus();

        mBmpPlayerX = getResBitmap(R.drawable.lib_cross);
        mBmpPlayerO = getResBitmap(R.drawable.lib_circle);

        if (mBmpPlayerX != null) {
            mSrcRect.set(0, 0, mBmpPlayerX.getWidth() - 1, mBmpPlayerX.getHeight() - 1);
        }

        mBmpPaint = new Paint(Paint.ANTI_ALIAS_FLAG);

        mLinePaint = new Paint();
        mLinePaint.setColor(0xFFFFFFFF);
        mLinePaint.setStrokeWidth(5);
        mLinePaint.setStyle(Style.STROKE);

        mWinPaint = new Paint(Paint.ANTI_ALIAS_FLAG);
        mWinPaint.setColor(0xFFFF0000);
        mWinPaint.setStrokeWidth(10);
        mWinPaint.setStyle(Style.STROKE);

        clearBoard();
    }

    /**
     * Sets the cell at the given row and column to the given State value.
     */
    public void setCell(int row, int column, State value) {
        mBoard[row][column] = value;
        invalidate();
    }

    /**
     * Updates the current layout of the board with the values stored in boardLayout.
     */
    public void updateBoard(int[][] boardLayout) {
        for (int i = 0; i < boardLayout.length; ++i) {
            for (int j = 0; j < boardLayout[i].length; ++j) {
                mBoard[i][j] = State.fromInt(boardLayout[i][j]);
            }
        }
        invalidate();
    }

    /**
     * Sets the specific cellListener this object uses.
     */
    public void setCellListener(ICellListener cellListener) {
        mCellListener = cellListener;
    }

    public State getAssignedPlayer() {
        return mAssignedPlayer;
    }

    /**
     * Sets or resets the currently assigned player, and clears the board.
     */
    public void setAssignedPlayer(State player) {
        mAssignedPlayer = player;
        clearBoard();
    }

    /**
     * Sets winning mark on specified column or row (0..2) or diagonal (0..1).
     */
    public void setFinished(int row, int column, int diagonal) {
        mWinRow = row;
        mWinCol = column;
        mWinDiag = diagonal;
        invalidate();
    }

    /**
     * Draws the board after each update, including all X and O symbols and possible win-state
     * indicator lines, if any.
     */
    @Override
    protected void onDraw(Canvas canvas) {
        super.onDraw(canvas);

        int sxy = mSxy;
        int s3 = sxy * 3;
        int x7 = mOffetX;
        int y7 = mOffetY;

        for (int i = 0, k = sxy; i < 2; i++, k += sxy) {
            canvas.drawLine(x7, y7 + k, x7 + s3 - 1, y7 + k, mLinePaint);
            canvas.drawLine(x7 + k, y7, x7 + k, y7 + s3 - 1, mLinePaint);
        }

        for (int j = 0, y = y7; j < 3; j++, y += sxy) {
            for (int i = 0, x = x7; i < 3; i++, x += sxy) {
                mDstRect.offsetTo(MARGIN + x, MARGIN + y);
                State value = mBoard[j][i];

                if (value == State.PLAYER_X && mBmpPlayerX != null) {
                    canvas.drawBitmap(mBmpPlayerX, mSrcRect, mDstRect, mBmpPaint);
                } else if (value == State.PLAYER_O && mBmpPlayerO != null) {
                    canvas.drawBitmap(mBmpPlayerO, mSrcRect, mDstRect, mBmpPaint);
                }
            }
        }

        if (mWinRow >= 0) {
            int y = y7 + mWinRow * sxy + sxy / 2;
            canvas.drawLine(x7 + MARGIN, y, x7 + s3 - 1 - MARGIN, y, mWinPaint);
        } else if (mWinCol >= 0) {
            int x = x7 + mWinCol * sxy + sxy / 2;
            canvas.drawLine(x, y7 + MARGIN, x, y7 + s3 - 1 - MARGIN, mWinPaint);
        } else if (mWinDiag == 0) {
            // Diagonal 0 is from (0,0) to (2,2)
            canvas.drawLine(x7 + MARGIN, y7 + MARGIN, x7 + s3 - 1 - MARGIN, y7 + s3 - 1 - MARGIN,
                    mWinPaint);
        } else if (mWinDiag == 1) {
            // Diagonal 1 is from (0,2) to (2,0)
            canvas.drawLine(x7 + MARGIN, y7 + s3 - 1 - MARGIN, x7 + s3 - 1 - MARGIN, y7 + MARGIN,
                    mWinPaint);
        }
    }

    /**
     * Ensures that the measured width and height of this View is square.
     */
    @Override
    protected void onMeasure(int widthMeasureSpec, int heightMeasureSpec) {
        int w = MeasureSpec.getSize(widthMeasureSpec);
        int h = MeasureSpec.getSize(heightMeasureSpec);
        int d = (w == 0) ? h : ((h == 0) ? w : ((w < h) ? w : h));
        setMeasuredDimension(d, d);
    }

    @Override
    protected void onSizeChanged(int w, int h, int oldw, int oldh) {
        super.onSizeChanged(w, h, oldw, oldh);

        int sx = (w - 2 * MARGIN) / 3;
        int sy = (h - 2 * MARGIN) / 3;
        int size = sx < sy ? sx : sy;

        mSxy = size;
        mOffetX = (w - 3 * size) / 2;
        mOffetY = (h - 3 * size) / 2;

        mDstRect.set(MARGIN, MARGIN, size - MARGIN, size - MARGIN);
    }

    /**
     * Detects whether the user has touched and released a cell, and if so alerts its listener that
     * the cell has been selected.
     */
    @Override
    public boolean onTouchEvent(MotionEvent event) {
        int action = event.getAction();

        if (action == MotionEvent.ACTION_DOWN) {
            return true;

        } else if (action == MotionEvent.ACTION_UP) {
            int x = (int) event.getX();
            int y = (int) event.getY();

            int sxy = mSxy;
            x = (x - MARGIN) / sxy;
            y = (y - MARGIN) / sxy;

            if ((x >= 0) && (x < 3) && (y >= 0) && (y < 3) && (mBoard[y][x] == State.EMPTY)) {
                if (mCellListener != null) {
                    mCellListener.onCellSelected(y, x);
                }
            }
            return true;
        }
        return false;
    }

    /**
     * Clears the board of all moves made and redraws the board.
     */
    public void clearBoard() {
        mWinCol = -1;
        mWinRow = -1;
        mWinDiag = -1;
        for (int i = 0; i < mBoard.length; ++i) {
            for (int j = 0; j < mBoard[i].length; ++j) {
                mBoard[i][j] = State.EMPTY;
            }
        }
        invalidate();
    }

    /**
     * Converts a bitmap resource ID to a valid Bitmap object.
     */
    private Bitmap getResBitmap(int bmpResId) {
        Options opts = new Options();
        opts.inDither = false;

        Resources res = getResources();
        Bitmap bmp = BitmapFactory.decodeResource(res, bmpResId, opts);

        if (bmp == null && isInEditMode()) {
            Drawable d = res.getDrawable(bmpResId);
            int w = d.getIntrinsicWidth();
            int h = d.getIntrinsicHeight();
            bmp = Bitmap.createBitmap(w, h, Config.ARGB_8888);
            Canvas c = new Canvas(bmp);
            d.setBounds(0, 0, w - 1, h - 1);
            d.draw(c);
        }
        return bmp;
    }
}
