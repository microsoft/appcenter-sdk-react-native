namespace Microsoft.Azure.Mobile.Utils
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
        Ingestion.Models.Device GetDeviceInformation();
    }
}
