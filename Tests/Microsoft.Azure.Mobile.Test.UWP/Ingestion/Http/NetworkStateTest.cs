using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using HyperMock;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Ingestion.Http;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Microsoft.Azure.Mobile.Test.Ingestion.Http
{
	[TestClass]
	public class NetworkStateTest
	{
		private Mock<IHttpNetworkAdapter> _adapter;
		private TestNetworkStateAdapter _networkState;
		private NetworkStateIngestion _networkStateIngestion;

		[TestInitialize]
		public void InitializeNetworkStateTest()
		{
			_adapter = Mock.Create<IHttpNetworkAdapter>();
			_networkState = new TestNetworkStateAdapter();
			_networkStateIngestion = new NetworkStateIngestion(new IngestionHttp(_adapter.Object), _networkState);
		}

		[TestMethod]
		public void NetworkStateIngestionPrepareServiceCall()
		{
			var appSecret = Guid.NewGuid().ToString();
			var installId = Guid.NewGuid();
			var logs = new List<Log>();
			var call = _networkStateIngestion.PrepareServiceCall(appSecret, installId, logs);
			Assert.AreEqual(call.GetType(), typeof(NetworkStateServiceCall));
			Assert.AreEqual(call.AppSecret, appSecret);
			Assert.AreEqual(call.InstallId, installId);
			Assert.AreEqual(call.Logs, logs);
		}
		
		[TestMethod]
		public void NetworkStateIngestionOnline()
		{
			var call = PrepareServiceCall();
			SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.OK));
			_networkState.IsConnected = true;
			_networkStateIngestion.ExecuteCallAsync(call).RunNotAsync();
			VerifyAdapterSend(Occurred.Once());

			// No throw any exception
		}

		[TestMethod]
		public void NetworkStateIngestionOffline()
		{
			var call = PrepareServiceCall();
			SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.OK));
			_networkState.IsConnected = false;
			Assert.ThrowsException<NetworkUnavailableException>(() => _networkStateIngestion.ExecuteCallAsync(call).RunNotAsync());
			VerifyAdapterSend(Occurred.Never());
		}

		private IServiceCall PrepareServiceCall()
		{
			var appSecret = Guid.NewGuid().ToString();
			var installId = Guid.NewGuid();
			var logs = new List<Log>();
			return _networkStateIngestion.PrepareServiceCall(appSecret, installId, logs);
		}

		private void SetupAdapterSendResponse(HttpResponseMessage response)
		{
			_adapter
				.Setup(a => a.SendAsync(Param.IsAny<HttpRequestMessage>(), Param.IsAny<CancellationToken>()))
				.Returns(Task.Run(() => response));
		}

		private void VerifyAdapterSend(Occurred occurred)
		{
			_adapter
				.Verify(a => a.SendAsync(Param.IsAny<HttpRequestMessage>(), Param.IsAny<CancellationToken>()), occurred);
		}

		private class TestNetworkStateAdapter : INetworkStateAdapter
		{
			public bool IsConnected { get; set; }

			public event NetworkAddressChangedEventHandler NetworkAddressChanged;

			public void TestNetworkAddressChanged()
			{
				NetworkAddressChanged?.Invoke(null, null);
			}
		}
	}
}
