using System;
namespace Microsoft.Azure.Mobile.Crashes
{
    using iOS.Bindings;

    public class ErrorAttachment
    {
        internal ErrorAttachment(MSErrorAttachment msAttachment)
        {
            //throw new NotImplementedException();
        }

        internal MSErrorAttachment ToMSErrorAttachment()
        {
            //throw new NotImplementedException();
            return new MSErrorAttachment();
        }
    }
}
