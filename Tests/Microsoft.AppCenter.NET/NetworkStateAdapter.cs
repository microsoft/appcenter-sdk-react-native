// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Ingestion.Http
{
    /*
     * This implementation offers a setter for IsConnected so that network changes can be simulated.
     */
    public class NetworkStateAdapter : INetworkStateAdapter
    {
        private bool _isConnected = true;

        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (_isConnected == value)
                {
                    return;
                }
                _isConnected = value;
                NetworkStatusChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler NetworkStatusChanged;
    }
}
