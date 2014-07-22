/*
 * Copyright (C) 2014 The Android Open Source Project
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

package com.example.activitydemo.app.service;

import android.app.IntentService;
import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Handler;
import android.os.HandlerThread;
import android.os.IBinder;
import android.os.Looper;
import android.os.Message;
import android.os.ParcelFileDescriptor;
import android.os.PowerManager;
import android.os.RemoteException;
import android.speech.tts.TextToSpeech;
import android.support.v4.app.NotificationCompat;
import android.text.format.DateUtils;
import android.util.Log;

import com.example.activitydemo.app.ActivityLog;
import com.example.activitydemo.app.GameConfig;
import com.example.activitydemo.app.GooglePlayServicesActivity;
import com.google.android.gms.common.ConnectionResult;
import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.common.api.ResultCallback;
import com.google.android.gms.common.api.Status;
import com.google.android.gms.drive.Contents;
import com.google.android.gms.drive.Drive;
import com.google.android.gms.drive.DriveApi;
import com.google.android.gms.drive.DriveFile;
import com.google.android.gms.drive.DriveFolder;
import com.google.android.gms.drive.DriveId;
import com.google.android.gms.drive.MetadataChangeSet;
import com.google.android.gms.drive.query.Filters;
import com.google.android.gms.drive.query.Query;
import com.google.android.gms.drive.query.SearchableField;
import com.google.android.gms.games.Games;
import com.google.android.gms.location.ActivityRecognition;
import com.google.android.gms.location.ActivityRecognitionResult;
import com.google.android.gms.location.DetectedActivity;
import com.example.activitydemo.app.R;

import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.HashSet;
import java.util.List;


/**
 * Contains our Binder service that the Activity will connect to and hosts the Service thread.
 */
public class GameService extends Service implements TextToSpeech.OnInitListener {

    private static final String TAG = "GameService";

    public static final int STATE_PRE_GAME = 0;
    public static final int STATE_IN_GAME = 1;

    private static final int MESSAGE_TICK = 1;
    private static final int MESSAGE_ACTIVITY = 2;
    private static final int MESSAGE_SHUTDOWN = 3;

    private static final long ACTIVITY_INTERVAL = 100L;

    private static final HashSet<Integer> ACTIVITY_MASK = new HashSet<Integer>();

    private static GameHandler mGameHandler;

    private PendingIntent mRecognitionPendingIntent;

    private GameAndroidService mBinderService;

    private GoogleApiClient mClient;
    private HandlerThread mHandlerThread;

    private TextToSpeech mTextToSpeech;
    private boolean mTextToSpeechWorks;
    private boolean mTextToSpeechEnabled;

    private PowerManager.WakeLock mWakeLock;

    static {
        ACTIVITY_MASK.add(DetectedActivity.STILL); // Handle this with accelerometer
        ACTIVITY_MASK.add(DetectedActivity.RUNNING);
        ACTIVITY_MASK.add(DetectedActivity.WALKING);
    }

    @Override
    public void onCreate() {
        mBinderService = new GameAndroidService(this);
    }

    @Override
    public IBinder onBind(Intent intent) {
        Log.d(TAG, "onBind");
        if (mTextToSpeech == null) {
            mTextToSpeech = new TextToSpeech(this, this);
        }
        return mBinderService.asBinder();
    }

    @Override
    public void onDestroy() {
        if (mTextToSpeech != null) {
            mTextToSpeech.shutdown();
        }
        removeActivityUpdates();
        if (mClient != null) {
            mClient.disconnect();
        }
    }

    public void startForeground() {
        Notification notification = new NotificationCompat.Builder(this)
                .setContentTitle(getString(R.string.app_name))
                .setContentText(getString(R.string.game_in_progress))
                .setContentIntent(buildActivityPendingIntent())
                .setSmallIcon(getApplicationInfo().icon)
                .setOngoing(true)
                .build();

        mHandlerThread = new HandlerThread("GameHandler");
        mHandlerThread.start();
        mGameHandler = new GameHandler(mHandlerThread.getLooper(), mGameCallbacks);


        super.startForeground(R.string.app_name, notification);
    }

    public void setGameCallbacks(GameCallbacks gameCallbacks) {
        mGameCallbacks = gameCallbacks;
    }

    public PendingIntent buildActivityPendingIntent() {
        return PendingIntent.getActivity(this,
                GooglePlayServicesActivity.REQUEST_CODE_NOTIFICATION,
                new Intent(this, GooglePlayServicesActivity.class),
                0 /* flags */);
    }

    private void connectGoogleApiClient() {
        synchronized (mBinderService) {
            if (mClient == null) {
                mClient = GooglePlayServicesActivity.getGoogleApiClientBuilder(this)
                        .addConnectionCallbacks(mBinderService)
                        .addOnConnectionFailedListener(mBinderService)
                        .build();
            }
            if (!(mClient.isConnected() || mClient.isConnecting())) {
                mClient.connect();
            } else {
                if (mBinderService.isConnected()) {
                    try {
                        int state = inGame() ? STATE_IN_GAME : STATE_PRE_GAME;
                        mBinderService.mCallbacks.onConnected(state);
                    } catch (RemoteException e) {
                        e.printStackTrace();
                    }
                }
            }
        }
    }

    public boolean inGame() {
        return mGameCallbacks != null;
    }

    public void startGame(final GameConfig config) {
        // Start ourselves!
        startService(new Intent(this, GameService.class));

        synchronized (mBinderService) {
            PowerManager pm = (PowerManager) getSystemService(Context.POWER_SERVICE);
            if (mWakeLock == null) {
                mWakeLock = pm.newWakeLock(PowerManager.PARTIAL_WAKE_LOCK, "ActivityGameService");
                mWakeLock.setReferenceCounted(false);
            }

            mWakeLock.acquire();
        }

        mTextToSpeechEnabled = config.useSound;

        Log.d(TAG, "Starting game!");
        sayString(getString(R.string.game_started));
        setGameCallbacks(new GameLogic(this, config, GameTrack.BASIC_TRACK));

        startForeground();
        registerActivityUpdates();
        mGameHandler.startGame();

        if (mBinderService.isConnected()) {
            try {
                mBinderService.mCallbacks.onStartGame();
            }catch(RemoteException e){
            }
        }
    }

    public void endGame() {
        Log.d(TAG, "Ending game");

        new UpdateDriveFileTask().execute(mGameHandler.mActivities);

        Games.Leaderboards.submitScore(mClient,
                getString(R.string.leaderboard_high_score_basic_track),
                ((GameLogic)mGameCallbacks).getScore());
        removeActivityUpdates();

        setGameCallbacks(null);

        Notification notification = new NotificationCompat.Builder(this)
                .setContentTitle(getString(R.string.game_over))
                .setSmallIcon(getApplicationInfo().icon)
                .setContentText(getString(R.string.game_over_notification_text))
                .setContentIntent(buildActivityPendingIntent())
                .build();

        NotificationManager nm = (NotificationManager) getSystemService(
                Context.NOTIFICATION_SERVICE);

        nm.notify(R.string.game_over, notification);

        mGameHandler.sendMessageDelayed(mGameHandler.obtainMessage(MESSAGE_SHUTDOWN),
                5 * DateUtils.SECOND_IN_MILLIS);

        if (mBinderService.isConnected()) {
            try {
                mBinderService.mCallbacks.onEndGame();
            } catch (RemoteException e) { }
        }
    }

    public void shutdown() {
        stopForeground(true);
        mWakeLock.release();
        stopSelf();

        mHandlerThread.quit();
        mHandlerThread = null;
    }

    public void updateScore(long score) {
        if (mBinderService.isConnected()) {
            try {
                mBinderService.mCallbacks.onScoreChanged(score);
            } catch (RemoteException e) {
                e.printStackTrace();
            }
        }
    }

    public void registerActivityUpdates() {
        if (mRecognitionPendingIntent == null) {
            mRecognitionPendingIntent = PendingIntent.getService(this, 0,
                    new Intent(this, RecognitionIntentService.class), PendingIntent.FLAG_UPDATE_CURRENT);
        }
        ActivityRecognition.ActivityRecognitionApi.requestActivityUpdates(mClient,
                ACTIVITY_INTERVAL,
                mRecognitionPendingIntent).setResultCallback(new ResultCallback<Status>() {
            @Override
            public void onResult(Status status) {
                if(status.isSuccess()) {
                    Log.d(TAG, "Activities coming!");
                } else {
                    Log.e(TAG, "No activity coming!");
                }
            }
        });
    }

    public void removeActivityUpdates() {
        if (mRecognitionPendingIntent == null) {
            mRecognitionPendingIntent = PendingIntent.getService(this, 0,
                    new Intent(this, RecognitionIntentService.class),
                    PendingIntent.FLAG_UPDATE_CURRENT);
        }
        if (mClient != null && mClient.isConnected()) {
            ActivityRecognition.ActivityRecognitionApi.removeActivityUpdates(mClient,
                    mRecognitionPendingIntent);
            mRecognitionPendingIntent.cancel();
        }
        mRecognitionPendingIntent.cancel();
    }

    @Override
    public void onInit(int result) {
        Log.d(TAG, "TTS Engine " + (result == 0 ? "Successful" : "Error"));
        if (result == TextToSpeech.SUCCESS) {
            mTextToSpeechWorks = true;
        }
    }

    public void sayString(String text) {
        if (mTextToSpeechWorks && mTextToSpeechEnabled) {
            mTextToSpeech.speak(text, TextToSpeech.QUEUE_ADD, null);
        }
    }

    public static final class RecognitionIntentService extends IntentService {
        private static final String TAG = "RecognitionIntentService";
        public RecognitionIntentService() {
            super(TAG);
        }

        @Override
        public void onHandleIntent(Intent intent) {
            if (ActivityRecognitionResult.hasResult(intent)) {
                ActivityRecognitionResult result = ActivityRecognitionResult.extractResult(intent);
                List<DetectedActivity> activities = result.getProbableActivities();

                DetectedActivity bestActivity = null;
                int bestConfidence = 0;

                for (DetectedActivity activity : activities) {
                    Log.d(TAG, "Activity " + activity.getType() + " " + activity.getConfidence());
                    if (ACTIVITY_MASK.contains(activity.getType())) {
                        if (activity.getConfidence() > bestConfidence) {
                            bestActivity = activity;
                            bestConfidence = activity.getConfidence();
                        }
                    }
                }

                DetectedActivity currentActivity = bestActivity;
                long currentTime = System.currentTimeMillis();
                if (currentActivity == null) {
                    Log.w(TAG, "No activity matches!");
                    return;
                }
                if (mGameHandler != null && mGameHandler.getLooper().getThread().isAlive()) {
                    mGameHandler.sendMessage(
                            mGameHandler.obtainMessage(MESSAGE_ACTIVITY,
                                    new ActivityLog(currentActivity, currentTime))
                    );
                }
            }
        }
    }

    private GameCallbacks mGameCallbacks;

    public final class GameHandler extends Handler {
        public static final long TICK_LENGTH = 50;

        private final GameCallbacks mCallbacks;
        private final List<ActivityLog> mActivities = new ArrayList<ActivityLog>();

        public GameHandler(Looper looper, GameCallbacks callbacks) {
            super(looper);
            mCallbacks = callbacks;
        }

        public void startGame() {
            sendMessage(obtainMessage(MESSAGE_TICK));
            mActivities.clear();
        }

        @Override
        public void handleMessage(Message msg) {
            switch(msg.what) {
                case MESSAGE_TICK:
                    boolean loop = mCallbacks.onTick(msg.getWhen());
                    if (loop) {
                        sendMessageDelayed(obtainMessage(MESSAGE_TICK), TICK_LENGTH);
                    } else {
                        endGame();
                    }
                    break;
                case MESSAGE_ACTIVITY:
                    ActivityLog aLog = (ActivityLog) msg.obj;
                    mActivities.add(aLog);
                    String activityString = aLog.getActivityType();
                    Log.d(TAG, "Activity is " + activityString + " at "
                            + aLog.getDetectedTime());
                    if (mBinderService.isConnected()) {
                        try {
                            mBinderService.mCallbacks.onActivityChanged(aLog);
                        } catch (RemoteException e) {
                        }
                    }
                    //sayString(activityString);

                    mCallbacks.onActivityDetected(aLog);

                    break;
                case MESSAGE_SHUTDOWN:
                    shutdown();
                    break;
            }
        }
    }

    public final class GameAndroidService extends IActivityGameService.Stub
            implements GoogleApiClient.ConnectionCallbacks,
            GoogleApiClient.OnConnectionFailedListener {
        private static final String TAG = "GameAndroidService";

        private GameService mContext;

        private IActivityGameServiceCallbacks mCallbacks;

        public GameAndroidService(GameService context) {
            mContext = context;
        }

        public boolean isConnected() {
            return mCallbacks != null && isBinderAlive();
        }

        @Override
        public void connect(IActivityGameServiceCallbacks callbacks) throws RemoteException {
            Log.d(TAG, "User connecting.");
            mCallbacks = callbacks;

            mContext.connectGoogleApiClient();
        }

        @Override
        public void startGame(GameConfig config) throws RemoteException {
            mContext.startGame(config);
        }

        @Override
        public List getHistory() throws RemoteException {
            return mGameHandler.mActivities;
        }

        @Override
        public long getScore() throws RemoteException {
            if (mGameCallbacks instanceof GameLogic) {
                return ((GameLogic)mGameCallbacks).getScore();
            }
            return 0;
        }

        @Override
        public void disconnect() throws RemoteException {
            mContext.endGame();
        }

        @Override
        public void onConnected(Bundle bundle) {
            Log.d(TAG, "Connection Successful.");
            if (isConnected()) {
                try {
                    mCallbacks.onConnected(mContext.inGame() ? STATE_IN_GAME : STATE_PRE_GAME);
                } catch (RemoteException e) {
                    e.printStackTrace();
                }
            }
        }

        @Override
        public void onConnectionSuspended(int i) {
            // We will ignore this for now.
            Log.w(TAG, "Connection Suspended...");
        }

        @Override
        public void onConnectionFailed(ConnectionResult connectionResult) {
            // We will ignore this for now.
            Log.w(TAG, "Connection Failed!");
        }
    }

    private class UpdateDriveFileTask extends AsyncTask<List<ActivityLog>, Void, Void> {
        private final SimpleDateFormat DATE_FORMAT = new SimpleDateFormat("yyyy-MM-dd");
        private final String FOLDER_NAME = "ActivityDemo";

        private DriveFolder createFolderIfNeeded(DriveFolder root) {
            DriveApi.MetadataBufferResult folders = root.queryChildren(mClient, new Query.Builder()
                    .addFilter(Filters.eq(SearchableField.MIME_TYPE, DriveFolder.MIME_TYPE))
                    .addFilter(Filters.eq(SearchableField.TITLE, FOLDER_NAME))
                    .build()).await();

            if (!folders.getStatus().isSuccess()) {
                Log.d(TAG, "Failed to search metadata");
                return null;
            }
            try {
                if (folders.getMetadataBuffer().getCount() > 0) {
                    Log.d(TAG, "Using Existing Folder");
                    DriveId id = folders.getMetadataBuffer().get(0).getDriveId();
                    return Drive.DriveApi.getFolder(mClient, id);

                } else {
                    Log.d(TAG, "Creating Folder");
                    DriveFolder.DriveFolderResult folderResult = root.createFolder(mClient,
                            new MetadataChangeSet.Builder()
                                    .setTitle(FOLDER_NAME)
                                    .build()
                    )
                            .await();
                    if (!folderResult.getStatus().isSuccess()) {
                        Log.e(TAG, "Failed to create folder.");
                        return null;
                    }
                    return folderResult.getDriveFolder();
                }
            } finally {
                folders.getMetadataBuffer().release();
            }
        }

        private DriveFile createFileIfNeeded(DriveFolder root) {
            String today = DATE_FORMAT.format(Calendar.getInstance().getTime());
            String todayFilename = "ActivityDemo-" + today;

            Log.d(TAG, "Filename " + todayFilename);

            DriveFolder folder = createFolderIfNeeded(root);
            if (folder == null) {
                return null;
            }

            DriveApi.MetadataBufferResult metadataBufferResult = folder.queryChildren(mClient,
                    new Query.Builder()
                            .addFilter(Filters.eq(SearchableField.TITLE, todayFilename))
                            .addFilter(Filters.eq(SearchableField.MIME_TYPE, "text/plain"))
                            .build())
                    .await();
            if (!metadataBufferResult.getStatus().isSuccess()) {
                Log.d(TAG, "Failed to search metadata");
                return null;
            }
            try {
                if (metadataBufferResult.getMetadataBuffer().getCount() == 0) {
                    Log.d(TAG, "Creating new file.");
                    // No file exists.

                    DriveApi.ContentsResult contentsResult = Drive.DriveApi.newContents(mClient)
                            .await();
                    if (!contentsResult.getStatus().isSuccess()) {
                        Log.e(TAG, "Failed to open new contents for writing.");
                        return null;
                    }
                    Contents contents = contentsResult.getContents();
                    try {
                        contents.getOutputStream().write(new byte[0]);
                    } catch (IOException e) { }

                    DriveFolder.DriveFileResult fileResult = folder.createFile(mClient,
                            new MetadataChangeSet.Builder()
                                    .setTitle(todayFilename)
                                    .setMimeType("text/plain")
                                    .build(),
                            contents).await();
                    if (!fileResult.getStatus().isSuccess()) {
                        Log.e(TAG, "Failed to create new file.");
                        return null;
                    }
                    return fileResult.getDriveFile();
                } else {
                    Log.d(TAG, "File already exists.");
                    DriveId driveId = metadataBufferResult.getMetadataBuffer().get(0).getDriveId();
                    return Drive.DriveApi.getFile(mClient, driveId);
                }
            } finally {
                metadataBufferResult.getMetadataBuffer().release();
            }
        }

        private void moveToEndOfFile(ParcelFileDescriptor pfd) throws IOException {
            FileInputStream fis = new FileInputStream(pfd.getFileDescriptor());
            byte buffer[] = new byte[4096];
            while (fis.read(buffer) != -1) { }
        }

        @Override
        protected Void doInBackground(List<ActivityLog>... activityLogs) {
            if (activityLogs == null || activityLogs[0] == null) {
                return null;
            }

            if (mClient != null && mClient.isConnected()) {
                DriveFolder root = Drive.DriveApi.getRootFolder(mClient);

                DriveFile file = createFileIfNeeded(root);
                if (file == null) {
                    Log.e(TAG, "Could not get file.");
                    return null;
                }

                DriveApi.ContentsResult contentsResult = file.openContents(mClient,
                        DriveFile.MODE_READ_WRITE, null).await();
                if (!contentsResult.getStatus().isSuccess()) {
                    Log.e(TAG, "Failed to open file for writing.");
                    return null;
                }
                Contents contents = contentsResult.getContents();
                ParcelFileDescriptor parcelFileDescriptor = contents.getParcelFileDescriptor();

                try {
                    moveToEndOfFile(parcelFileDescriptor);
                } catch (IOException e) {
                    Log.e(TAG, "Failed to move to end of file");
                    contents.close();
                    e.printStackTrace();
                    return null;
                }

                Log.d(TAG, "Writing File");
                OutputStream outStream = new FileOutputStream(
                        parcelFileDescriptor.getFileDescriptor());
                OutputStreamWriter writer = new OutputStreamWriter(outStream);
                try {
                    SimpleDateFormat dateFormat = new SimpleDateFormat("HH:mm:ss.SSS");
                    writer.write("--- Begin Event ---\n");
                    for (ActivityLog log : activityLogs[0]) {
                        String s = dateFormat.format(new Date(log.getDetectedTime()))
                                + ": " + log.getActivityType() + "\n";
                        writer.write(s);
                    }
                    writer.write("--- End Event ---\n");
                    writer.close();
                } catch (IOException e) {
                    e.printStackTrace();
                }
                if (file.commitAndCloseContents(mClient, contents)
                        .await().getStatus().isSuccess()) {
                    Log.d(TAG, "Wrote File");
                } else {
                    Log.d(TAG, "Write failed.");
                }

                Drive.DriveApi.requestSync(mClient);
            }
            return null;
        }
    }
}
