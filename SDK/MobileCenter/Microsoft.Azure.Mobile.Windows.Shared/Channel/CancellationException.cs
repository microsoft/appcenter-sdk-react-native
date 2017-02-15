using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile
{
    public class CancellationException : MobileCenterException
    {
        public CancellationException() : base("Request cancelled because channel is disabled.") { }
    }
}
