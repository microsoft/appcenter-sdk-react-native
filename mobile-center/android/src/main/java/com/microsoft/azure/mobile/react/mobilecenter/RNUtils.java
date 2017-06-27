package com.microsoft.azure.mobile.react.mobilecenter;

import com.facebook.react.bridge.Arguments;
import com.facebook.react.bridge.ReadableMap;
import com.facebook.react.bridge.ReadableMapKeySetIterator;
import com.facebook.react.bridge.ReadableType;
import com.facebook.react.bridge.WritableMap;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.HashMap;
import java.util.Map;
import java.util.Iterator;
import java.util.regex.Pattern;
import java.util.Date;
import android.util.Log;

import com.microsoft.azure.mobile.MobileCenter;
import com.microsoft.azure.mobile.CustomProperties;

public class RNUtils {

    private static final String LOG_TAG = "RNMobileCenter";

    public static void logError(String message) {
        Log.e(LOG_TAG, message);
    }

    public static void logInfo(String message) {
        Log.i(LOG_TAG, message);
    }

    public static void logDebug(String message) {
        Log.d(LOG_TAG, message);
    }

    public static CustomProperties toCustomProperties(ReadableMap readableMap) {
        CustomProperties properties = new CustomProperties();

        ReadableMapKeySetIterator iterator = readableMap.keySetIterator();

        while (iterator.hasNextKey()) {
            String key = iterator.nextKey();
            ReadableType type = readableMap.getType(key);

            final Pattern pattern = Pattern.compile("^[a-zA-Z][a-zA-Z0-9]*$");
            if (pattern.matcher(key).matches()) {
                switch (type) {
                    case Null:
                        break;
                    case Boolean:
                        properties.set(key, readableMap.getBoolean(key));
                        break;
                    case Number:
                        properties.set(key, readableMap.getInt(key));
                        break;
                    case String:
                        properties.set(key, readableMap.getString(key));
                        break;
                }
            } else {
                logError(String.format("Unable to parse key %s as it is not in ^[a-zA-Z][a-zA-Z0-9]*$", key));
            }
        }

        return properties;
    }
}
