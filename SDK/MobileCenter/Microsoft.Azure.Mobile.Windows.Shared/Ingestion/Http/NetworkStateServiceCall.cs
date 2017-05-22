using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class NetworkStateServiceCall : ServiceCallDecorator
    {
        private readonly NetworkStateIngestion _networkIngestion;

        public NetworkStateServiceCall(IServiceCall decoratedApi, NetworkStateIngestion networkIngestion) : base(decoratedApi)
        {
            _networkIngestion = networkIngestion;
        }

        public override async Task ExecuteAsync()
        {
            try
            {
                await _networkIngestion.ExecuteCallAsync(DecoratedApi).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (exception is NetworkUnavailableException)
                {
                    return;
                }
                throw exception;
            }
        }
    }
}
