namespace Microsoft.AppCenter.Analytics.Channel
{
    public interface ISessionTracker
    {
        void Resume();
        void Pause();
    }
}
