// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Com.Microsoft.Appcenter.Http;

namespace Microsoft.AppCenter
{
    internal class AndroidHttpClientAdapter : Java.Lang.Object, IAndroidHttpClient
    {
        private readonly IHttpNetworkAdapter _httpNetworkAdapter;

        public AndroidHttpClientAdapter(IHttpNetworkAdapter httpNetworkAdapter)
        {
            _httpNetworkAdapter = httpNetworkAdapter;
        }

        public IServiceCall CallAsync(string uri, string method, IDictionary<string, string> headers, IAndroidHttpClientCallTemplate callTemplate, IServiceCallback serviceCallback)
        {
            _httpNetworkAdapter.
            throw new System.NotImplementedException();
        }

        public void Close()
        {
            throw new System.NotImplementedException();
        }

        public void Reopen()
        {
            throw new System.NotImplementedException();
        }
    }
}
