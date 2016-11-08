#import "RNCrashesUtils.h"

@import MobileCenterCrashes.MSErrorReport;
@import MobileCenter.MSDevice;

NSArray* convertReportsToJS(NSArray* reports) {
    NSMutableArray* jsReadyReports = [[NSMutableArray alloc] init];
    [reports enumerateObjectsUsingBlock:^(MSErrorReport* report, NSUInteger idx, BOOL * stop) {
        [jsReadyReports addObject:convertReportToJS(report)];
    }];
    return jsReadyReports;
}


static NSString *const kMSSdkName = @"sdk_name";
static NSString *const kMSSdkVersion = @"sdk_version";
static NSString *const kMSModel = @"model";
static NSString *const kMSOemName = @"oem_name";
static NSString *const kMSOsName = @"os_name";
static NSString *const kMSOsVersion = @"os_version";
static NSString *const kMSOsBuild = @"os_build";
static NSString *const kMSOsApiLevel = @"os_api_level";
static NSString *const kMSLocale = @"locale";
static NSString *const kMSTimeZoneOffset = @"time_zone_offset";
static NSString *const kMSScreenSize = @"screen_size";
static NSString *const kMSAppVersion = @"app_version";
static NSString *const kMSCarrierName = @"carrier_name";
static NSString *const kMSCarrierCountry = @"carrier_country";
static NSString *const kMSAppBuild = @"app_build";
static NSString *const kMSAppNamespace = @"app_namespace";

static NSDictionary *serializeDeviceToDictionary(MSDevice* device) {
    NSMutableDictionary *dict = [[NSMutableDictionary alloc] init];
    
    if (device.sdkName) {
        dict[kMSSdkName] = device.sdkName;
    }
    if (device.sdkVersion) {
        dict[kMSSdkVersion] = device.sdkVersion;
    }
    if (device.model) {
        dict[kMSModel] = device.model;
    }
    if (device.oemName) {
        dict[kMSOemName] = device.oemName;
    }
    if (device.osName) {
        dict[kMSOsName] = device.osName;
    }
    if (device.osVersion) {
        dict[kMSOsVersion] = device.osVersion;
    }
    if (device.osBuild) {
        dict[kMSOsBuild] = device.osBuild;
    }
    if (device.osApiLevel) {
        dict[kMSOsApiLevel] = device.osApiLevel;
    }
    if (device.locale) {
        dict[kMSLocale] = device.locale;
    }
    if (device.timeZoneOffset) {
        dict[kMSTimeZoneOffset] = device.timeZoneOffset;
    }
    if (device.screenSize) {
        dict[kMSScreenSize] = device.screenSize;
    }
    if (device.appVersion) {
        dict[kMSAppVersion] = device.appVersion;
    }
    if (device.carrierName) {
        dict[kMSCarrierName] = device.carrierName;
    }
    if (device.carrierCountry) {
        dict[kMSCarrierCountry] = device.carrierCountry;
    }
    if (device.appBuild) {
        dict[kMSAppBuild] = device.appBuild;
    }
    if (device.appNamespace) {
        dict[kMSAppNamespace] = device.appNamespace;
    }
    return dict;
}

NSDictionary* convertReportToJS(MSErrorReport* report) {
    if (report == nil) {
        return @{};
    }
    NSMutableDictionary * dict = [[NSMutableDictionary alloc] init];
    NSString * identifier = [report incidentIdentifier];
    if (identifier) {
        dict[@"id"] = identifier;
    }
    
    NSUInteger processIdentifier = [report appProcessIdentifier];
    dict[@"appProcessIdentifier"] = @(processIdentifier);
    
    NSTimeInterval startTime = [[report appStartTime] timeIntervalSince1970];
    if (startTime) {
      dict[@"appStartTime"] = @(startTime);
    }
    NSTimeInterval errTime = [[report appErrorTime] timeIntervalSince1970];
    if (errTime) {
      dict[@"appErrorTime"] = @(errTime);
    }

    NSString * exceptionName = [report exceptionName];
    if (exceptionName) {
      dict[@"exceptionName"] = exceptionName;
    }
    NSString * exceptionReason = [report exceptionReason];
    if (exceptionReason) {
      dict[@"exceptionReason"] = exceptionReason;
    }

    NSString * signal = [report signal];
    if (signal) {
      dict[@"signal"] = signal;
    }

    dict[@"device"] = serializeDeviceToDictionary([report device]);

    return dict;
}
