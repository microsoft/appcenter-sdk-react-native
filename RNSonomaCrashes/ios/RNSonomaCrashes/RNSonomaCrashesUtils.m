#import "RNSonomaCrashesUtils.h"

@import SonomaCrashes.SNMErrorReport;
@import SonomaCore.SNMDevice;

NSArray* convertReportsToJS(NSArray* reports) {
    NSMutableArray* jsReadyReports = [[NSMutableArray alloc] init];
    [reports enumerateObjectsUsingBlock:^(SNMErrorReport* report, NSUInteger idx, BOOL * stop) {
        [jsReadyReports addObject:convertReportToJS(report)];
    }];
    return jsReadyReports;
}


static NSString *const kSNMSdkName = @"sdk_name";
static NSString *const kSNMSdkVersion = @"sdk_version";
static NSString *const kSNMModel = @"model";
static NSString *const kSNMOemName = @"oem_name";
static NSString *const kSNMOsName = @"os_name";
static NSString *const kSNMOsVersion = @"os_version";
static NSString *const kSNMOsBuild = @"os_build";
static NSString *const kSNMOsApiLevel = @"os_api_level";
static NSString *const kSNMLocale = @"locale";
static NSString *const kSNMTimeZoneOffset = @"time_zone_offset";
static NSString *const kSNMScreenSize = @"screen_size";
static NSString *const kSNMAppVersion = @"app_version";
static NSString *const kSNMCarrierName = @"carrier_name";
static NSString *const kSNMCarrierCountry = @"carrier_country";
static NSString *const kSNMAppBuild = @"app_build";
static NSString *const kSNMAppNamespace = @"app_namespace";

static NSDictionary *serializeDeviceToDictionary(SNMDevice* device) {
    NSMutableDictionary *dict = [[NSMutableDictionary alloc] init];
    
    if (device.sdkName) {
        dict[kSNMSdkName] = device.sdkName;
    }
    if (device.sdkVersion) {
        dict[kSNMSdkVersion] = device.sdkVersion;
    }
    if (device.model) {
        dict[kSNMModel] = device.model;
    }
    if (device.oemName) {
        dict[kSNMOemName] = device.oemName;
    }
    if (device.osName) {
        dict[kSNMOsName] = device.osName;
    }
    if (device.osVersion) {
        dict[kSNMOsVersion] = device.osVersion;
    }
    if (device.osBuild) {
        dict[kSNMOsBuild] = device.osBuild;
    }
    if (device.osApiLevel) {
        dict[kSNMOsApiLevel] = device.osApiLevel;
    }
    if (device.locale) {
        dict[kSNMLocale] = device.locale;
    }
    if (device.timeZoneOffset) {
        dict[kSNMTimeZoneOffset] = device.timeZoneOffset;
    }
    if (device.screenSize) {
        dict[kSNMScreenSize] = device.screenSize;
    }
    if (device.appVersion) {
        dict[kSNMAppVersion] = device.appVersion;
    }
    if (device.carrierName) {
        dict[kSNMCarrierName] = device.carrierName;
    }
    if (device.carrierCountry) {
        dict[kSNMCarrierCountry] = device.carrierCountry;
    }
    if (device.appBuild) {
        dict[kSNMAppBuild] = device.appBuild;
    }
    if (device.appNamespace) {
        dict[kSNMAppNamespace] = device.appNamespace;
    }
    return dict;
}

NSDictionary* convertReportToJS(SNMErrorReport* report) {
    if (report == nil) {
        return @{};
    }
    return @{
             @"id": [report incidentIdentifier],
             @"appProcessIdentifier": @([report appProcessIdentifier]),
             @"appErrorTime": @([[report appErrorTime] timeIntervalSince1970]),
             @"appStartTime": @([[report appStartTime] timeIntervalSince1970]),
             
             @"exceptionName": [report exceptionName],
             @"exceptionReason": [report exceptionReason],
             @"signal": [report signal],
             @"device": serializeDeviceToDictionary([report device])
             };
}
