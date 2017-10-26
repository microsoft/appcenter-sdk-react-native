namespace Microsoft.AppCenterAnalytics.Channel
{
    public interface ISessionTracker
    {
        void Resume();
        void Pause();
        void ClearSessions();
    }
}
