using Microsoft.Azure.Mobile.Ingestion.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Azure.Mobile.Test.Windows.Ingestion.Http
{
    [TestClass]
    public class ServiceCallTest
    {
        private MockIngestion _ingestion;
        private ServiceCall _serviceCall;

        private const int DefaultWaitTime = 5000;

        /* Event semaphores for invokation verification */
        private const int SucceededCallbackSemaphoreIdx = 0;
        private const int FailedCallbackSemaphoreIdx = 1;
        private readonly List<SemaphoreSlim> _eventSemaphores = new List<SemaphoreSlim> { new SemaphoreSlim(0), new SemaphoreSlim(0) };

        [TestInitialize]
        public void InitializeServiceCallTest()
        {
            _ingestion = new MockIngestion();
            _serviceCall = new MockServiceCall(_ingestion, null, string.Empty, Guid.NewGuid());
            SetupEventCallbacks();
        }

        [TestMethod]
        public void CheckSuccessCallback()
        {
            _ingestion.CallShouldSucceed = true;

            _serviceCall.Execute();

            Assert.IsTrue(SuccessCallbackOccurred());
        }

        [TestMethod]
        public void CheckUnsuccessCallback()
        {
            _ingestion.CallShouldSucceed = false;

            _serviceCall.Execute();

            Assert.IsTrue(FailedCallbackOccurred());
        }

        private void SetupEventCallbacks()
        {
            foreach (var sem in _eventSemaphores)
            {
                if (sem.CurrentCount != 0)
                {
                    sem.Release(sem.CurrentCount);
                }
            }

            _serviceCall.ServiceCallSucceededCallback += () => _eventSemaphores[SucceededCallbackSemaphoreIdx].Release();
            _serviceCall.ServiceCallFailedCallback += (e) => _eventSemaphores[FailedCallbackSemaphoreIdx].Release();
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
    }
}
