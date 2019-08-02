// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#import "TestAppNative.h"

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

@interface TestAppNative () <RCTBridgeModule>

@property(atomic, strong) NSMutableArray *dataArray;

@end

@implementation TestAppNative

static int const blockSize = 256 * 1024 * 1024;
static NSString* const kAppCenterSecretKey = @"AppSecret";
static NSString* const kAppCenterStartAutomaticallyKey = @"StartAutomatically";

- (instancetype) init {
  self = [super init];
  if (self){
    self.dataArray = [[NSMutableArray alloc] init];
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

RCT_EXPORT_METHOD(generateTestCrash)
{
  int* p = 0;
  *p = 0;
}

RCT_EXPORT_METHOD(produceLowMemoryWarning)
{
  [_dataArray addObject:[self create256mbRandomNSData]];

  dispatch_after(dispatch_time(DISPATCH_TIME_NOW, 1 * NSEC_PER_SEC), dispatch_get_main_queue(), ^{
    [self produceLowMemoryWarning];
  });
}

-(NSData*)create256mbRandomNSData {
  void *bytes = malloc(blockSize);
  NSData *data = [NSData dataWithBytes:bytes length:blockSize];
  free(bytes);
  return data;
}

@end
