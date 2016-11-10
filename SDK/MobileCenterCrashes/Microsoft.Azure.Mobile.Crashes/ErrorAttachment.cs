using System;

namespace Microsoft.Azure.Mobile.Crashes
{
    public class ErrorAttachment
    {
        // Constructors
        public ErrorAttachment(string text)
        {
            throw new NotImplementedException();
        }

        public ErrorAttachment(byte[] data, string filename, string contentType)
        {
            throw new NotImplementedException();
        }

        public ErrorAttachment(string text, byte[] data, string filename, string contentType)
        {
            throw new NotImplementedException();
        }

        // Properties
        public string TextAttachment
        { 
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public ErrorBinaryAttachment BinaryAttachment
        { 
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        //// Operators
        //public override bool Equals(object obj)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
