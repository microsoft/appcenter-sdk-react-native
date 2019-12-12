// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Foundation;
using Microsoft.AppCenter.iOS.Bindings;

namespace Microsoft.AppCenter
{
    internal class IosHttpClientAdapter : MSHttpClientProtocol
    {
        private readonly IHttpNetworkAdapter _httpNetworkAdapter;

        public IosHttpClientAdapter(IHttpNetworkAdapter httpNetworkAdapter)
        {
            _httpNetworkAdapter = httpNetworkAdapter;
        }

        public override void SendAsync(NSUrl url, NSString method, NSDictionary<NSString, NSString> headers, NSData data, MSHttpRequestCompletionHandler completionHandler)
        {
            var managedHeaders = new Dictionary<string, string>();
            foreach (KeyValuePair<NSObject, NSObject> header in headers)
            {
                managedHeaders[header.Key.ToString()] = header.Value.ToString();
            }
            _httpNetworkAdapter.SendAsync(url.ToString(), method, managedHeaders, data.ToString(), default).ContinueWith(t =>
            {
                var innerException = t.Exception?.InnerException;
                if (innerException is HttpException)
                {
                    var response = (innerException as HttpException).HttpResponse;
                    completionHandler(NSData.FromString(response.Content), new NSHttpUrlResponse(url, response.StatusCode, "1.1", new NSDictionary()), null);
                }
                else if (innerException != null)
                {
                    var userInfo = new NSDictionary();
                    userInfo["stackTrace"] = new NSString(innerException.ToString());
                    completionHandler(null, null, new NSError(new NSString(".NET SDK"), 1, userInfo));
                }
                else
                {
                    var response = t.Result;
                    completionHandler(NSData.FromString(response.Content), new NSHttpUrlResponse(url, response.StatusCode, "1.1", new NSDictionary()), null);
                }
            });
        }

        public override void SendAsync(NSUrl url, NSString method, NSDictionary<NSString, NSString> headers, NSData data, NSArray retryIntervals, bool compressionEnabled, MSHttpRequestCompletionHandler completionHandler)
        {
            SendAsync(url, method, headers, data, completionHandler);
        }

        public override void Pause()
        {
        }

        public override void Resume()
        {
        }

        public override void SetEnabled(bool enabled)
        {
        }
    }
}
