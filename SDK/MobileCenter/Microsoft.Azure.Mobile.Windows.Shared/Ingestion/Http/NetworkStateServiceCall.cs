using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class NetworkStateServiceCall : ServiceCallDecorator
    {
        private readonly NetworkStateIngestion _networkIngestion;

        public NetworkStateServiceCall(IServiceCall decoratedApi, NetworkStateIngestion networkIngestion) : base(decoratedApi)
        {
            _networkIngestion = networkIngestion;
        }
        public override event Action Succeeded;
        public override event ServiceCallFailedHandler Failed;
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
                    Failed?.Invoke(innerException as IngestionException);
                }
                else
                {
                    Succeeded?.Invoke();
                }
            });
        }
    }
}
