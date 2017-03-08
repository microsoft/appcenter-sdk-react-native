using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Ingestion.Http;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Moq;
using Xunit;

namespace Microsoft.Azure.Mobile.Test.Ingestion.Http
{
	public class RetryableTest
    {
        private TestInterval[] _intervals;
        private Mock<IHttpNetworkAdapter> _adapter;
        private IIngestion _retryableIngestion;

        public RetryableTest()
        {
            _adapter = new Mock<IHttpNetworkAdapter>();
            _intervals = new []
            {
                new TestInterval(),
                new TestInterval(),
                new TestInterval()
            };
            var retryIntervals = new Func<Task>[_intervals.Length];
            for (int i = 0; i < _intervals.Length; i++)
            {
                retryIntervals[i] = _intervals[i].Wait;
            }
            _retryableIngestion = new RetryableIngestion(new IngestionHttp(_adapter.Object), retryIntervals);
        }

        /// <summary>
        /// Verify that not retrying not recoverable exceptions.
        /// </summary>
        [Fact]
        public void RetryableExceptionOnFail()
        {
            var call = PrepareServiceCall();
            SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.BadRequest));
            Assert.Throws<HttpIngestionException>(() => _retryableIngestion.ExecuteCallAsync(call).RunNotAsync());
            VerifyAdapterSend(Times.Once());
        }

        /// <summary>
        /// Helper for prepare ServiceCall.
        /// </summary>
        private IServiceCall PrepareServiceCall()
        {
            var appSecret = Guid.NewGuid().ToString();
            var installId = Guid.NewGuid();
            var logs = new List<Log>();
            return _retryableIngestion.PrepareServiceCall(appSecret, installId, logs);
        }

        /// <summary>
        /// Helper for setup responce.
        /// </summary>
        private void SetupAdapterSendResponse(HttpResponseMessage response)
        {
            _adapter
                .Setup(a => a.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => response));
        }

        /// <summary>
        /// Helper for verify SendAsync call.
        /// </summary>
        private void VerifyAdapterSend(Times times)
        {
            _adapter
                .Verify(a => a.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()), times);
        }
        
        public class TestInterval
        {
            private volatile TaskCompletionSource<bool> _task = new TaskCompletionSource<bool>();

            public event Action OnRequest;

            public Task Wait() { OnRequest?.Invoke(); return _task.Task; }
            public void Set() => _task.TrySetResult(true);
        }
    }
}
