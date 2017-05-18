using Microsoft.Azure.Mobile.Ingestion.Models;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Channel
{
    public interface IChannelUnit : IChannel
    {

        /// <summary>
        /// Enqueue a log for processing
        /// </summary>
        /// <param name="log"></param>
        Task Enqueue(Log log);
    }
}
