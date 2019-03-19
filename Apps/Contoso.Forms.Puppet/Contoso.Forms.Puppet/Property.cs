// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Contoso.Forms.Puppet
{
    public class Property
    {
        public string Name { get; private set; }

        public string Value { get; private set; }

        public Property(string propertyName, string propertyValue)
        {
            Name = propertyName;
            Value = propertyValue;
        }
    }
}
