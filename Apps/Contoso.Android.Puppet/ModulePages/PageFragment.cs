// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Android.Support.V4.App;

namespace Contoso.Android.Puppet
{
    public abstract class PageFragment : Fragment
    {
        public override void SetInitialSavedState(SavedState state)
        {
            // Prevent set saved state
        }

        public override bool UserVisibleHint
        {
            get { return base.UserVisibleHint; }
            set
            {
                base.UserVisibleHint = value;
                if (value && IsResumed)
                    UpdateState();
            }
        }

        protected abstract void UpdateState();
    }
}
