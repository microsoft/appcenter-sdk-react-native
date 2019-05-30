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
                JsonElement element = documentWrapper.getDeserializedValue();
                if (element.isJsonPrimitive()) {
                    JsonPrimitive jsonPrimitive = element.getAsJsonPrimitive();
                    if (jsonPrimitive.isString()) {
                        promise.resolve(jsonPrimitive.getAsString());
                    } else if (jsonPrimitive.isNumber()) {
                        promise.resolve(jsonPrimitive.getAsNumber());
                    } else if (jsonPrimitive.isBoolean()) {
                        promise.resolve(jsonPrimitive.getAsBoolean());
                    }
                } else if (element.isJsonObject()) {
                    JsonObject jsonObject = element.getAsJsonObject();
                    WritableMap writableMap = convertJsonObjectToWritableMap(jsonObject);
                    promise.resolve(writableMap);
                } else if (element.isJsonArray()) {
                    JsonArray jsonArray = element.getAsJsonArray();
                    WritableArray writableArray = convertJsonArrayToWritableArray(jsonArray);
                    promise.resolve(writableArray);
                } else {
                    promise.resolve(null);
                }
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