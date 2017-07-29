#import "RNCrashesDelegate.h"

// Support React Native headers both in the React namespace, where they are in RN version 0.40+,
// and no namespace, for older versions of React Native
#if __has_include(<React/RCTEventDispatcher.h>)
#import <React/RCTEventDispatcher.h>
#else
#import "RCTEventDispatcher.h"
#endif

#import "RNCrashesUtils.h"

static NSString *ON_BEFORE_SENDING_EVENT = @"MobileCenterErrorReportOnBeforeSending";
static NSString *ON_SENDING_FAILED_EVENT = @"MobileCenterErrorReportOnSendingFailed";
static NSString *ON_SENDING_SUCCEEDED_EVENT = @"MobileCenterErrorReportOnSendingSucceeded";


@implementation RNCrashesDelegateBase

- (instancetype) init
{
    self.reports = [[NSMutableArray alloc] init];
    return self;
}

- (BOOL) crashes:(MSCrashes *)crashes shouldProcessErrorReport:(MSErrorReport *)errorReport
{
    // By default handle all reports and expose them all to JS.
    [self storeReportForJS: errorReport];
    return YES;
}

- (MSUserConfirmationHandler)shouldAwaitUserConfirmationHandler
{
    // Do not send anything until instructed to by JS
    return ^(NSArray<MSErrorReport *> *errorReports){
        return YES;
    };
}

- (void)storeReportForJS:(MSErrorReport *) report
{
    [self.reports addObject:report];
}

- (void) crashes:(MSCrashes *)crashes willSendErrorReport:(MSErrorReport *)errorReport
{
    [self.bridge.eventDispatcher sendAppEventWithName:ON_BEFORE_SENDING_EVENT body:convertReportToJS(errorReport)];
}

- (void) crashes:(MSCrashes *)crashes didSucceedSendingErrorReport:(MSErrorReport *)errorReport
{
    [self.bridge.eventDispatcher sendAppEventWithName:ON_SENDING_SUCCEEDED_EVENT body:convertReportToJS(errorReport)];
}

- (void) crashes:(MSCrashes *)crashes didFailSendingErrorReport:(MSErrorReport *)errorReport withError:(NSError *)sendError
{
    [self.bridge.eventDispatcher sendAppEventWithName:ON_SENDING_FAILED_EVENT body:convertReportToJS(errorReport)];
}

- (void) provideAttachments: (NSDictionary*) attachments
{
    self.attachments = attachments;
}

- (NSArray<MSErrorAttachmentLog *> *)attachmentsWithCrashes:(MSCrashes *)crashes
                                             forErrorReport:(MSErrorReport *)errorReport
{
    id attachmentLogs = [[NSMutableArray alloc] init];
    id attachmentsForErrorReport = [self.attachments objectForKey:[errorReport incidentIdentifier]];
    if (attachmentsForErrorReport && [attachmentsForErrorReport isKindOfClass:[NSArray class]]) {
        for (id attachment in (NSArray *) attachmentsForErrorReport) {
            if (attachment && [attachment isKindOfClass:[NSDictionary class]]) {
                NSDictionary *attachmentDict = (NSDictionary *) attachment;
                id fileName = [attachmentDict objectForKey:@"fileName"];
                NSString *fileNameString = nil;
                if (fileName && [fileName isKindOfClass:[NSString class]]) {
                    fileNameString = (NSString *) fileName;
                }

                // Check for text versus binary attachment.
                id text = [attachmentDict objectForKey:@"text"];
                if (text && [text isKindOfClass:[NSString class]]) {
                    id attachmentLog = [MSErrorAttachmentLog attachmentWithText:text filename:fileNameString];
                    [attachmentLogs addObject:attachmentLog];
                }
                else {
                    id data = [attachmentDict objectForKey:@"data"];
                    if (data && [data isKindOfClass:[NSString class]]) {

                        // Binary data is passed as a base64 string from Javascript, decode it.
                        NSData *decodedData = [[NSData alloc] initWithBase64EncodedString:data options:NSDataBase64DecodingIgnoreUnknownCharacters];
                        id contentType = [attachmentDict objectForKey:@"contentType"];
                        NSString *contentTypeString = nil;
                        if (contentType && [contentType isKindOfClass:[NSString class]]) {
                            contentTypeString = (NSString *) contentType;
                        }
                        id attachmentLog = [MSErrorAttachmentLog attachmentWithBinary:decodedData filename:fileNameString contentType:contentTypeString];
                        [attachmentLogs addObject:attachmentLog];
                    }
                }
            }
        }
    }
    return attachmentLogs;
}

- (NSArray<MSErrorReport *>*) getAndClearReports
{
    NSArray<MSErrorReport *>* result = self.reports;
    self.reports = [[NSMutableArray alloc] init];
    return result;
}

@end

@implementation RNCrashesDelegateAlwaysSend
- (BOOL) crashes:(MSCrashes *)crashes shouldProcessErrorReport:(MSErrorReport *)errorReport
{
    // Do not pass the report to JS, but do process them
    return YES;
}

- (MSUserConfirmationHandler)shouldAwaitUserConfirmationHandler
{
    // Do not wait for user confirmation, always send.
    return ^(NSArray<MSErrorReport *> *errorReports){
        return NO;
    };
}

@end
