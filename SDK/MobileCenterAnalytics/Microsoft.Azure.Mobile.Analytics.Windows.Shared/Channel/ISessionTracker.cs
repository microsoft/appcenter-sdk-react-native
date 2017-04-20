namespace Microsoft.Azure.Mobile.Analytics.Channel
{
    public interface ISessionTracker
    {
        void Resume();
        void Pause();
        void ClearSessions();
    }
}
