// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Text;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.AppCenter.Crashes.Test.Windows
{
    [TestClass]
    public class ErrorAttachmentLogTest
    {
        [TestMethod]
        public void TestCreateBinaryAttachmentLog()
        {
            // Create an error attachment log.
            var contentType = "mime/type";
            var data = new byte[] { 0, 3, 2, 1, 0 };
            var fileName = "binary attachment";
            var attachment = ErrorAttachmentLog.AttachmentWithBinary(data, fileName, contentType);

            // Verify the contents.
            Assert.AreEqual(contentType, attachment.ContentType);
            Assert.AreEqual(fileName, attachment.FileName);
            CollectionAssert.AreEqual(data, attachment.Data);
        }

        [TestMethod]
        public void TestCreateTextAttachmentLog()
        {
            // Create an error attachment log with text.
            var text = "This is the text.";
            var fileName = "text attachment";
            var attachment = ErrorAttachmentLog.AttachmentWithText(text, fileName);

            // Verify the contents.
            Assert.AreEqual("text/plain", attachment.ContentType);
            Assert.AreEqual(fileName, attachment.FileName);
            Assert.AreEqual(text, Encoding.UTF8.GetString(attachment.Data));
        }

        [TestMethod]
        public void TestNullFileNameIsAllowed()
        {
            // Create an error attachment log.
            var contentType = "mime/type";
            var data = new byte[] { 0, 3, 2, 1, 0 };
            string fileName = null;
            var attachment = ErrorAttachmentLog.AttachmentWithBinary(data, fileName, contentType);

            // Verify the contents.
            Assert.AreEqual(contentType, attachment.ContentType);
            Assert.AreEqual(fileName, attachment.FileName);
            CollectionAssert.AreEqual(data, attachment.Data);
        }

        [TestMethod]
        public void TestCreateNullBinaryAttachmentLog()
        {
            // Attempt to create an error attachment log with null data.
            var contentType = "mime/type";
            byte[] data = null;
            var fileName = "binary attachment";
            var attachment = ErrorAttachmentLog.AttachmentWithBinary(data, fileName, contentType);

            // Verify the result is null.
            Assert.IsNull(attachment);
        }

        [TestMethod]
        public void TestCreateNullTextAttachmentLog()
        {
            // Attempt to create an error attachment log with null text.
            string text = null;
            var fileName = "text attachment";
            var attachment = ErrorAttachmentLog.AttachmentWithText(text, fileName);

            // Verify the result is null.
            Assert.IsNull(attachment);
        }

        [TestMethod]
        public void TestValidateDoesNotThrowForValidLog()
        {
            var validErrorAttachmentLog = new ErrorAttachmentLog
            {
                ContentType = "ContentType",
                Data = new byte[] { 1, 2, 3, 4 },
                Device = GetValidDevice()
            };

            // Pass if validate does not throw.
            validErrorAttachmentLog.Validate();
        }

        [TestMethod]
        public void TestValidateThrowsIfMissingData()
        {
            var validErrorAttachmentLog = new ErrorAttachmentLog
            {
                ContentType = "ContentType",
                Device = GetValidDevice()
            };
            Assert.ThrowsException<ValidationException>(() => validErrorAttachmentLog.Validate());
        }

        [TestMethod]
        public void TestValidateThrowsIfMissingContentType()
        {
            var validErrorAttachmentLog = new ErrorAttachmentLog
            {
                Data = new byte[] { 1, 2, 3, 4 },
                Device = GetValidDevice()
            };
            Assert.ThrowsException<ValidationException>(() => validErrorAttachmentLog.Validate());
        }

        [TestMethod]
        public void TestValidatePropertiesReturnsTrueIfValidData()
        {
            var validErrorAttachmentLog = new ErrorAttachmentLog
            {
                ContentType = "ContentType",
                ErrorId = Guid.NewGuid(),
                Data = new byte[] { 1, 2, 3, 4 },
                Id = Guid.NewGuid()
            };
            Assert.IsTrue(validErrorAttachmentLog.ValidatePropertiesForAttachment());
        }

        [TestMethod]
        public void TestValidatePropertiesReturnsFalseIfMissingData()
        {
            var validErrorAttachmentLog = new ErrorAttachmentLog
            {
                ContentType = "ContentType",
                ErrorId = Guid.NewGuid(),
                Id = Guid.NewGuid()
            };
            Assert.IsFalse(validErrorAttachmentLog.ValidatePropertiesForAttachment());
        }

        [TestMethod]
        public void TestValidatePropertiesReturnsFalseIfMissingContentType()
        {
            var validErrorAttachmentLog = new ErrorAttachmentLog
            {
                Data = new byte[] { 1, 2, 3, 4 },
                Id = Guid.NewGuid(),
                ErrorId = Guid.NewGuid(),
            };
            Assert.IsFalse(validErrorAttachmentLog.ValidatePropertiesForAttachment());
        }

        [TestMethod]
        public void TestValidatePropertiesReturnsFalseIfInvalidErrorId()
        {
            var validErrorAttachmentLog = new ErrorAttachmentLog
            {
                Data = new byte[] { 1, 2, 3, 4 },
                ContentType = "ContentType",
                Id = Guid.NewGuid()
            };
            Assert.IsFalse(validErrorAttachmentLog.ValidatePropertiesForAttachment());
        }

        [TestMethod]
        public void TestValidatePropertiesReturnsFalseIfInvalidAttachId()
        {
            var validErrorAttachmentLog = new ErrorAttachmentLog
            {
                Data = new byte[] { 1, 2, 3, 4 },
                ErrorId = Guid.NewGuid(),
                ContentType = "ContentType",
            };
            Assert.IsFalse(validErrorAttachmentLog.ValidatePropertiesForAttachment());
        }

        private Microsoft.AppCenter.Ingestion.Models.Device GetValidDevice()
        {
            return new Microsoft.AppCenter.Ingestion.Models.Device("sdkName", "sdkVersion", "osName", "osVersion",
                "locale", 1,"appVersion", "appBuild", null, null, "model", "oemName", "osBuild", null, "screenSize",
                null, null, "appNamespace", null, null, null, null);
        }
    }
}
