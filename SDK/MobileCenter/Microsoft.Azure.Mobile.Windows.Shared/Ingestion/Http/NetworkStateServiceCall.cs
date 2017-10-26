using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Ingestion.Http
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
            await _networkIngestion.ExecuteCallAsync(DecoratedApi).ConfigureAwait(false);
        }
    }
}
