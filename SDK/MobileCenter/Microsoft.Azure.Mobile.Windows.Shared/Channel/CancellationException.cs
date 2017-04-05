namespace Microsoft.Azure.Mobile
{
    public class CancellationException : MobileCenterException
    {
        public CancellationException() : base("Request cancelled because channel is disabled.") { }
    }
}
