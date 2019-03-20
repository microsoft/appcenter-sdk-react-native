// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#import <Foundation/Foundation.h>

@class MSErrorReport;

NSDictionary* convertReportToJS(MSErrorReport* report);
NSArray* convertReportsToJS(NSArray* reports);
NSArray* convertJSAttachmentsToNativeAttachments(NSArray* jsAttachments);
