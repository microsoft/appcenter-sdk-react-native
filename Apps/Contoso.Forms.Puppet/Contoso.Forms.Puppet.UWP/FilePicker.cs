// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Contoso.Forms.Puppet.UWP;
using Windows.Storage.Pickers;
using Xamarin.Forms;

[assembly: Dependency(typeof(FilePicker))]
namespace Contoso.Forms.Puppet.UWP
{
    public class FilePicker : IFilePicker
    {

        async Task<string> IFilePicker.PickFile()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add("*");
            var file = await openPicker.PickSingleFileAsync();
            return file?.Path;
        }

        Tuple<byte[], string, string> IFilePicker.ReadFile(string file)
        {
            throw new NotImplementedException();
        }

        string IFilePicker.GetFileDescription(string file)
        {
            throw new NotImplementedException();
        }
    }
}