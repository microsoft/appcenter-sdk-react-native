// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Data.iOS.Bindings;
using System;

namespace Microsoft.AppCenter.Data.iOS
{
    public class RemoteOperationDelegate : MSRemoteOperationDelegate
    {
        public override void DataDidCompletedRemoteOperation(MSData data, string operation, MSDocumentMetadata documentMetadata, MSDataError error)
        {
            OnDataDidCompletedRemoteOperationAction.Invoke(operation, documentMetadata, error);
        }

        public Action<string, MSDocumentMetadata, MSDataError> OnDataDidCompletedRemoteOperationAction { get; set; }
    }
}