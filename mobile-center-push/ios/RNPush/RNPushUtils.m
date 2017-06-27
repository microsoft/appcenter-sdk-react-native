#import "RNPushUtils.h"

@import MobileCenterPush.MSPushNotification;

NSDictionary* convertNotificationToJS(MSPushNotification* pushNotification) {
    if (pushNotification == nil) {
        return @{};
    }
    
    NSMutableDictionary * dict = [[NSMutableDictionary alloc] init];
    NSString *message = pushNotification.message;
    NSString *title = pushNotification.title;
    
    dict[@"message"] = message;
    dict[@"title"] = title;
    
    NSMutableDictionary *customData = [[NSMutableDictionary alloc] init];
    for (NSString *key in pushNotification.customData) {
        customData[key] = [pushNotification.customData objectForKey:key];
    }
    
    dict[@"customProperties"] = customData;
    
    return dict;
}
