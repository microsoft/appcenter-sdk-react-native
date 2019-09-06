// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Data.iOS.Bindings;
using System;

namespace Microsoft.AppCenter.Data.iOS
{
    public class RemoteOperationDelegate : MSRemoteOperationDelegate
    {
        public override void DataDidCompleteRemoteOperation(MSData data, string operation, MSDocumentMetadata documentMetadata, MSDataError error)
        {
            OnDataDidCompleteRemoteOperationAction.Invoke(operation, documentMetadata, error);
        }

        public Action<string, MSDocumentMetadata, MSDataError> OnDataDidCompleteRemoteOperationAction { get; set; }
    }
}