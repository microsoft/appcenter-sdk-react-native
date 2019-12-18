// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.appcenter.reactnative.appcenter;

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
import java.util.TimeZone;
import java.util.Locale;
import java.text.DateFormat;
import java.text.ParseException;
import java.text.SimpleDateFormat;

import android.util.Log;
import javax.annotation.Nullable;

import com.microsoft.appcenter.AppCenter;
import com.microsoft.appcenter.CustomProperties;

import static com.facebook.react.bridge.ReadableType.String;

public class ReactNativeUtils {

    private static final String LOG_TAG = "AppCenter";
    private static final String DATE_KEY = "RNDate";

    private static final ThreadLocal<DateFormat> DATETIME_FORMAT = new ThreadLocal<DateFormat>() {

        @Override
        protected DateFormat initialValue() {
            DateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.ms'Z'", Locale.US);
            dateFormat.setTimeZone(TimeZone.getTimeZone("UTC"));
            return dateFormat;
        }
    };

    // This code in general purpose for possible future use, but for now we only support
    // maps with single string field, used to encode a date
    private static Object toObject(@Nullable ReadableMap readableMap, String key) {
        if (readableMap == null) {
            return null;
        }

        Object result;
        ReadableType readableType = readableMap.getType(key);
        switch (readableType) {
            case Null:
                result = null;
                break;
            case Boolean:
                result = readableMap.getBoolean(key);
                break;
            case Number:
                // Can be int or double.
                double tmp = readableMap.getDouble(key);
                if (tmp == (int) tmp) {
                    result = (int) tmp;
                } else {
                    result = tmp;
                }
                break;
            case String:
                result = readableMap.getString(key);
                break;
            case Map:
                result = toMap(readableMap.getMap(key));
                break;
            //case Array:
            //    result = toList(readableMap.getArray(key));
            //    break;
            default:
                throw new IllegalArgumentException("Could not convert object with key: " + key + ".");
        }

        return result;
    }

    // This code in general purpose for possible future use, but for now we only support
    // maps with single string field, used to encode a date
    private static Map<String, Object> toMap(@Nullable ReadableMap readableMap) {
        if (readableMap == null) {
            return null;
        }

        ReadableMapKeySetIterator iterator = readableMap.keySetIterator();
        if (!iterator.hasNextKey()) {
            return null;
        }

        Map<String, Object> result = new HashMap<>();
        while (iterator.hasNextKey()) {
            String key = iterator.nextKey();
            result.put(key, toObject(readableMap, key));
        }

        return result;
    }

    private static Date tryToGetDate(@Nullable ReadableMap readableMap) {
        Map<String, Object> map = toMap(readableMap);
        //it is not allowed that there should be more than one date at a time for Custom Properties
        if (map == null || map.size() != 1){
            return null;
        }

        Date date = null;
        if (map.containsKey(DATE_KEY)){
            try {
                date = DATETIME_FORMAT.get().parse(map.get(DATE_KEY).toString());
            } catch (ParseException exception) {
                logError("Unable to parse date for value");
            }
        }
        return date;
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

    static CustomProperties toCustomProperties(ReadableMap readableMap) {
        CustomProperties properties = new CustomProperties();
        ReadableMapKeySetIterator keyIt = readableMap.keySetIterator();
        while (keyIt.hasNextKey()) {
            String key = keyIt.nextKey();
            ReadableMap valueObject = readableMap.getMap(key);
            String type = valueObject.getString("type");
            switch (type) {
                case "clear":
                    properties.clear(key);
                    break;

                case "string":
                    properties.set(key, valueObject.getString("value"));
                    break;

                case "number":
                    properties.set(key, valueObject.getDouble("value"));
                    break;

                case "boolean":
                    properties.set(key, valueObject.getBoolean("value"));
                    break;

                case "date-time":
                    properties.set(key, new Date((long) valueObject.getDouble("value")));
                    break;
            }
        }
        return properties;
    }
}
