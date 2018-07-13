using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.AppCenter.Test.UWP.Utils
{
    public class NetworkStateTests
    {
        private NetworkStateAdapter _networkState;

        [TestMethod]
        public void NetWorkInterfaceThrowsExceptionCanBeHandled()
        {
            var networkInterfaceAbstractionMock = new Mock<NetworkInterfaceAbstraction>();
            networkInterfaceAbstractionMock.Setup(m => m.IsNetworkAvailable()).Callback(() =>
            {
                throw new Exception();
            });
            _networkState.NetworkAbstraction = networkInterfaceAbstractionMock.Object;

            Assert.IsFalse(_networkState.IsConnected);
        }
    }
}
