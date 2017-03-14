using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;

namespace Microsoft.Azure.Mobile.Utils
{
    public class ApplicationLifecycleHelper : IApplicationLifecycleHelper
    {
        public ApplicationLifecycleHelper()
        {
            Enabled = true;
        }

        private void InvokeResuming(object sender, object e)
        {
            ApplicationResuming?.Invoke();
        }

        private void InvokeSuspending(object sender, object e)
        {
            ApplicationSuspending?.Invoke();
        }

        private bool _enabled;
        public bool Enabled {
            get
            {
                return _enabled;
            }
            set
            {
                if (value == _enabled)
                {
                    return;
                }
                if (value)
                {
                    CoreApplication.Resuming += InvokeResuming;
                    CoreApplication.Suspending += InvokeSuspending;
                }
                else
                {
                    CoreApplication.Resuming -= InvokeResuming;
                    CoreApplication.Suspending -= InvokeSuspending;
                }
                _enabled = value;
            }
        }

        public event Action ApplicationSuspending;
        public event Action ApplicationResuming;
    }
}
