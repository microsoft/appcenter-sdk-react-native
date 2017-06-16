package com.microsoft.azure.mobile.react.mobilecenterv2;
import com.microsoft.azure.mobile.MobileCenter;

import com.microsoft.azure.mobile.CustomProperties;

public class CustomMobileCenter extends MobileCenter
{
    public static void setCustomPropertiesNew(CustomProperties properties){
        MobileCenter.setCustomProperties(properties);
    }
}
