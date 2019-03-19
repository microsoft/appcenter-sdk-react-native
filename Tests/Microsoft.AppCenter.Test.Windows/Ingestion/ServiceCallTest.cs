// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Ingestion;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.AppCenter.Test.Windows.Ingestion
{
    [TestClass]
    public class ServiceCallTest
    {
        private ServiceCall _serviceCall;

        [TestInitialize]
        public void InitializeServiceCallTest()
        {
            var appSecret = Guid.NewGuid().ToString();
            var installId = Guid.NewGuid();
            var logs = new List<Log>();
            _serviceCall = new ServiceCall(appSecret, installId, logs);
            Assert.AreEqual(appSecret, _serviceCall.AppSecret);
            Assert.AreEqual(installId, _serviceCall.InstallId);
            Assert.AreEqual(logs, _serviceCall.Logs);
            Assert.IsFalse(_serviceCall.IsCompleted);
            Assert.IsFalse(_serviceCall.IsCanceled);
            Assert.IsFalse(_serviceCall.IsFaulted);
        }

        [TestMethod]
        public void ServiceCallSucceeded()
        {
            var continueWith = 0;
            _serviceCall.ContinueWith(serviceCall =>
            {
                Assert.AreEqual(_serviceCall, serviceCall);
                Assert.IsTrue(serviceCall.IsCompleted);
                continueWith++;
            });
            Assert.AreEqual(0, continueWith);
            _serviceCall.SetResult("Test");
            Assert.AreEqual(1, continueWith);
            Assert.IsTrue(_serviceCall.IsCompleted);
            Assert.IsFalse(_serviceCall.IsCanceled);
            Assert.IsFalse(_serviceCall.IsFaulted);
            Assert.AreEqual("Test", _serviceCall.Result);
            Assert.IsNull(_serviceCall.Exception);

            // Check copy state.
            var copy = new ServiceCall();
            copy.CopyState(_serviceCall);
            Assert.IsTrue(copy.IsCompleted);
            Assert.IsFalse(copy.IsCanceled);
            Assert.IsFalse(copy.IsFaulted);
            Assert.AreEqual("Test", copy.Result);
            Assert.IsNull(copy.Exception);
        }

        [TestMethod]
        public void ServiceCallFaulted()
        {
            var continueWith = 0;
            var exeption = new CancellationException();
            _serviceCall.ContinueWith(serviceCall =>
            {
                Assert.AreEqual(_serviceCall, serviceCall);
                Assert.IsTrue(serviceCall.IsCompleted);
                continueWith++;
            });
            Assert.AreEqual(0, continueWith);
            _serviceCall.SetException(exeption);
            Assert.AreEqual(1, continueWith);
            Assert.IsTrue(_serviceCall.IsCompleted);
            Assert.IsFalse(_serviceCall.IsCanceled);
            Assert.IsTrue(_serviceCall.IsFaulted);
            Assert.IsNull(_serviceCall.Result);
            Assert.AreEqual(exeption, _serviceCall.Exception);

            // Check copy state.
            var copy = new ServiceCall();
            copy.CopyState(_serviceCall);
            Assert.IsTrue(copy.IsCompleted);
            Assert.IsFalse(copy.IsCanceled);
            Assert.IsTrue(copy.IsFaulted);
            Assert.IsNull(copy.Result);
            Assert.AreEqual(exeption, copy.Exception);
        }

        [TestMethod]
        public void ServiceCallCanceled()
        {
            var continueWith = 0;
            _serviceCall.ContinueWith(serviceCall =>
            {
                Assert.AreEqual(_serviceCall, serviceCall);
                Assert.IsTrue(serviceCall.IsCanceled);
                continueWith++;
            });
            Assert.AreEqual(0, continueWith);
            _serviceCall.Cancel();
            Assert.AreEqual(1, continueWith);
            Assert.IsFalse(_serviceCall.IsCompleted);
            Assert.IsTrue(_serviceCall.IsCanceled);
            Assert.IsFalse(_serviceCall.IsFaulted);
            Assert.IsNull(_serviceCall.Result);
            Assert.IsNull(_serviceCall.Exception);

            // Check copy state.
            var copy = new ServiceCall();
            copy.CopyState(_serviceCall);
            Assert.IsFalse(copy.IsCompleted);
            Assert.IsTrue(copy.IsCanceled);
            Assert.IsFalse(copy.IsFaulted);
            Assert.IsNull(copy.Result);
            Assert.IsNull(copy.Exception);
        }

        [TestMethod]
        public void ServiceCallContinueWith()
        {
            var continueWith1 = 0;
            var continueWith2 = 0;
            _serviceCall.ContinueWith(serviceCall =>
            {
                Assert.AreEqual(_serviceCall, serviceCall);
                Assert.IsTrue(_serviceCall.IsCompleted);
                continueWith1++;
            });
            Assert.AreEqual(0, continueWith1);
            Assert.AreEqual(0, continueWith2);
            _serviceCall.SetResult("Test");
            Assert.AreEqual(1, continueWith1);
            Assert.AreEqual(0, continueWith2);
            _serviceCall.ContinueWith(serviceCall =>
            {
                Assert.AreEqual(_serviceCall, serviceCall);
                Assert.IsTrue(_serviceCall.IsCompleted);
                continueWith2++;
            });
            Assert.AreEqual(1, continueWith1);
            Assert.AreEqual(1, continueWith2);
        }

        [TestMethod]
        public void ServiceCallDisposed()
        {
            _serviceCall.Dispose();
            Assert.ThrowsException<ObjectDisposedException>(() => _serviceCall.SetResult(""));
            Assert.ThrowsException<ObjectDisposedException>(() => _serviceCall.SetException(new AppCenterException("")));
            Assert.ThrowsException<ObjectDisposedException>(() => _serviceCall.ContinueWith(_ => {}));
            Assert.ThrowsException<ObjectDisposedException>(() => _serviceCall.CopyState(new ServiceCall()));
            Assert.ThrowsException<ObjectDisposedException>(() => _serviceCall.Cancel());
        }
    }
}
