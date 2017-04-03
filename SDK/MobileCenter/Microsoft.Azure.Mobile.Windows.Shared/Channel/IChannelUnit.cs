using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Channel
{
    public interface IChannelUnit : IChannel
    {

        /// <summary>
        /// Enqueue a log for processing
        /// </summary>
        /// <param name="log"></param>
        void Enqueue(Log log);
    }
}
