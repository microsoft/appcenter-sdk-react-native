package com.microsoft.azure.mobile.react.mobilecenter;
import com.microsoft.azure.mobile.MobileCenter;

import com.microsoft.azure.mobile.CustomProperties;

public class CustomMobileCenter extends MobileCenter
{
    public static void setCustomPropertiesNew(CustomProperties properties){
        MobileCenter.setCustomProperties(properties);
    }
}
