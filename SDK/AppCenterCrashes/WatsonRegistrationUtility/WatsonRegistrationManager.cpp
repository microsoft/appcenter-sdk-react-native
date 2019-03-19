// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#include "pch.h"
#include "WatsonRegistrationManager.h"

using namespace WatsonRegistrationUtility;

WatsonRegistrationManager::WatsonRegistrationManager() {}

void WatsonRegistrationManager::Start(Platform::String^ appSecret)
{
	WerRegisterCustomMetadata(TEXT("VSMCAppSecret"), const_cast<LPWSTR>(appSecret->Data()));
}

void WatsonRegistrationManager::SetCorrelationId(Platform::String^ correlationId)
{
	WerRegisterCustomMetadata(TEXT("VSMCCorrelationId"), const_cast<LPWSTR>(correlationId->Data()));
}
