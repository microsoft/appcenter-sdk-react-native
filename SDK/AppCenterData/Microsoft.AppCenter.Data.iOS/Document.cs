// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Data
{
    /// <summary>
    /// A document coming back from CosmosDB.
    /// </summary>
    public partial class Document<T>
    {
        public static T DeserializeString(string objectString)
        {            
            return JsonConvert.DeserializeObject<T>(objectString);
        }
    }
}
