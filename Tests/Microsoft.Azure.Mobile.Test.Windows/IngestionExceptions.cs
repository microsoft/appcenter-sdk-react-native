using Microsoft.Azure.Mobile.Ingestion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile
{
    public class RecoverableIngestionException : IngestionException
    {
        public override bool IsRecoverable => true;
    }

    public class NonRecoverableIngestionException : IngestionException
    {
        public override bool IsRecoverable => false;
    }
}
