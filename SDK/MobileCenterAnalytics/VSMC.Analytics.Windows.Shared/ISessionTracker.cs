using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Mobile.Analytics.Channel
{
    public interface ISessionTracker
    {
        void Resume();
        void Pause();
        void ClearSessions();
    }
}
