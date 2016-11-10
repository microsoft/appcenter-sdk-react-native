using System;
using Foundation;

namespace Microsoft.Azure.Mobile.Crashes
{
    using iOS.Bindings;

    public class ErrorBinaryAttachment
    {
        internal MSErrorBinaryAttachment internalBinaryAttachment { get; }

        internal ErrorBinaryAttachment(MSErrorBinaryAttachment internalBinaryErrorAttachment)
        {
            internalBinaryAttachment = internalBinaryErrorAttachment;
        }

        public string FileName
        { 
            get 
            {
                return internalBinaryAttachment.FileName; 
            }
        }

        public byte[] Data
        { 
            get 
            {
                return internalBinaryAttachment.Data.ToArray(); 
            }
        }

        public string ContentType
        { 
            get 
            {
                return internalBinaryAttachment.ContentType; 
            }
        }

    }
}
