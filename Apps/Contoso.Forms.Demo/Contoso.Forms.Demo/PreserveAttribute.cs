// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Android.Runtime
{
    public sealed class PreserveAttribute : System.Attribute
    {
        public bool AllMembers;
        public bool Conditional;
    }
}
