namespace Microsoft.Azure.Mobile.Crashes
{
    using AndroidBinaryAttachment = Com.Microsoft.Azure.Mobile.Crashes.Model.ErrorBinaryAttachment;

    public class ErrorBinaryAttachment
    {
        internal AndroidBinaryAttachment internalBinaryAttachment { get; }

        internal ErrorBinaryAttachment(AndroidBinaryAttachment internalBinaryErrorAttachment)
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
                return internalBinaryAttachment.GetData(); 
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
