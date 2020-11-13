// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#import <Foundation/Foundation.h>

@class MSACErrorReport;

NSDictionary* convertReportToJS(MSACErrorReport* report);
NSArray* convertReportsToJS(NSArray* reports);
NSArray* convertJSAttachmentsToNativeAttachments(NSArray* jsAttachments);
