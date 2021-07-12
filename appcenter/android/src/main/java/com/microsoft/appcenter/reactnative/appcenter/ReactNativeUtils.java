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
import com.microsoft.appcenter.crashes.ingestion.models.Exception;

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

    static Exception toExceptionModel(ReadableMap readableMap) {
        Exception model = new Exception();
        try {
            String type = readableMap.getString("type");
            String message = readableMap.getString("message");
            if (type == null || type == "") {
                throw new Exception("Type value shouldn't be null ot empty"); 
            }
            if (message == null || message == "") {
                throw new Exception("Message value shouldn't be null or empty");
            }
            model.setType(type);
            model.setMessage(message);
            model.setStackTrace(readableMap.getString("stackTrace"));
            List<StackFrame> msFrames = new List<StackFrame>();
            Array<ReadableMap> frames = readableMap.getArray("frames"));
            for (ReadableMap frame in frames) {
                StackFrame msFrame = new StackFrame();
                msFrame.setFileName(frame.getString("fileName"));
                msFrame.selLineNumber(frame.getString("lineNumber"));
                msFrame.setMethodName(frame.getString("methodName"));
                msFrame.setClassName(frame.getString("className"));
                msFrame.setCode(frame.getString("code"));
                msFrame.setAddress(frame.getString("address"));
                msFrames.add(msFrame)
            }
            model.setFrames(frames);
        } catch (Exception e) {
            AppCenterReactNativeCrashesUtils.logError("Failed to get exception model");
            AppCenterReactNativeCrashesUtils.logError(Log.getStackTraceString(e));
        }
        return model;
    }

    static Collection<ErrorAttachmentLog> toCustomErrorAttachments(ReadableArray attachments) {
        Collection<ErrorAttachmentLog> attachmentLogs = new LinkedList<>();
        try {
            for (int i = 0; i < attachments.size(); i++) {
                ReadableMap jsAttachment = attachments.getMap(i);
                String fileName = null;
                if (jsAttachment.hasKey(FILE_NAME_FIELD)) {
                    fileName = jsAttachment.getString(FILE_NAME_FIELD);
                }
                if (jsAttachment.hasKey(TEXT_FIELD)) {
                    String text = jsAttachment.getString(TEXT_FIELD);
                    attachmentLogs.add(ErrorAttachmentLog.attachmentWithText(text, fileName));
                } else {
                    String encodedData = jsAttachment.getString(DATA_FIELD);
                    byte[] data = Base64.decode(encodedData, Base64.DEFAULT);
                    String contentType = jsAttachment.getString(CONTENT_TYPE_FIELD);
                    attachmentLogs.add(.attachmentWithBinary(data, fileName, contentType));
                }
            }
        } catch (Exception e) {
            AppCenterReactNativeCrashesUtils.logError("Failed to get error attachment for report: " + errorId);
            AppCenterReactNativeCrashesUtils.logError(Log.getStackTraceString(e));
        }
        return attachmentLogs;
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
