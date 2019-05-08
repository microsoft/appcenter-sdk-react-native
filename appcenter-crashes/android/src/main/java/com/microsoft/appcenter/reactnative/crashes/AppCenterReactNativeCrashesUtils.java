// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.appcenter.reactnative.crashes;

import android.util.Log;

import com.facebook.react.bridge.Arguments;
import com.facebook.react.bridge.WritableArray;
import com.facebook.react.bridge.WritableMap;
import com.microsoft.appcenter.crashes.model.ErrorReport;
import com.microsoft.appcenter.ingestion.models.Device;

import org.json.JSONException;
import org.json.JSONObject;
import org.json.JSONStringer;

import java.util.Collection;

class AppCenterReactNativeCrashesUtils {
    private static final String LOG_TAG = "AppCenterCrashes";

    static void logError(String message) {
        Log.e(LOG_TAG, message);
    }

    static void logInfo(String message) {
        Log.i(LOG_TAG, message);
    }

    static void logDebug(String message) {
        Log.d(LOG_TAG, message);
    }

    static WritableMap convertErrorReportToWritableMap(ErrorReport errorReport) throws JSONException {
        if (errorReport == null) {
            return Arguments.createMap();
        }
        WritableMap errorReportMap = Arguments.createMap();
        errorReportMap.putString("id", errorReport.getId());
        errorReportMap.putString("threadName", errorReport.getThreadName());
        errorReportMap.putString("appErrorTime", "" + errorReport.getAppErrorTime().getTime());
        errorReportMap.putString("appStartTime", "" + errorReport.getAppStartTime().getTime());
        
        //noinspection ThrowableResultOfMethodCallIgnored
        Throwable error = errorReport.getThrowable();
        if (error != null) {
            errorReportMap.putString("exception", Log.getStackTraceString(error));
            errorReportMap.putString("exceptionReason", error.getMessage());
        }
        /* Convert device info. */
        Device deviceInfo = errorReport.getDevice();
        WritableMap deviceInfoMap;
        if (deviceInfo != null) {
            JSONStringer jsonStringer = new JSONStringer();
            jsonStringer.object();
            deviceInfo.write(jsonStringer);
            jsonStringer.endObject();
            JSONObject deviceInfoJson = new JSONObject(jsonStringer.toString());
            deviceInfoMap = RNUtils.convertJsonObjectToWritableMap(deviceInfoJson);
        } else {

            /* TODO investigate why this can be null. */
            deviceInfoMap = Arguments.createMap();
        }
        errorReportMap.putMap("device", deviceInfoMap);
        return errorReportMap;
    }

    private static WritableArray convertErrorReportsToWritableArray(Collection<ErrorReport> errorReports) throws JSONException {
        WritableArray errorReportsArray = Arguments.createArray();

        for (ErrorReport report : errorReports) {
            errorReportsArray.pushMap(convertErrorReportToWritableMap(report));
        }

        return errorReportsArray;
    }

    static WritableArray convertErrorReportsToWritableArrayOrEmpty(Collection<ErrorReport> errorReports) {
        try {
            return convertErrorReportsToWritableArray(errorReports);
        } catch (JSONException e) {
            logError("Unable to serialize crash reports");
            logError(Log.getStackTraceString(e));
            return Arguments.createArray();
        }
    }

    static WritableMap convertErrorReportToWritableMapOrEmpty(ErrorReport errorReport) {
        try {
            return convertErrorReportToWritableMap(errorReport);
        } catch (JSONException e) {
            logError("Unable to serialize crash report");
            logError(Log.getStackTraceString(e));
            return Arguments.createMap();
        }
    }
}
