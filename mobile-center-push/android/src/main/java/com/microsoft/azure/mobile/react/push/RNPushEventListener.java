package com.microsoft.azure.mobile.react.push;

import android.app.Activity;
import android.app.AlertDialog;
import android.util.Log;
import android.widget.Toast;

import com.facebook.react.bridge.ReactApplicationContext;
import com.facebook.react.bridge.WritableMap;
import com.microsoft.azure.mobile.push.PushListener;
import com.microsoft.azure.mobile.push.PushNotification;

import org.json.JSONException;

import java.util.Map;

import static com.facebook.react.modules.core.DeviceEventManagerModule.*;

public class RNPushEventListener implements PushListener {
    private ReactApplicationContext mReactApplicationContext;
    private boolean mSaveInitialNotification = true;
    private WritableMap mInitialNotification = null;

    private static final String ON_PUSH_NOTIFICATION_RECEIVED_EVENT = "MobileCenterPushNotificationReceived";

    public final void setReactApplicationContext(ReactApplicationContext reactApplicationContext) {
        this.mReactApplicationContext = reactApplicationContext;
    }

    @Override
    public void onPushNotificationReceived(Activity activity, PushNotification pushNotification) {
        RNPushUtil.logInfo("Push notification received");

        // If saveInitialNotification is true, assume we've just been launched and save the first notification we receive.
        // This handles the scenario that when the user taps on a background notification to launch the app, the launch notification
        // gets sent to this native callback before the JS callback has a chance to register. So we need to save that notification off,
        // then send it when the JS callback regsters & stop saving notifications after
        if (mSaveInitialNotification) {
            if (mInitialNotification == null) {
                RNPushUtil.logInfo("Saving initial notification");
                try {
                    mInitialNotification = RNPushUtil.convertPushNotificationToWritableMap(pushNotification);
                } catch (JSONException e) {
                    RNPushUtil.logError("Failed to convert initial notification to WritableMap:");
                    RNPushUtil.logError(Log.getStackTraceString(e));
                }
            }
        }
        else {
            try {
                if (this.mReactApplicationContext != null) {
                    this.mReactApplicationContext
                            .getJSModule(RCTDeviceEventEmitter.class)
                            .emit(ON_PUSH_NOTIFICATION_RECEIVED_EVENT, RNPushUtil.convertPushNotificationToWritableMap(pushNotification));
                }
            } catch (JSONException e) {
                RNPushUtil.logError("Failed to send onPushNotificationReceived event:");
                RNPushUtil.logError(Log.getStackTraceString(e));
            }
        }
    }

    protected void sendAndClearInitialNotification() {
        if (mInitialNotification != null) {
            if (this.mReactApplicationContext != null) {
                this.mReactApplicationContext
                        .getJSModule(RCTDeviceEventEmitter.class)
                        .emit(ON_PUSH_NOTIFICATION_RECEIVED_EVENT, mInitialNotification);
            }
            
            mInitialNotification = null;
        }

        mSaveInitialNotification = false;
    }
}
