// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter.Channel
{
    /* Capability interface for having an app secret */
    public interface IAppSecretHolder
    {
        string AppSecret { get; }
    }
}
