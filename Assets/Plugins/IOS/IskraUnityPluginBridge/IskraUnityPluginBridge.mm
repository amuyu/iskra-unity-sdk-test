//
//  IskraUnityPluginBridge.m
//  IskraUnityPluginBridge
//
//  Created by amuyu on 2022/11/25.
//

#import <Foundation/Foundation.h>
#import <IskraUnityPlugin/IskraUnityPlugin-Swift.h>


static NSString * const NSStringFromCString(const char *string)
{
    if (string != NULL) {
        return [NSString stringWithUTF8String:string];
    } else {
        return nil;
    }
}

static const char * const CStringFromNSString(NSString *string)
{
    if (string != NULL) {
        return strdup([string cStringUsingEncoding:NSUTF8StringEncoding]);
    } else {
        return NULL;
    }
}


extern "C" {

    void IskraPluginStoreAuthToken(const char* str) {
        NSString *strNS = NSStringFromCString(str);
        [[IskraUnityPlugin shared] StoreAuthTokenWithToken:strNS];
    }

    const char* IskraPluginGetAuthToken(const char* str) {
        NSString *strNS = NSStringFromCString(str);
        return CStringFromNSString([[IskraUnityPlugin shared] ReadAuthToken]);
    }

    void IskraPluginRemoveAuthToken(const char* str) {
        NSString *strNS = NSStringFromCString(str);
        [[IskraUnityPlugin shared] RemoveAuthToken];
    }

}
