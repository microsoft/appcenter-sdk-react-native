package com.microsoft.react.sonoma.crashes;

import android.util.Log;

import com.facebook.react.bridge.Arguments;
import com.facebook.react.bridge.WritableMap;
import com.microsoft.sonoma.core.ingestion.models.Device;
import com.microsoft.sonoma.crashes.model.ErrorReport;

import org.json.JSONException;
import org.json.JSONObject;
import org.json.JSONStringer;

public class RNSonomaCrashesUtils {
    private static final String LOG_TAG = "RNSonomaCrashes";

    public static void logError(String message) {
        Log.e(LOG_TAG, message);
    }

    public static void logInfo(String message) {
        Log.i(LOG_TAG, message);
    }

    public static WritableMap convertErrorReportToWritableMap(ErrorReport errorReport) throws JSONException {
        WritableMap errorReportMap = Arguments.createMap();
        errorReportMap.putString("id", errorReport.getId());
        errorReportMap.putString("threadName", errorReport.getThreadName());
        errorReportMap.putString("appErrorTime", "" + errorReport.getAppErrorTime().getTime());
        errorReportMap.putString("appStartTime", "" + errorReport.getAppStartTime().getTime());
        errorReportMap.putString("exception", Log.getStackTraceString(errorReport.getThrowable()));

        Device deviceInfo = errorReport.getDevice();
        JSONStringer jsonStringer = new JSONStringer();
        jsonStringer.object();
        deviceInfo.write(jsonStringer);
        jsonStringer.endObject();
        JSONObject deviceInfoJson = new JSONObject(jsonStringer.toString());
        WritableMap deviceInfoMap = RNSonomaUtils.convertJsonObjectToWritableMap(deviceInfoJson);

        errorReportMap.putMap("device", deviceInfoMap);
        return errorReportMap;
    }
}
