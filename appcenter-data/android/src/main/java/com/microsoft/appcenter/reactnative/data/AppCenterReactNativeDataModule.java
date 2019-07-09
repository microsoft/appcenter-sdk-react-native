/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

package com.microsoft.appcenter.reactnative.data;

import android.app.Application;

import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.Promise;
import com.facebook.react.bridge.ReactMethod;
import com.facebook.react.bridge.ReadableMap;
import com.facebook.react.bridge.WritableArray;
import com.facebook.react.bridge.WritableMap;
import com.facebook.react.bridge.WritableNativeArray;
import com.facebook.react.bridge.WritableNativeMap;

import com.google.gson.JsonElement;
import com.google.gson.JsonObject;

import com.microsoft.appcenter.AppCenter;
import com.microsoft.appcenter.data.Data;
import com.microsoft.appcenter.data.exception.DataException;
import com.microsoft.appcenter.data.models.DocumentWrapper;
import com.microsoft.appcenter.data.models.Page;
import com.microsoft.appcenter.data.models.PaginatedDocuments;
import com.microsoft.appcenter.data.models.ReadOptions;
import com.microsoft.appcenter.data.models.WriteOptions;
import com.microsoft.appcenter.reactnative.shared.AppCenterReactNativeShared;
import com.microsoft.appcenter.utils.async.AppCenterConsumer;

import java.util.List;

public class AppCenterReactNativeDataModule extends BaseJavaModule {

    private static final String DESERIALIZED_VALUE_KEY = "deserializedValue";

    private static final String JSON_VALUE_KEY = "jsonValue";

    private static final String ETAG_KEY = "eTag";

    private static final String LAST_UPDATED_DATE_KEY = "lastUpdatedDate";

    private static final String IS_FROM_DEVICE_CACHE_KEY = "isFromDeviceCache";

    private static final String ID_KEY = "id";

    private static final String PARTITION_KEY = "partition";

    private PaginatedDocuments<JsonElement> mPaginatedDocuments;

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
    public void read(String documentId, String partition, ReadableMap readOptionsMap, final Promise promise) {
        ReadOptions readOptions = AppCenterReactNativeDataUtils.getReadOptions(readOptionsMap);
        Data.read(documentId, JsonElement.class, partition, readOptions).thenAccept(new Consumer<JsonElement>("Read failed", promise));
    }

    @ReactMethod
    public void list(String partition, final Promise promise) {
        Data.list(JsonElement.class, partition).thenAccept(new AppCenterConsumer<PaginatedDocuments<JsonElement>>() {

            @Override
            public void accept(PaginatedDocuments<JsonElement> documentWrappers) {
                mPaginatedDocuments = documentWrappers;
                WritableMap PaginatedDocumentsMap = new WritableNativeMap();
                WritableMap currentPageMap = new WritableNativeMap();
                WritableArray itemsArray = new WritableNativeArray();
                Page<JsonElement> currentPage = mPaginatedDocuments.getCurrentPage();
                List<DocumentWrapper<JsonElement>> documents = currentPage.getItems();

                /* Add documents to WritableArray */
                for (DocumentWrapper<JsonElement> document : documents) {
                    JsonElement deserializedDocument = document.getDeserializedValue();
                    AppCenterReactNativeDataUtils.pushJsonElementToWritableArray(itemsArray, deserializedDocument);
                }
                currentPageMap.putArray("items", itemsArray);
                PaginatedDocumentsMap.putMap("currentPage", currentPageMap);
                promise.resolve(PaginatedDocumentsMap);
            }
        });
    }

    @ReactMethod
    public void hasNextPage(final Promise promise) {
        if (mPaginatedDocuments == null || !mPaginatedDocuments.hasNextPage()) {
            promise.resolve(false);
        }
        promise.resolve(true);
    }

    @ReactMethod
    public void getNextPage(final Promise promise) {
        if (mPaginatedDocuments == null || !mPaginatedDocuments.hasNextPage()) {
            promise.resolve(null);
        }
        mPaginatedDocuments.getNextPage().thenAccept(new AppCenterConsumer<Page<JsonElement>>() {

            @Override
            public void accept(Page<JsonElement> page) {
                WritableArray itemsArray = new WritableNativeArray();
                List<DocumentWrapper<JsonElement>> documents = page.getItems();

                /* Add documents to WritableArray */
                for (DocumentWrapper<JsonElement> document : documents) {
                    JsonElement deserializedDocument = document.getDeserializedValue();
                    AppCenterReactNativeDataUtils.pushJsonElementToWritableArray(itemsArray, deserializedDocument);
                }
                promise.resolve(itemsArray);
            }
        });
    }

    @ReactMethod
    public void create(String documentId, String partition, ReadableMap documentMap, ReadableMap writeOptionsMap, final Promise promise) {
        WriteOptions writeOptions = AppCenterReactNativeDataUtils.getWriteOptions(writeOptionsMap);
        JsonObject jsonObject = AppCenterReactNativeDataUtils.convertReadableMapToJsonObject(documentMap);
        Data.create(documentId, jsonObject, JsonElement.class, partition, writeOptions).thenAccept(new Consumer<JsonElement>("Create failed", promise));
    }

    @ReactMethod
    public void remove(String documentId, String partition, ReadableMap writeOptionsMap, final Promise promise) {
        WriteOptions writeOptions = AppCenterReactNativeDataUtils.getWriteOptions(writeOptionsMap);
        Data.delete(documentId, partition, writeOptions).thenAccept(new Consumer<Void>("Delete failed", promise));
    }

    @ReactMethod
    public void replace(String documentId, String partition, ReadableMap documentMap, ReadableMap writeOptionsMap, final Promise promise) {
        WriteOptions writeOptions = AppCenterReactNativeDataUtils.getWriteOptions(writeOptionsMap);
        JsonObject jsonObject = AppCenterReactNativeDataUtils.convertReadableMapToJsonObject(documentMap);
        Data.replace(documentId, jsonObject, JsonElement.class, partition, writeOptions).thenAccept(new Consumer<JsonElement>("Replace failed", promise));
    }

    private class Consumer<T> implements AppCenterConsumer<DocumentWrapper<T>> {

        private Promise mPromise;

        private String mErrorCode;

        private Consumer(String errorCode, Promise promise) {
            mPromise = promise;
            mErrorCode = errorCode;
        }

        @Override
        public void accept(DocumentWrapper<T> documentWrapper) {
            WritableMap jsDocumentWrapper = new WritableNativeMap();
            jsDocumentWrapper.putString(ETAG_KEY, documentWrapper.getETag());
            jsDocumentWrapper.putString(ID_KEY, documentWrapper.getId());
            jsDocumentWrapper.putString(PARTITION_KEY, documentWrapper.getPartition());

            /* Pass milliseconds back to JS object since `WritableMap` does not support `Date` as values. */
            jsDocumentWrapper.putDouble(LAST_UPDATED_DATE_KEY, documentWrapper.getLastUpdatedDate().getTime());
            jsDocumentWrapper.putBoolean(IS_FROM_DEVICE_CACHE_KEY, documentWrapper.isFromDeviceCache());
            jsDocumentWrapper.putString(JSON_VALUE_KEY, documentWrapper.getJsonValue());
            if (documentWrapper.getError() != null) {
                DataException dataException = documentWrapper.getError();
                mPromise.reject(mErrorCode, dataException.getMessage(), dataException, jsDocumentWrapper);
                return;
            }
            if (!(documentWrapper.getDeserializedValue() instanceof JsonElement)) {
                mPromise.resolve(jsDocumentWrapper);
                return;
            }
            JsonElement deserializedValue = (JsonElement) documentWrapper.getDeserializedValue();
            AppCenterReactNativeDataUtils.putJsonElementToWritableMap(jsDocumentWrapper, DESERIALIZED_VALUE_KEY, deserializedValue);
            mPromise.resolve(jsDocumentWrapper);
        }
    }
}