package com.microsoft.sonoma.react.analytics;

import com.facebook.react.bridge.ReadableArray;
import com.facebook.react.bridge.ReadableMap;
import com.facebook.react.bridge.ReadableMapKeySetIterator;
import com.facebook.react.bridge.ReadableType;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.HashMap;
import java.util.Map;

public class RNSonomaUtils {
    public static JSONObject convertReadableMapToJsonObject(ReadableMap map) throws JSONException {
        JSONObject jsonObj = new JSONObject();
        ReadableMapKeySetIterator it = map.keySetIterator();
        while (it.hasNextKey()) {
            String key = it.nextKey();
            ReadableType type = map.getType(key);
            switch (type) {
                case Map:
                    jsonObj.put(key, convertReadableMapToJsonObject(map.getMap(key)));
                    break;
                case Array:
                    jsonObj.put(key, convertReadableArrayToJsonArray(map.getArray(key)));
                    break;
                case String:
                    jsonObj.put(key, map.getString(key));
                    break;
                case Number:
                    Double number = map.getDouble(key);
                    if ((number == Math.floor(number)) && !Double.isInfinite(number)) {
                        jsonObj.put(key, number.longValue());
                    } else {
                        jsonObj.put(key, number.doubleValue());
                    }

                    break;
                case Boolean:
                    jsonObj.put(key, map.getBoolean(key));
                    break;
                default:
                    jsonObj.put(key, null);
                    break;
            }
        }

        return jsonObj;
    }

    public static JSONArray convertReadableArrayToJsonArray(ReadableArray arr) throws JSONException {
        JSONArray jsonArr = new JSONArray();
        for (int i=0; i<arr.size(); i++) {
            ReadableType type = arr.getType(i);
            switch (type) {
                case Map:
                    jsonArr.put(convertReadableMapToJsonObject(arr.getMap(i)));
                    break;
                case Array:
                    jsonArr.put(convertReadableArrayToJsonArray(arr.getArray(i)));
                    break;
                case String:
                    jsonArr.put(arr.getString(i));
                    break;
                case Number:
                    Double number = arr.getDouble(i);
                    if ((number == Math.floor(number)) && !Double.isInfinite(number)) {
                        jsonArr.put(number.longValue());
                    } else {
                        jsonArr.put(number.doubleValue());
                    }

                    break;
                case Boolean:
                    jsonArr.put(arr.getBoolean(i));
                    break;
                case Null:
                    jsonArr.put(null);
                    break;
            }
        }

        return jsonArr;
    }

    public static Map<String, String> convertReadableMapToStringMap(ReadableMap map) throws JSONException {
        Map<String, String> stringMap = new HashMap<>();
        ReadableMapKeySetIterator it = map.keySetIterator();
        while (it.hasNextKey()) {
            String key = it.nextKey();
            ReadableType type = map.getType(key);
            switch (type) {
                case Map:
                    stringMap.put(key, convertReadableMapToJsonObject(map.getMap(key)).toString());
                    break;
                case Array:
                    stringMap.put(key, convertReadableArrayToJsonArray(map.getArray(key)).toString());
                    break;
                case String:
                    stringMap.put(key, map.getString(key));
                    break;
                case Number:
                    Double number = map.getDouble(key);
                    if ((number == Math.floor(number)) && !Double.isInfinite(number)) {
                        stringMap.put(key, "" + number.longValue());
                    } else {
                        stringMap.put(key, "" + number.doubleValue());
                    }

                    break;
                case Boolean:
                    stringMap.put(key, "" + map.getBoolean(key));
                    break;
                default:
                    stringMap.put(key, null);
                    break;
            }
        }

        return stringMap;
    }
}
