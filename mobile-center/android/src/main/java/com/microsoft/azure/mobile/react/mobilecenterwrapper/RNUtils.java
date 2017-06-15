package com.microsoft.azure.mobile.react.mobilecenterwrapper;

import com.facebook.react.bridge.ReadableArray;
import com.facebook.react.bridge.ReadableMap;
import com.facebook.react.bridge.ReadableMapKeySetIterator;
import com.facebook.react.bridge.ReadableType;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.HashMap;
import java.util.Map;

import com.microsoft.azure.mobile.MobileCenter;

public class RNUtils {

    public static CustomProperties toCustomProperties(ReadableMap readableMap) {
        CustomProperties properties = new CustomProperties();

        Iterator iterator = readableMap.entrySet().iterator();

        while (iterator.hasNext()) {
            readableMap.Entry pair = (readableMap.Entry)iterator.next();
            Object value = pair.getValue();

            final String key = (String) pair.getKey();
            final Pattern pattern = Pattern.compile("^[a-zA-Z][a-zA-Z0-9]*$");

            if (pattern.matcher(key).matches()) {
                if (value == null) {
                    //do nothing
                } else if (value instanceof Boolean) {
                    properties.set(key, (Boolean) value);
                } else if (value instanceof Integer) {
                    properties.set(key, (Integer) value);
                } else if (value instanceof String) {
                    properties.set(key, (String) value);
                } else if (value instanceof Date) {
                    properties.set(key, (Date) value);
                }
            }

            iterator.remove();
        }

        return properties;
    }
}
