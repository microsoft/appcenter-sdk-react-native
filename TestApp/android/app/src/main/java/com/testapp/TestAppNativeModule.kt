package com.testapp

import android.content.Context
import android.content.SharedPreferences
import android.os.Handler
import android.os.Looper
import android.util.Log
import com.facebook.react.bridge.BaseJavaModule
import com.facebook.react.bridge.Promise
import com.facebook.react.bridge.ReactMethod
import com.microsoft.appcenter.crashes.model.TestCrashException
import com.microsoft.appcenter.reactnative.shared.AppCenterReactNativeShared
import com.microsoft.appcenter.analytics.Analytics
import java.util.concurrent.atomic.AtomicInteger

class TestAppNativeModule(context: Context) : BaseJavaModule() {

    companion object {
        private const val DEMO_APP_NATIVE = "TestAppNative"
        private const val APP_SECRET = "app_secret"
        private const val START_AUTOMATICALLY = "start_automatically"
        private const val MANUAL_SESSION_TRACKER_ENABLED_KEY = "manual_session_tracker_enabled"

        init {
            System.loadLibrary("native-lib")
        }

        fun initSecrets(context: Context) {
            val sharedPreferences = context.getSharedPreferences(DEMO_APP_NATIVE, Context.MODE_PRIVATE)
            val secretOverride = sharedPreferences.getString(APP_SECRET, null)
            AppCenterReactNativeShared.setAppSecret(secretOverride)
            val startAutomaticallyOverride = sharedPreferences.getBoolean(START_AUTOMATICALLY, true)
            AppCenterReactNativeShared.setStartAutomatically(startAutomaticallyOverride)
        }

        fun initManualSessionTrackerState(context: Context) {
            val sharedPreferences = context.getSharedPreferences(DEMO_APP_NATIVE, Context.MODE_PRIVATE)
            val isManualSessionTrackerEnabled = sharedPreferences.getBoolean(MANUAL_SESSION_TRACKER_ENABLED_KEY, false)
            if (isManualSessionTrackerEnabled) {
                Analytics.enableManualSessionTracker()
            }
        }
    }

    private val sharedPreferences: SharedPreferences = 
        context.getSharedPreferences(name, Context.MODE_PRIVATE)

    private external fun nativeAllocateLargeBuffer()

    override fun getName(): String {
        return DEMO_APP_NATIVE
    }

    @ReactMethod
    fun saveManualSessionTrackerState(state: Boolean) {
        sharedPreferences.edit()
            .putBoolean(MANUAL_SESSION_TRACKER_ENABLED_KEY, state)
            .apply()
    }

    @ReactMethod
    fun getManualSessionTrackerState(promise: Promise) {
        promise.resolve(if (sharedPreferences.getBoolean(MANUAL_SESSION_TRACKER_ENABLED_KEY, false)) 1 else 0)
    }

    @ReactMethod
    fun configureStartup(secretString: String?, startAutomatically: Boolean) {
        sharedPreferences.edit()
            .putString(APP_SECRET, secretString ?: "")
            .putBoolean(START_AUTOMATICALLY, startAutomatically)
            .apply()
    }

    @ReactMethod
    fun generateTestCrash() {
        throw TestCrashException()
    }

    @ReactMethod
    fun produceLowMemoryWarning() {
        val atomicInteger = AtomicInteger(0)
        val handler = Handler(Looper.getMainLooper())
        handler.post(object : Runnable {
            override fun run() {
                nativeAllocateLargeBuffer()
                Log.d("TestApp", "Memory allocated: ${atomicInteger.addAndGet(128)}MB")
                handler.post(this)
            }
        })
    }
}
