// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Storage;
using Microsoft.AppCenter.Utils.Files;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace Microsoft.AppCenter.Test.Storage
{
    [TestClass]
    public class StorageAdapterTest
    {
        [TestMethod]
        public void InitializeStorageCreatesStorageDirectory()
        {
            var adapter = new StorageAdapter("path/to/database/file.db");

            // Verify that a directory object was created.
            Assert.IsNotNull(adapter._databaseDirectory);

            // Replace the directory with a mock and initialize.
            adapter._databaseDirectory = Mock.Of<Directory>();
            adapter.InitializeStorageAsync().Wait();
            Mock.Get(adapter._databaseDirectory).Verify(directory => directory.Create());
        }

        [TestMethod]
        public void CreateStorageAdapterDoesNotCreateDirectoryWhenNull()
        {
            var adapter = new StorageAdapter("databaseAtRoot.db");

            // Verify that a directory object was not created.
            Assert.IsNull(adapter._databaseDirectory);

            // Should not crash even if directory is null.
            adapter.InitializeStorageAsync().Wait();
        }

        [TestMethod]
        public void CreateStorageAdapterExceptionIsWrapped()
        {
            var adapter = new StorageAdapter("path/to/database/file.db")
            {
                _databaseDirectory = Mock.Of<Directory>()
            };
            var sourceException = new System.IO.PathTooLongException();

            // Mock the directory to throw when created.
            Mock.Get(adapter._databaseDirectory).Setup(directory => directory.Create()).Throws(sourceException);
            const string databaseDirectory = "databaseDirectory";
            var databasePath = System.IO.Path.Combine(databaseDirectory, "database.db");
            Exception actualException = null;
            try
            {
                adapter.InitializeStorageAsync().Wait();
            }
            catch (AggregateException ex)
            {
                actualException = ex.InnerException;
            }
            Assert.IsInstanceOfType(actualException, typeof(StorageException));
            Assert.IsInstanceOfType(actualException?.InnerException, typeof(System.IO.PathTooLongException));
        }
    }
}
