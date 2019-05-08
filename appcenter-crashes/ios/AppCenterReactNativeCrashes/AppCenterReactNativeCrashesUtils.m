// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#import "AppCenterReactNativeCrashesUtils.h"

#import <AppCenterCrashes/MSErrorReport.h>
#import <AppCenterCrashes/MSErrorAttachmentLog.h>
#import <AppCenterCrashes/MSErrorAttachmentLog+Utility.h>

#import <AppCenter/MSDevice.h>

NSArray* convertReportsToJS(NSArray* reports) {
    NSMutableArray* jsReadyReports = [[NSMutableArray alloc] init];
    [reports enumerateObjectsUsingBlock:^(MSErrorReport* report, NSUInteger idx, BOOL * stop) {
        [jsReadyReports addObject:convertReportToJS(report)];
    }];
    return jsReadyReports;
}

static NSString *const kMSSdkName = @"sdkName";
static NSString *const kMSSdkVersion = @"sdkVersion";
static NSString *const kMSModel = @"model";
static NSString *const kMSOemName = @"oemName";
static NSString *const kMSOsName = @"osName";
static NSString *const kMSOsVersion = @"osVersion";
static NSString *const kMSOsBuild = @"osBuild";
static NSString *const kMSOsApiLevel = @"osApiLevel";
static NSString *const kMSLocale = @"locale";
static NSString *const kMSTimeZoneOffset = @"timeZoneOffset";
static NSString *const kMSScreenSize = @"screenSize";
static NSString *const kMSAppVersion = @"appVersion";
static NSString *const kMSCarrierName = @"carrierName";
static NSString *const kMSCarrierCountry = @"carrierCountry";
static NSString *const kMSAppBuild = @"appBuild";
static NSString *const kMSAppNamespace = @"appNamespace";

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
        return nil;
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

NSArray* convertJSAttachmentsToNativeAttachments(NSArray* jsAttachments) {
    id attachmentLogs = [NSMutableArray new];
    for (NSDictionary *jsAttachment in jsAttachments) {
        id fileName = [jsAttachment objectForKey:@"fileName"];
        NSString *fileNameString = nil;
        if (fileName && [fileName isKindOfClass:[NSString class]]) {
            fileNameString = (NSString *) fileName;
        }

        // Check for text versus binary attachment.
        id text = [jsAttachment objectForKey:@"text"];
        if (text && [text isKindOfClass:[NSString class]]) {
            id attachmentLog = [MSErrorAttachmentLog attachmentWithText:text filename:fileNameString];
            [attachmentLogs addObject:attachmentLog];
        }
        else {
            id data = [jsAttachment objectForKey:@"data"];
            if (data && [data isKindOfClass:[NSString class]]) {

                // Binary data is passed as a base64 string from Javascript, decode it.
                NSData *decodedData = [[NSData alloc] initWithBase64EncodedString:data options:NSDataBase64DecodingIgnoreUnknownCharacters];
                id contentType = [jsAttachment objectForKey:@"contentType"];
                NSString *contentTypeString = nil;
                if (contentType && [contentType isKindOfClass:[NSString class]]) {
                    contentTypeString = (NSString *) contentType;
                }
                id attachmentLog = [MSErrorAttachmentLog attachmentWithBinary:decodedData filename:fileNameString contentType:contentTypeString];
                [attachmentLogs addObject:attachmentLog];
            }
        }
    }
    return attachmentLogs;
}
