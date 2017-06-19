package com.microsoft.azure.mobile.react.push;

import android.app.Activity;
import android.app.AlertDialog;
import android.util.Log;
import android.widget.Toast;

import com.facebook.react.bridge.ReactApplicationContext;
import com.microsoft.azure.mobile.push.PushListener;
import com.microsoft.azure.mobile.push.PushNotification;

import org.json.JSONException;

import java.util.Map;

import static com.facebook.react.modules.core.DeviceEventManagerModule.*;

public class RNPushEventListener implements PushListener {

    private ReactApplicationContext mReactApplicationContext;

    private static final String ON_PUSH_NOTIFICATION_RECEIVED_EVENT = "MobileCenterPushNotificationReceived";

    public final void setReactApplicationContext(ReactApplicationContext reactApplicationContext) {
        this.mReactApplicationContext = reactApplicationContext;
    }

    @Override
    public void onPushNotificationReceived(Activity activity, PushNotification pushNotification) {
        RNPushUtil.logInfo("Push notification received");

        try {
            if(this.mReactApplicationContext != null) {
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
