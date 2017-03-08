namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class NetworkStateServiceCall : ServiceCallDecorator
    {
        private readonly NetworkStateIngestion _networkIngestion;

        public NetworkStateServiceCall(IServiceCall decoratedApi, NetworkStateIngestion networkIngestion) : base(decoratedApi)
        {
            _networkIngestion = networkIngestion;
        }

        public override void Execute()
        {
            _networkIngestion.ExecuteCallAsync(DecoratedApi).ContinueWith(completedTask =>
            {
                if (completedTask.IsFaulted)
                {
                    var innerException = completedTask.Exception?.InnerException;
                    if (innerException is NetworkUnavailableException)
                    {
                        return;
                    }
                    ServiceCallFailedCallback?.Invoke(innerException as IngestionException);
                }
                else
                {
                    ServiceCallSucceededCallback?.Invoke();
                }
            });
        }
    }
}
