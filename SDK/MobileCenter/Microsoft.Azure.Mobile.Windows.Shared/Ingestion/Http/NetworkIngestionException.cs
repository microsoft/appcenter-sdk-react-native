using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class NetworkIngestionException : IngestionException
    {
        public override bool IsRecoverable => true;
    }
}
