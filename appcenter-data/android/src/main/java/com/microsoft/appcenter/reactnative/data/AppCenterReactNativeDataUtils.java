// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.appcenter.reactnative.data;

import com.facebook.react.bridge.ReadableArray;
import com.facebook.react.bridge.ReadableMap;
import com.facebook.react.bridge.ReadableMapKeySetIterator;
import com.facebook.react.bridge.ReadableType;
import com.facebook.react.bridge.WritableArray;
import com.facebook.react.bridge.WritableMap;
import com.facebook.react.bridge.WritableNativeArray;
import com.facebook.react.bridge.WritableNativeMap;

import com.google.gson.JsonArray;
import com.google.gson.JsonElement;
import com.google.gson.JsonNull;
import com.google.gson.JsonObject;
import com.google.gson.JsonPrimitive;
import com.microsoft.appcenter.data.exception.DataException;
import com.microsoft.appcenter.data.models.DocumentMetadata;
import com.microsoft.appcenter.data.models.ReadOptions;
import com.microsoft.appcenter.data.models.WriteOptions;

public class AppCenterReactNativeDataUtils {

    private static final String TIME_TO_LIVE_KEY = "timeToLive";

    public static WritableMap convertJsonObjectToWritableMap(JsonObject jsonObject) {
        WritableMap writableMap = new WritableNativeMap();
        for (String key : jsonObject.keySet()) {
            JsonElement child = jsonObject.get(key);
            if (child.isJsonPrimitive()) {
                JsonPrimitive jsonPrimitive = child.getAsJsonPrimitive();
                if (jsonPrimitive.isString()) {
                    writableMap.putString(key, jsonPrimitive.getAsString());
                } else if (jsonPrimitive.isNumber()) {
                    writableMap.putDouble(key, jsonPrimitive.getAsDouble());
                } else if (jsonPrimitive.isBoolean()) {
                    writableMap.putBoolean(key, jsonPrimitive.getAsBoolean());
                }
            } else if (child.isJsonObject()) {
                writableMap.putMap(key, convertJsonObjectToWritableMap(child.getAsJsonObject()));
            } else if (child.isJsonArray()) {
                writableMap.putArray(key, convertJsonArrayToWritableArray(child.getAsJsonArray()));
            } else {
                writableMap.putNull(key);
            }
        }
        return writableMap;
    }

    public static WritableArray convertJsonArrayToWritableArray(JsonArray jsonArray) {
        WritableArray writableArray = new WritableNativeArray();
        if (!jsonArray.isJsonArray()) {
            return writableArray;
        }
        for (JsonElement jsonElement : jsonArray) {
            if (jsonElement.isJsonPrimitive()) {
                JsonPrimitive jsonPrimitive = jsonElement.getAsJsonPrimitive();
                if (jsonPrimitive.isString()) {
                    writableArray.pushString(jsonPrimitive.getAsString());
                } else if (jsonPrimitive.isNumber()) {
                    writableArray.pushDouble(jsonPrimitive.getAsDouble());
                } else if (jsonPrimitive.isBoolean()) {
                    writableArray.pushBoolean(jsonPrimitive.getAsBoolean());
                }
            } else if (jsonElement.isJsonObject()) {
                writableArray.pushMap(convertJsonObjectToWritableMap(jsonElement.getAsJsonObject()));
            } else if (jsonElement.isJsonArray()) {
                writableArray.pushArray(convertJsonArrayToWritableArray(jsonElement.getAsJsonArray()));
            } else {
                writableArray.pushNull();
            }
        }
        return writableArray;
    }

    public static JsonObject convertReadableMapToJsonObject(ReadableMap readableMap) {
        JsonObject jsonObject = new JsonObject();
        if (readableMap == null) {
            return jsonObject;
        }
        ReadableMapKeySetIterator iterator = readableMap.keySetIterator();
        while (iterator.hasNextKey()) {
            String key = iterator.nextKey();
            ReadableType type = readableMap.getType(key);
            switch (type) {
                case Number:
                    jsonObject.addProperty(key, readableMap.getDouble(key));
                    break;
                case String:
                    jsonObject.addProperty(key, readableMap.getString(key));
                    break;
                case Boolean:
                    jsonObject.addProperty(key, readableMap.getBoolean(key));
                    break;
                case Map:
                    jsonObject.add(key, convertReadableMapToJsonObject(readableMap.getMap(key)));
                    break;
                case Array:
                    jsonObject.add(key, convertReadableArrayToJsonArray(readableMap.getArray(key)));
                    break;
                default:
                    jsonObject.add(key, JsonNull.INSTANCE);
                    break;
            }
        }
        return jsonObject;
    }

    public static JsonArray convertReadableArrayToJsonArray(ReadableArray readableArray) {
        JsonArray jsonArray = new JsonArray();
        if (readableArray == null) {
            return jsonArray;
        }
        for (int i = 0; i < readableArray.size(); i++) {
            ReadableType type = readableArray.getType(i);
            switch (type) {
                case Number:
                    jsonArray.add(readableArray.getDouble(i));
                    break;
                case String:
                    jsonArray.add(readableArray.getString(i));
                    break;
                case Boolean:
                    jsonArray.add(readableArray.getBoolean(i));
                    break;
                case Map:
                    jsonArray.add(convertReadableMapToJsonObject(readableArray.getMap(i)));
                    break;
                case Array:
                    jsonArray.add(convertReadableArrayToJsonArray(readableArray.getArray(i)));
                    break;
                default:
                    jsonArray.add(JsonNull.INSTANCE);
                    break;
            }
        }
        return jsonArray;
    }

    public static void putJsonElementToWritableMap(WritableMap writableMap, String key, JsonElement jsonElement) {
        if (jsonElement == null || jsonElement.isJsonNull()) {
            writableMap.putNull(key);
        } else if (jsonElement.isJsonPrimitive()) {
            JsonPrimitive jsonPrimitive = jsonElement.getAsJsonPrimitive();
            if (jsonPrimitive.isString()) {
                writableMap.putString(key, jsonPrimitive.getAsString());
            } else if (jsonPrimitive.isNumber()) {
                writableMap.putDouble(key, jsonPrimitive.getAsDouble());
            } else if (jsonPrimitive.isBoolean()) {
                writableMap.putBoolean(key, jsonPrimitive.getAsBoolean());
            }
        } else if (jsonElement.isJsonObject()) {
            JsonObject jsonObject = jsonElement.getAsJsonObject();
            writableMap.putMap(key, AppCenterReactNativeDataUtils.convertJsonObjectToWritableMap(jsonObject));
        } else if (jsonElement.isJsonArray()) {
            JsonArray jsonArray = jsonElement.getAsJsonArray();
            writableMap.putArray(key, AppCenterReactNativeDataUtils.convertJsonArrayToWritableArray(jsonArray));
        }
    }

    public static ReadOptions getReadOptions(ReadableMap readOptionsMap) {
        ReadOptions readOptions;
        if (readOptionsMap != null && readOptionsMap.hasKey(TIME_TO_LIVE_KEY)) {
            readOptions = new ReadOptions(readOptionsMap.getInt(TIME_TO_LIVE_KEY));
        } else {
            readOptions = new ReadOptions();
        }
        return readOptions;
    }

    public static WriteOptions getWriteOptions(ReadableMap writeOptionsMap) {
        WriteOptions writeOptions;
        if (writeOptionsMap != null && writeOptionsMap.hasKey(TIME_TO_LIVE_KEY)) {
            writeOptions = new WriteOptions(writeOptionsMap.getInt(TIME_TO_LIVE_KEY));
        } else {
            writeOptions = new WriteOptions();
        }
        return writeOptions;
    }

    public static WritableMap convertDocumentMetaDataToWritableMap(DocumentMetadata documentMetadata) {
        if (documentMetadata != null) {
            WritableMap documentMetaDataMap = new WritableNativeMap();
            documentMetaDataMap.putString("eTag", documentMetadata.getETag());
            documentMetaDataMap.putString("partition", documentMetadata.getPartition());
            documentMetaDataMap.putString("id", documentMetadata.getId());
            return documentMetaDataMap;
        }
        return null;
    }

    public static WritableMap convertDataExceptionToWritableMap(DataException dataException) {
        if (dataException != null) {
            WritableNativeMap errorMap = new WritableNativeMap();
            errorMap.putString("errorMessage", dataException.getMessage());
            return errorMap;
        }
        return null;
    }
}
