// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.AppCenter.Crashes.Test
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
            var fileName = "binary attachment";
            var attachment = ErrorAttachmentLog.AttachmentWithText(text, fileName);

            // Verify the contents.
            Assert.AreEqual("text/plain", attachment.ContentType);
            Assert.AreEqual(fileName, attachment.FileName);
            Assert.AreEqual(text, Encoding.UTF8.GetString(attachment.Data));
        }
    }
}
