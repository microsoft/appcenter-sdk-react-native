namespace Microsoft.AAppCenter
{
    public class CancellationException : AppCenterException
    {
        public CancellationException() : base("Request cancelled because channel is disabled.") { }
    }
}
