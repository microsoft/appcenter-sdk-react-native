namespace Microsoft.Azure.Mobile.Crashes
{
    using AndroidBinaryAttachment = Com.Microsoft.Azure.Mobile.Crashes.Model.AndroidBinaryErrorAttachment;

    /// <summary>
    /// Binary attachment for error report.
    /// </summary>
    public class ErrorBinaryAttachment
    {
        internal AndroidBinaryAttachment internalBinaryAttachment { get; }

        internal ErrorBinaryAttachment(AndroidBinaryAttachment internalBinaryErrorAttachment)
        {
            internalBinaryAttachment = internalBinaryErrorAttachment;
        }

        /// <summary>
        /// Gets the binary data file name.
        /// </summary>
        /// <value>The name of the binary data file.</value>
        public string FileName
        { 
            get 
            {
                return internalBinaryAttachment.FileName; 
            }
        }

        /// <summary>
        /// Gets the binary data.
        /// </summary>
        /// <value>The binary data.</value>
        public byte[] Data
        { 
            get 
            {
                return internalBinaryAttachment.GetData(); 
            }
        }

        /// <summary>
        /// Gets the content type for the binary data.
        /// </summary>
        /// <value>The MIME type of the binary data.</value>
        public string ContentType
        { 
            get 
            {
                return internalBinaryAttachment.ContentType; 
            }
        }

    }
}
