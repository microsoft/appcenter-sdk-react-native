namespace Microsoft.AAppCenterAnalytics.Channel
{
    public interface ISessionTracker
    {
        void Resume();
        void Pause();
        void ClearSessions();
    }
}
