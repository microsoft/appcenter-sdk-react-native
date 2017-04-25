namespace Microsoft.Azure.Mobile.Push
{
   	public partial class Push
	{
        /// <summary>
        /// Enable or disable Push module.
        /// </summary>
		public static bool Enabled
		{
			get { return PlatformEnabled; }
            set { PlatformEnabled = value; }
		}
	}
}
