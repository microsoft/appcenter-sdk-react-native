// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Windows.Shared.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.AppCenter.Test.Windows
{
    [TestClass]
    public class PropertyValidatorTest
    {
        [TestMethod]
        public void TestPropertyLimits()
        {
            Assert.Equals(20, PropertyValidator.MaxProperties);
            Assert.Equals(125, PropertyValidator.MaxPropertyKeyLength);
            Assert.Equals(125, PropertyValidator.MaxPropertyValueLength);
        }
    }
}
