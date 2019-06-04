/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

package com.microsoft.appcenter.reactnative.data;

import android.app.Application;

import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.bridge.WritableArray;
import com.facebook.react.bridge.WritableMap;
import com.facebook.react.bridge.WritableNativeArray;
import com.facebook.react.bridge.WritableNativeMap;

import com.google.gson.JsonArray;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonPrimitive;

import com.microsoft.appcenter.AppCenter;
import com.microsoft.appcenter.data.Data;
import com.microsoft.appcenter.data.models.DocumentWrapper;
import com.microsoft.appcenter.reactnative.shared.AppCenterReactNativeShared;
import com.microsoft.appcenter.utils.async.AppCenterConsumer;

public class AppCenterReactNativeDataModule extends BaseJavaModule {

    private static final String DESERIALIZED_VALUE_FIELD = "deserializedValue";

    private static final String JSON_VALUE_FIELD = "jsonValue";

    private static final String ETAG_FIELD = "eTag";

    private static final String LAST_UPDATED_DATE_FIELD = "lastUpdatedDate";

    private static final String IS_FROM_DEVICE_CACHE_FIELD = "isFromDeviceCache";

    private static final String ID_FIELD = "id";

    private static final String PARTITION_FIELD = "partition";

    public AppCenterReactNativeDataModule(Application application) {
        AppCenterReactNativeShared.configureAppCenter(application);
        if (AppCenter.isConfigured()) {
            AppCenter.start(Data.class);
        }
    }

    @Override
    public String getName() {
        return "AppCenterReactNativeData";
    }

    @ReactMethod
    public void read(String documentId, String partition, final Promise promise) {
        Data.read(documentId, JsonElement.class, partition).thenAccept(new AppCenterConsumer<DocumentWrapper<JsonElement>>() {

            @Override
            public void accept(DocumentWrapper<JsonElement> documentWrapper) {
                JsonElement deserializedValue = documentWrapper.getDeserializedValue();
                WritableMap jsDocumentWrapper = new WritableNativeMap();
                jsDocumentWrapper.putString(JSON_VALUE_FIELD, documentWrapper.getJsonValue());
                jsDocumentWrapper.putString(ETAG_FIELD, documentWrapper.getETag());
                jsDocumentWrapper.putDouble(LAST_UPDATED_DATE_FIELD, documentWrapper.getLastUpdatedDate().getTime());
                jsDocumentWrapper.putBoolean(IS_FROM_DEVICE_CACHE_FIELD, documentWrapper.isFromDeviceCache());
                jsDocumentWrapper.putString(ID_FIELD, documentWrapper.getId());
                jsDocumentWrapper.putString(PARTITION_FIELD, documentWrapper.getPartition());
                if (deserializedValue.isJsonPrimitive()) {
                    JsonPrimitive jsonPrimitive = deserializedValue.getAsJsonPrimitive();
                    if (jsonPrimitive.isString()) {
                        jsDocumentWrapper.putString(DESERIALIZED_VALUE_FIELD, jsonPrimitive.getAsString());
                    } else if (jsonPrimitive.isNumber()) {
                        jsDocumentWrapper.putDouble(DESERIALIZED_VALUE_FIELD, jsonPrimitive.getAsDouble());
                    } else if (jsonPrimitive.isBoolean()) {
                        jsDocumentWrapper.putBoolean(DESERIALIZED_VALUE_FIELD, jsonPrimitive.getAsBoolean());
                    }
                } else if (deserializedValue.isJsonObject()) {
                    JsonObject jsonObject = deserializedValue.getAsJsonObject();
                    WritableMap writableMap = convertJsonObjectToWritableMap(jsonObject);
                    jsDocumentWrapper.putMap(DESERIALIZED_VALUE_FIELD, writableMap);
                } else if (deserializedValue.isJsonArray()) {
                    JsonArray jsonArray = deserializedValue.getAsJsonArray();
                    WritableArray writableArray = convertJsonArrayToWritableArray(jsonArray);
                    jsDocumentWrapper.putArray(DESERIALIZED_VALUE_FIELD, writableArray);
                } else {
                    jsDocumentWrapper.putNull(DESERIALIZED_VALUE_FIELD);
                }
                promise.resolve(jsDocumentWrapper);
            }
        });
    }

    private static WritableMap convertJsonObjectToWritableMap(JsonObject jsonObject) {
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

    private static WritableArray convertJsonArrayToWritableArray(JsonArray jsonArray) {
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
}