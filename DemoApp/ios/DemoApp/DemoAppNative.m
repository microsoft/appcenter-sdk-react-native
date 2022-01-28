// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#import "DemoAppNative.h"

#if __has_include(<React/RCTAssert.h>)
#import <React/RCTAssert.h>
#import <React/RCTBridgeModule.h>
#import <React/RCTConvert.h>
#import <React/RCTEventDispatcher.h>
#import <React/RCTRootView.h>
#import <React/RCTUtils.h>
#else
#import "RCTAssert.h"
#import "RCTBridgeModule.h"
#import "RCTConvert.h"
#import "RCTEventDispatcher.h"
#import "RCTRootView.h"
#import "RCTUtils.h"
#endif

@interface DemoAppNative () <RCTBridgeModule>

@property NSMutableArray *buffers;

@property size_t allocated;

@end

@implementation DemoAppNative

static int const blockSize = 256 * 1024 * 1024;
static NSString* const kAppCenterSecretKey = @"AppSecret";
static NSString* const kAppCenterStartAutomaticallyKey = @"StartAutomatically";
static NSString* const kManualSessionTrackerEnabled = @"ManualSessionTrackerEnabled";

- (instancetype) init {
  self = [super init];
  if (self) {
    _buffers = [NSMutableArray new];
    _allocated = 0;
  }
  return self;
}

RCT_EXPORT_MODULE();

RCT_EXPORT_METHOD(configureStartup:(NSString*)secretString
                                  :(BOOL)startAutomatically)
{
  NSUserDefaults *userDefaults = [NSUserDefaults standardUserDefaults];
  if (secretString == nil) {
    secretString = @"";
  }
  [userDefaults setObject:secretString forKey:kAppCenterSecretKey];
  [userDefaults setBool:startAutomatically forKey:kAppCenterStartAutomaticallyKey];
}

RCT_EXPORT_METHOD(saveManualSessionTrackerState:(BOOL)state)
 {
   NSUserDefaults *userDefaults = [NSUserDefaults standardUserDefaults];
   [userDefaults setBool:state forKey:kManualSessionTrackerEnabled];
 }

RCT_EXPORT_METHOD(getManualSessionTrackerState:(RCTPromiseResolveBlock)resolve rejecter:(RCTPromiseRejectBlock)reject)
{
  if ([[NSUserDefaults standardUserDefaults] boolForKey:kManualSessionTrackerEnabled]){
    resolve(@(1));
  } else {
    resolve(@(0));
  }
}

RCT_EXPORT_METHOD(generateTestCrash)
{
  int* p = 0;
  *p = 0;
}

RCT_EXPORT_METHOD(produceLowMemoryWarning)
{
  const size_t blockSize = 128 * 1024 * 1024;
  dispatch_after(dispatch_time(DISPATCH_TIME_NOW, 100 * NSEC_PER_MSEC), dispatch_get_main_queue(), ^{
    void *buffer = malloc(blockSize);
    memset(buffer, 42, blockSize);
    [self.buffers addObject:[NSValue valueWithPointer:buffer]];
    self.allocated += blockSize;
    NSLog(@"Allocated %zu MB", self.allocated / (1024 * 1024));
    [self produceLowMemoryWarning];
  });
}

+ (BOOL)requiresMainQueueSetup
{
  return YES;
}

@end
