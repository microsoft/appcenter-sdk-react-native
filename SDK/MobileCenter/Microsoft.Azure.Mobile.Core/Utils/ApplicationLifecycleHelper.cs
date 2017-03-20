using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Utils
{
    public class ApplicationLifecycleHelper : IApplicationLifecycleHelper
    {
        public bool Enabled { get; set; }
        public event Action ApplicationSuspending;
        public event Action ApplicationResuming;
        public event UnhandledExceptionOccurredEventHandler UnhandledExceptionOccurred;
    }
}
