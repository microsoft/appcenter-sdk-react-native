// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Contoso.Forms.Puppet.UWP;
using Windows.Storage;
using Windows.Storage.Pickers;
using Xamarin.Forms;

[assembly: Dependency(typeof(FilePicker))]
namespace Contoso.Forms.Puppet.UWP
{
    public class FilePicker : IFilePicker
    {
        async Task<string> IFilePicker.PickFile()
        {
            FileOpenPicker openPicker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            // FileTypeFilter is required for FileOpenPicker.PickSingleFileAsync to work.
            // UWP app will crash without specifying FileTypeFilter.
            openPicker.FileTypeFilter.Add("*");
            var file = await openPicker.PickSingleFileAsync();
            return file?.Path;
        }

        Tuple<byte[], string, string> IFilePicker.ReadFile(string file)
        {
            var storageFileTask = StorageFile.GetFileFromPathAsync(file).AsTask();
            var storageFile = storageFileTask.GetAwaiter().GetResult();
            var bufferTask = FileIO.ReadBufferAsync(storageFile).AsTask();
            var buffer = bufferTask.GetAwaiter().GetResult();
            byte[] bytes = buffer.ToArray();
            return new Tuple<byte[], string, string>(bytes, storageFile.Name, storageFile.ContentType);
        }

        string IFilePicker.GetFileDescription(string file)
        {
            throw new NotImplementedException();
        }
    }
}
