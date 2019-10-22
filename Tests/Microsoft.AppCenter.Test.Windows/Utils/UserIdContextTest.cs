// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.AppCenter.Windows.Shared.Utils;

namespace Microsoft.AppCenter.Test.WindowsDesktop.Utils
{
    [TestClass]
    public class UserIdContextTest
    {
        [TestInitialize]
        public void InitializeUserIdContextTest()
        {
            UserIdContext.Instance = new UserIdContext();
        }

        [TestMethod]
        public void InstanceIsNotNull()
        {
            UserIdContext.Instance = null;
            Assert.IsNotNull(UserIdContext.Instance);
        }

        /// <summary>
        /// Verify User Id max length
        /// </summary>
        [TestMethod]
        public void UserIdLengthShouldNotBeGreaterThan256()
        {
            string userIdGreaterThan256 = new string('a', 257);
            var isUserIdValid = UserIdContext.CheckUserIdValidForAppCenter(userIdGreaterThan256);
            Assert.IsFalse(isUserIdValid, "User Id can't be greater than 256");
        }

        /// <summary>
        /// Test signleton instance
        /// </summary>
        [TestMethod]
        public void InstanceIsSingleton()
        {
            var localInstance = UserIdContext.Instance;

            Assert.AreSame(localInstance, UserIdContext.Instance);
        }
    }
}
