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
import java.util.TimeZone;
import java.util.Locale;
import java.text.DateFormat;
import java.text.ParseException;
import java.text.SimpleDateFormat;

import android.util.Log;
import javax.annotation.Nullable;

import com.microsoft.azure.mobile.MobileCenter;
import com.microsoft.azure.mobile.CustomProperties;

import static com.facebook.react.bridge.ReadableType.String;

public class RNUtils {

    private static final String LOG_TAG = "RNMobileCenter";
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
                    case Map:
                        //as ReadableMap doesn't have support for getting Date type pass it via Map object
                        Date resultDate = tryToGetDate(readableMap.getMap(key));
                        if (resultDate != null) {
                            properties.set(key, resultDate);
                        } else {
                            logError("Unable to get date for custom property key");
                        }
                        break;
                }
            } else {
                logError("Unable to parse key %s as it does not conform to the the valid key name pattern: ^[a-zA-Z][a-zA-Z0-9]*$");
            }
        }

        return properties;
    }
}
