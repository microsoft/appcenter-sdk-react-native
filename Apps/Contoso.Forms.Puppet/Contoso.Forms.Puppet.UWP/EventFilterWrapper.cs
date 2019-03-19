// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

namespace Contoso.Forms.Puppet.UWP
{
    internal class EventFilterWrapper : EventFilterHolder.IImplementation
    {
        public Type BindingType => typeof(EventFilter);

        public Task<bool> IsEnabledAsync() => EventFilter.IsEnabledAsync();

        public Task SetEnabledAsync(bool enabled) => EventFilter.SetEnabledAsync(enabled);
    }
}
