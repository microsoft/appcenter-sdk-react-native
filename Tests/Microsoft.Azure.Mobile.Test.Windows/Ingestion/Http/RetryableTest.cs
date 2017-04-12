using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Ingestion.Http;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.Azure.Mobile.Test.Ingestion.Http
{
    [TestClass]
    public class RetryableTest
    {
        private TestInterval[] _intervals;
        private Mock<IHttpNetworkAdapter> _adapter;
        private IIngestion _retryableIngestion;

        private const int DefaultWaitTime = 5000;

        /* Event semaphores for invokation verification */
        private const int SucceededCallbackSemaphoreIdx = 0;
        private const int FailedCallbackSemaphoreIdx = 1;
        private readonly List<SemaphoreSlim> _eventSemaphores = new List<SemaphoreSlim> { new SemaphoreSlim(0), new SemaphoreSlim(0) };

        [TestInitialize]
        public void InitializeRetryableTest()
        {
            _adapter = new Mock<IHttpNetworkAdapter>();
            _intervals = new[]
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
        /// Verify that ingestion create ServiceCall correctly.
        /// </summary>
        [TestMethod]
        public void RetryableIngestionPrepareServiceCall()
        {
            var appSecret = Guid.NewGuid().ToString();
            var installId = Guid.NewGuid();
            var logs = new List<Log>();
            var call = _retryableIngestion.PrepareServiceCall(appSecret, installId, logs);
            Assert.IsInstanceOfType(call, typeof(RetryableServiceCall));
            Assert.AreEqual(call.AppSecret, appSecret);
            Assert.AreEqual(call.InstallId, installId);
            Assert.AreEqual(call.Logs, logs);
        }

        /// <summary>
        /// Verify behaviour without exceptions.
        /// </summary>
        [TestMethod]
        public void RetryableIngestionSuccess()
        {
            var call = PrepareServiceCall();
            SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.OK));
            _retryableIngestion.ExecuteCallAsync(call).RunNotAsync();
            VerifyAdapterSend(Times.Once());

            // No throw any exception
        }

        /// <summary>
        /// Verify that retrying on recoverable exceptions.
        /// </summary>
        [TestMethod]
        public void RetryableIngestionRepeat1()
        {
            var call = PrepareServiceCall();
            // RequestTimeout - retryable
            SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.RequestTimeout));
            // Run code after this interval immideatly
            _intervals[0].Set();
            // On first delay: replace response (next will be succeed)
            _intervals[0].OnRequest += () => SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.OK));
            // Checks send times on N delay moment
            _intervals[0].OnRequest += () => VerifyAdapterSend(Times.Once());
            _intervals[1].OnRequest += () => Assert.Fail();

            // Run all chain not async
            _retryableIngestion.ExecuteCallAsync(call).RunNotAsync();

            // Must be sent 2 times: 1 - main, 1 - repeat
            VerifyAdapterSend(Times.Exactly(2));
        }

        /// <summary>
        /// Verify that retrying on recoverable exceptions.
        /// </summary>
        [TestMethod]
        public void RetryableIngestionRepeat3()
        {
            var call = PrepareServiceCall();
            // RequestTimeout - retryable
            SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.RequestTimeout));
            // Run code after this intervals immideatly
            _intervals[0].Set();
            _intervals[1].Set();
            _intervals[2].Set();
            // On third delay: replace response (next will be succeed)
            _intervals[2].OnRequest += () => SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.OK));
            // Checks send times on N delay moment
            _intervals[0].OnRequest += () => VerifyAdapterSend(Times.Once());
            _intervals[1].OnRequest += () => VerifyAdapterSend(Times.Exactly(2));
            _intervals[2].OnRequest += () => VerifyAdapterSend(Times.Exactly(3));

            // Run all chain not async
            _retryableIngestion.ExecuteCallAsync(call).RunNotAsync();

            // Must be sent 4 times: 1 - main, 3 - repeat
            VerifyAdapterSend(Times.Exactly(4));
        }

        /// <summary>
        /// Verify service call canceling.
        /// </summary>
        [TestMethod]
        public void RetryableIngestionCancel()
        {
            var call = PrepareServiceCall();
            // RequestTimeout - retryable
            SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.RequestTimeout));
            // Run code after this intervals immideatly
            _intervals[0].Set();
            _intervals[1].Set();
            // On second delay: cancel call
            _intervals[1].OnRequest += () => call.Cancel();
            // Checks send times on N delay moment
            _intervals[0].OnRequest += () => VerifyAdapterSend(Times.Once());
            _intervals[2].OnRequest += () => Assert.Fail();

            // Run all chain not async
            Assert.ThrowsException<TaskCanceledException>(() => _retryableIngestion.ExecuteCallAsync(call).RunNotAsync());

            // Must be sent 2 times: 1 - main, 1 - repeat
            VerifyAdapterSend(Times.Exactly(2));
        }

        /// <summary>
        /// Verify that not retrying not recoverable exceptions.
        /// </summary>
        [TestMethod]
        public void RetryableIngestionException()
        {
            var call = PrepareServiceCall();
            SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.BadRequest));
            Assert.ThrowsException<HttpIngestionException>(() => _retryableIngestion.ExecuteCallAsync(call).RunNotAsync());
            VerifyAdapterSend(Times.Once());
        }

        [TestMethod]
        public void ServiceCallSuccessCallbackTest()
        {
            SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.OK));
            var call = PrepareServiceCall();

            SetupEventCallbacks(call);

            call.Execute();

            Assert.IsTrue(SuccessCallbackOccurred());
        }

        [TestMethod]
        public void ServiceCallFailedCallbackTest()
        {
            SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.NotFound));
            var call = PrepareServiceCall();

            SetupEventCallbacks(call);

            call.Execute();

            Assert.IsTrue(FailedCallbackOccurred());
        }

        /// <summary>
        /// Validate that constructor throws correct exception type with nullable timespan array
        /// </summary>
        [TestMethod]
        public void RetryableIngestionWithNullIntervals()
        {
            TimeSpan[] timeSpans = null;
            Assert.ThrowsException<ArgumentNullException>(() => { new RetryableIngestion(new IngestionHttp(_adapter.Object), timeSpans); });
        }

        /// <summary>
        /// Validate that constructor throws correct exception type with nullable timespan array
        /// </summary>
        [TestMethod]
        public void RetryableIngestionWithNullFunc()
        {
            Func<Task>[] funcs = null;
            Assert.ThrowsException<ArgumentNullException>(() => { new RetryableIngestion(new IngestionHttp(_adapter.Object), funcs); });
        }

        /// <summary>
        /// Validate that function doesn't throw any exception
        /// </summary>
        [TestMethod]
        public void CallExecuteAsyncWhenCallIsNotOfTypeRetryableServiceCall()
        {
            var mockServiceCall = new Mock<IServiceCall>();
            _retryableIngestion.ExecuteCallAsync(new TestServiceCall(mockServiceCall.Object));

            // No throw any exception
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

        private void SetupEventCallbacks(IServiceCall call)
        {
            foreach (var sem in _eventSemaphores)
            {
                if (sem.CurrentCount != 0)
                {
                    sem.Release(sem.CurrentCount);
                }
            }

            call.ServiceCallSucceededCallback += () => _eventSemaphores[SucceededCallbackSemaphoreIdx].Release();
            call.ServiceCallFailedCallback += (e) => _eventSemaphores[FailedCallbackSemaphoreIdx].Release();
        }

        private bool SuccessCallbackOccurred(int waitTime = DefaultWaitTime)
        {
            return EventWithSemaphoreOccurred(_eventSemaphores[SucceededCallbackSemaphoreIdx], 1, waitTime);
        }

        private bool FailedCallbackOccurred(int waitTime = DefaultWaitTime)
        {
            return EventWithSemaphoreOccurred(_eventSemaphores[FailedCallbackSemaphoreIdx], 1, waitTime);
        }

        private static bool EventWithSemaphoreOccurred(SemaphoreSlim semaphore, int numTimes, int waitTime)
        {
            var enteredAll = true;
            for (var i = 0; i < numTimes; ++i)
            {
                enteredAll &= semaphore.Wait(waitTime);
            }
            return enteredAll;
        }

        public class TestServiceCall : ServiceCallDecorator
        {
            public TestServiceCall(IServiceCall decoratedApi) : base(decoratedApi)
            {
            }
        }
    }
}
