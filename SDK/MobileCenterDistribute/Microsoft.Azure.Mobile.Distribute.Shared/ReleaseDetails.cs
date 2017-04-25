using System;

namespace Microsoft.Azure.Mobile.Distribute
{
    /// <summary>
    /// Release details.
    /// </summary>
    public class ReleaseDetails
    {
        internal ReleaseDetails()
        {
        }

        /// <summary>
        /// Release identifier.
        /// </summary>
        /// <value>Release identifier.</value>
        public int Id { get; internal set; }

        /// <summary>
        /// Build number or version code.
        /// </summary>
        /// <value>The version.</value>
        public string Version { get; internal set; }

        /// <summary>
        /// Human readable version.
        /// </summary>
        /// <value>The version.</value>
        public string ShortVersion { get; internal set; }

        /// <summary>
        /// Release notes.
        /// </summary>
        /// <value>The release notes.</value>
        public string ReleaseNotes { get; internal set; }

        /// <summary>
        /// Release notes URL.
        /// </summary>
        /// <value>The release notes URL.</value>
        public Uri ReleaseNotesUrl { get; internal set; }

        /// <summary>
        /// Returns <c>true</c> if this release update is mandatory, otherwise <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if mandatory update; otherwise, <c>false</c>.</value>
        public bool MandatoryUpdate { get; internal set; }
    }
}
