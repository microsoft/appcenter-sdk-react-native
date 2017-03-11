using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace Microsoft.Azure.Mobile.Test.Windows
{
    /*
        MobileCenterService Protected members for reference:
            protected IChannelGroup ChannelGroup { get; private set; }
            protected IChannel Channel { get; private set; }
            protected abstract string ChannelName { get; }
            protected abstract string ServiceName { get; }
            protected virtual string EnabledPreferenceKey => KeyEnabled + PreferenceKeySeparator + ChannelName;
            protected virtual int TriggerCount => DefaultTriggerCount;
            protected virtual TimeSpan TriggerInterval => DefaultTriggerInterval;
            protected virtual int TriggerMaxParallelRequests => DefaultTriggerMaxParallelRequests;
            protected bool IsInactive;
    */

    [TestClass]
    public class MobileCenterServiceTest
    {
    }
}
