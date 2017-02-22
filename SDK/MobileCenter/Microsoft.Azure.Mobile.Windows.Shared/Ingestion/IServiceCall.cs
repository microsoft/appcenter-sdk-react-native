using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Mobile.Ingestion
{
    public delegate void ServiceCallFailedHandler(IngestionException exception);
    public interface IServiceCall
    {
        void Execute();
        event ServiceCallFailedHandler Failed;
        event Action Succeeded;
    }
}
