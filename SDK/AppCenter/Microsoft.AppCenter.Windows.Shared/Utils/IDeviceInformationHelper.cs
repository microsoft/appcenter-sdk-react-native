// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace Microsoft.AppCenter.Utils
{
    /// <summary>
    /// Represents an object that is able to retrieve hardware and software information about the device running the SDK.
    /// </summary>
    public interface IDeviceInformationHelper
    {
        /// <summary>
        /// Gets the device information.
        /// </summary>
        /// <returns>Device object with fields populated appropriately</returns>
        Task<Ingestion.Models.Device> GetDeviceInformationAsync();
    }
}
