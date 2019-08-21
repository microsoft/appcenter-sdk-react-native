/*
 * Copyright (c) 2019 Microsoft Corporation
 */

#include <jni.h>
#include <string>
#include <stdio.h>
#include <string.h>
#include <vector>
#include "android/log.h"

#ifdef __cplusplus
extern "C"
{
#endif

#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wunused-parameter"
#pragma ide diagnostic ignored "OCDFAInspection"
#pragma clang diagnostic ignored "-Wmissing-noreturn"
#pragma ide diagnostic ignored "cppcoreguidelines-avoid-magic-numbers"

static std::vector<void*> data;

void
Java_com_demoapp_DemoAppNativeModule_nativeAllocateLargeBuffer(
        JNIEnv *env,
        jobject obj) {
    size_t size = 128 * 1024 * 1024;
    void *buffer = malloc(size);
    memset(buffer, 42, size);
    data.push_back(buffer);
}
#pragma clang diagnostic pop

#ifdef __cplusplus
}
#endif
