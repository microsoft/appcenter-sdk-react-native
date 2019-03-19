// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

namespace Contoso.Forms.Puppet
{
    public interface IFilePicker
    {
        Task<string> PickFile();
        Tuple<byte[], string, string> ReadFile(string file);
        string GetFileDescription(string file);
    }
}
