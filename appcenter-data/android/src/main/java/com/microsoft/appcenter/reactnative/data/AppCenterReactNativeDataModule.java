/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

package com.microsoft.appcenter.reactnative.data;

import android.app.Application;

import com.facebook.react.bridge.BaseJavaModule;
import com.facebook.react.bridge.Callback;
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
import com.microsoft.appcenter.data.models.DocumentMetadata;
import com.microsoft.appcenter.data.models.DocumentWrapper;
import com.microsoft.appcenter.data.models.Page;
import com.microsoft.appcenter.data.models.PaginatedDocuments;
import com.microsoft.appcenter.data.models.RemoteOperationListener;
import com.microsoft.appcenter.data.models.ReadOptions;
import com.microsoft.appcenter.data.models.WriteOptions;
import com.microsoft.appcenter.reactnative.shared.AppCenterReactNativeShared;
import com.microsoft.appcenter.utils.async.AppCenterConsumer;

import java.util.List;
import java.util.Map;
import java.util.UUID;
import java.util.concurrent.ConcurrentHashMap;

public class AppCenterReactNativeDataModule extends BaseJavaModule {

    private static final String READ_FAILED_ERROR_CODE = "ReadFailed";

    private static final String LIST_FAILED_ERROR_CODE = "ListFailed";

    private static final String CREATE_FAILED_ERROR_CODE = "CreateFailed";

    private static final String REMOVE_FAILED_ERROR_CODE = "RemoveFailed";

    private static final String REPLACE_FAILED_ERROR_CODE = "ReplaceFailed";

    private static final String DESERIALIZED_VALUE_KEY = "deserializedValue";

    private static final String JSON_VALUE_KEY = "jsonValue";

    private static final String ETAG_KEY = "eTag";

    private static final String LAST_UPDATED_DATE_KEY = "lastUpdatedDate";

    private static final String IS_FROM_DEVICE_CACHE_KEY = "isFromDeviceCache";

    private static final String ID_KEY = "id";

    private static final String PARTITION_KEY = "partition";

    private static final String ERROR_KEY = "errorMessage";

    private static final String PAGINATED_DOCUMENTS_ID_KEY = "paginatedDocumentsId";

    private static final String ITEMS_KEY = "items";

    private static final String CURRENT_PAGE_KEY = "currentPage";

    private static final String MESSAGE_KEY = "message";

    private final Map<String, PaginatedDocuments<JsonElement>> mPaginatedDocuments = new ConcurrentHashMap<>();

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
    public void isEnabled(final Promise promise) {
        Data.isEnabled().thenAccept(new AppCenterConsumer<Boolean>() {

            @Override
            public void accept(Boolean isEnabled) {
                promise.resolve(isEnabled);
            }
        });
    }

    @ReactMethod
    public void setEnabled(boolean enabled, final Promise promise) {
        Data.setEnabled(enabled).thenAccept(new AppCenterConsumer<Void>() {

            @Override
            public void accept(Void aVoid) {
                promise.resolve(null);
            }
        });
    }

    @ReactMethod
    public void setRemoteOperationListener(final Callback remoteCompletedListener) {
        RemoteOperationListener remoteOperationlistener = new RemoteOperationListener() {

            @Override
            public void onRemoteOperationCompleted(String operation, DocumentMetadata documentMetadata, DataException error) {
                remoteCompletedListener.invoke(operation, documentMetadata, error);
            }
        };
        Data.setRemoteOperationListener(remoteOperationlistener);

    }

    @ReactMethod
    public void read(String documentId, String partition, ReadableMap readOptionsMap, final Promise promise) {
        ReadOptions readOptions = AppCenterReactNativeDataUtils.getReadOptions(readOptionsMap);
        Data.read(documentId, JsonElement.class, partition, readOptions).thenAccept(new Consumer<JsonElement>(READ_FAILED_ERROR_CODE, promise));
    }

    @ReactMethod
    public void list(String partition, final Promise promise) {
        Data.list(JsonElement.class, partition).thenAccept(new AppCenterConsumer<PaginatedDocuments<JsonElement>>() {

            @Override
            public void accept(PaginatedDocuments<JsonElement> documentWrappers) {
                String paginatedDocumentsId = UUID.randomUUID().toString();
                mPaginatedDocuments.put(paginatedDocumentsId, documentWrappers);
                WritableMap paginatedDocumentsMap = new WritableNativeMap();
                WritableMap currentPageMap = new WritableNativeMap();
                WritableArray itemsArray = new WritableNativeArray();
                Page<JsonElement> currentPage = documentWrappers.getCurrentPage();
                if (currentPage.getError() != null) {
                    DataException dataException = currentPage.getError();
                    promise.reject(LIST_FAILED_ERROR_CODE, dataException.getMessage(), dataException);
                    return;
                }
                List<DocumentWrapper<JsonElement>> documents = currentPage.getItems();

                /* Add documents to WritableArray */
                for (DocumentWrapper<JsonElement> document : documents) {
                    addedDocumentToWritableArray(itemsArray, document);
                }
                currentPageMap.putArray(ITEMS_KEY, itemsArray);
                paginatedDocumentsMap.putMap(CURRENT_PAGE_KEY, currentPageMap);
                paginatedDocumentsMap.putString(PAGINATED_DOCUMENTS_ID_KEY, paginatedDocumentsId);
                promise.resolve(paginatedDocumentsMap);
            }
        });
    }

    @ReactMethod
    public void hasNextPage(String paginatedDocumentsId, Promise promise) {
        PaginatedDocuments<JsonElement> paginatedDocuments = mPaginatedDocuments.get(paginatedDocumentsId);
        if (paginatedDocuments == null || !paginatedDocuments.hasNextPage()) {
            close(paginatedDocumentsId);
            promise.resolve(false);
        } else {
            promise.resolve(true);
        }
    }

    @ReactMethod
    public void getNextPage(String paginatedDocumentsId, final Promise promise) {
        if (!mPaginatedDocuments.containsKey(paginatedDocumentsId) || mPaginatedDocuments.get(paginatedDocumentsId) == null) {
            close(paginatedDocumentsId);
            promise.reject(LIST_FAILED_ERROR_CODE, "No additional pages available");
            return;
        }
        PaginatedDocuments<JsonElement> paginatedDocuments = mPaginatedDocuments.get(paginatedDocumentsId);
        paginatedDocuments.getNextPage().thenAccept(new AppCenterConsumer<Page<JsonElement>>() {

            @Override
            public void accept(Page<JsonElement> page) {
                WritableMap pageMap = new WritableNativeMap();
                WritableArray itemsArray = new WritableNativeArray();
                if (page.getError() != null) {
                    DataException dataException = page.getError();
                    promise.reject(LIST_FAILED_ERROR_CODE, dataException.getMessage(), dataException);
                    return;
                }
                List<DocumentWrapper<JsonElement>> documents = page.getItems();

                /* Add documents to WritableArray */
                for (DocumentWrapper<JsonElement> document : documents) {
                    addedDocumentToWritableArray(itemsArray, document);
                }
                pageMap.putArray(ITEMS_KEY, itemsArray);
                promise.resolve(pageMap);
            }
        });
    }

    @ReactMethod
    public void close(String paginatedDocumentsId) {
        mPaginatedDocuments.remove(paginatedDocumentsId);
    }

    @ReactMethod
    public void create(String documentId, String partition, ReadableMap documentMap, ReadableMap writeOptionsMap, final Promise promise) {
        WriteOptions writeOptions = AppCenterReactNativeDataUtils.getWriteOptions(writeOptionsMap);
        JsonObject jsonObject = AppCenterReactNativeDataUtils.convertReadableMapToJsonObject(documentMap);
        Data.create(documentId, jsonObject, JsonElement.class, partition, writeOptions).thenAccept(new Consumer<JsonElement>(CREATE_FAILED_ERROR_CODE, promise));
    }

    @ReactMethod
    public void remove(String documentId, String partition, ReadableMap writeOptionsMap, final Promise promise) {
        WriteOptions writeOptions = AppCenterReactNativeDataUtils.getWriteOptions(writeOptionsMap);
        Data.delete(documentId, partition, writeOptions).thenAccept(new Consumer<Void>(REMOVE_FAILED_ERROR_CODE, promise));
    }

    @ReactMethod
    public void replace(String documentId, String partition, ReadableMap documentMap, ReadableMap writeOptionsMap, final Promise promise) {
        WriteOptions writeOptions = AppCenterReactNativeDataUtils.getWriteOptions(writeOptionsMap);
        JsonObject jsonObject = AppCenterReactNativeDataUtils.convertReadableMapToJsonObject(documentMap);
        Data.replace(documentId, jsonObject, JsonElement.class, partition, writeOptions).thenAccept(new Consumer<JsonElement>(REPLACE_FAILED_ERROR_CODE, promise));
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
            addDocumentWrapperMetaData(documentWrapper, jsDocumentWrapper);
            if (documentWrapper.getError() != null) {
                DataException dataException = documentWrapper.getError();
                mPromise.reject(mErrorCode, dataException.getMessage(), dataException, jsDocumentWrapper);
                return;
            }
            if (!(documentWrapper.getDeserializedValue() instanceof JsonElement)) {
                mPromise.resolve(jsDocumentWrapper);
                return;
            }
            JsonElement deserializedDocument = (JsonElement) documentWrapper.getDeserializedValue();
            AppCenterReactNativeDataUtils.putJsonElementToWritableMap(jsDocumentWrapper, DESERIALIZED_VALUE_KEY, deserializedDocument);
            mPromise.resolve(jsDocumentWrapper);
        }
    }

    private static <T> void addDocumentWrapperMetaData(DocumentWrapper<T> documentWrapper, WritableMap writableMap) {
        writableMap.putString(ETAG_KEY, documentWrapper.getETag());
        writableMap.putString(ID_KEY, documentWrapper.getId());
        writableMap.putString(PARTITION_KEY, documentWrapper.getPartition());

        /* Pass milliseconds back to JS object since `WritableMap` does not support `Date` as values. */
        writableMap.putDouble(LAST_UPDATED_DATE_KEY, documentWrapper.getLastUpdatedDate().getTime());
        writableMap.putBoolean(IS_FROM_DEVICE_CACHE_KEY, documentWrapper.isFromDeviceCache());
        if (documentWrapper.getJsonValue() == null) {
            writableMap.putNull(JSON_VALUE_KEY);
        } else {
            writableMap.putString(JSON_VALUE_KEY, documentWrapper.getJsonValue());
        }
    }

    private static void addedDocumentToWritableArray(WritableArray writableArray, DocumentWrapper<JsonElement> documentWrapper) {
        WritableMap jsDocumentWrapper = new WritableNativeMap();
        addDocumentWrapperMetaData(documentWrapper, jsDocumentWrapper);
        if (documentWrapper.getError() != null) {
            DataException dataException = documentWrapper.getError();
            WritableMap errorMap = new WritableNativeMap();
            errorMap.putString(MESSAGE_KEY, dataException.getMessage());
            jsDocumentWrapper.putMap(ERROR_KEY, errorMap);
        } else {
            JsonElement deserializedDocument = documentWrapper.getDeserializedValue();
            AppCenterReactNativeDataUtils.putJsonElementToWritableMap(jsDocumentWrapper, DESERIALIZED_VALUE_KEY, deserializedDocument);
        }
        writableArray.pushMap(jsDocumentWrapper);
    }
}