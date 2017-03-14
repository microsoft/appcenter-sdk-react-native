using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Utils;

namespace Microsoft.Azure.Mobile.Utils
{
    public class ApplicationLifecycleHelper : IApplicationLifecycleHelper
    {
        public bool Enabled { get; set; }

        public void InvokeSuspending()
        {
            if (Enabled)
            {
                ApplicationSuspending?.Invoke();
            }
        }

        public void InvokeResuming()
        {
            if (Enabled)
            {
                ApplicationResuming?.Invoke();
            }
        }

        public event Action ApplicationSuspending;
        public event Action ApplicationResuming;
    }
}
