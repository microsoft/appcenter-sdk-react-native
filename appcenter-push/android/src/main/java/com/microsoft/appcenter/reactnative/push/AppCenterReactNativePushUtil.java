//
// Source code recreated from a .class file by IntelliJ IDEA
// (powered by Fernflower decompiler)
//

package com.microsoft.appcenter.reactnative.push;

import android.util.Log;
import com.facebook.react.bridge.Arguments;
import com.facebook.react.bridge.WritableMap;
import com.microsoft.appcenter.push.PushNotification;
import java.util.Iterator;
import java.util.Map;
import java.util.Map.Entry;
import org.json.JSONException;

public class AppCenterReactNativePushUtil {
    private static final String LOG_TAG = "AppCenterPush";

    public AppCenterReactNativePushUtil() {
    }

    public static void logError(String message) {
        Log.e(LOG_TAG, message);
    }

    public static void logInfo(String message) {
        Log.i(LOG_TAG, message);
    }

    public static void logDebug(String message) {
        Log.d(LOG_TAG, message);
    }

    public static WritableMap convertPushNotificationToWritableMap(PushNotification pushNotification) throws JSONException {
        if (pushNotification == null) {
            return Arguments.createMap();
        } else {
            WritableMap pushNotificationMap = Arguments.createMap();
            String title = pushNotification.getTitle();
            String message = pushNotification.getMessage();
            Map customData = pushNotification.getCustomData();
            pushNotificationMap.putString("title", title);
            pushNotificationMap.putString("message", message);
            if (!customData.isEmpty()) {
                WritableMap cp = Arguments.createMap();
                Iterator iterator = customData.entrySet().iterator();

                while (iterator.hasNext()) {
                    Entry pair = (Entry) iterator.next();
                    Object value = pair.getValue();
                    cp.putString((String) pair.getKey(), (String) value);
                }

                iterator.remove();
                pushNotificationMap.putMap("customProperties", cp);
            }

            return pushNotificationMap;
        }
    }
}
