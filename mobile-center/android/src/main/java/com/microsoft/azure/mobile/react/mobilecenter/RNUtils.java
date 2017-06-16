package com.microsoft.azure.mobile.react.mobilecenterv2;

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

import com.microsoft.azure.mobile.MobileCenter;
import com.microsoft.azure.mobile.CustomProperties;

public class RNUtils {

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
            }
        }

        return properties;
    }
}
