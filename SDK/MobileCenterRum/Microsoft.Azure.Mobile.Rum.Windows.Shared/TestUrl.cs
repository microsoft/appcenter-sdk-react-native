namespace Microsoft.Azure.Mobile.Rum
{
    internal class TestUrl
    {
        internal string Url { get; set; }

        internal string RequestId { get; set; }

        internal string Object { get; set; }

        internal string Conn { get; set; }

        internal long Result { get; set; }

        public override string ToString()
        {
            return $"{nameof(Url)}: {Url}, {nameof(RequestId)}: {RequestId}, {nameof(Object)}: {Object}, {nameof(Conn)}: {Conn}, {nameof(Result)}: {Result}";
        }
    }
}
