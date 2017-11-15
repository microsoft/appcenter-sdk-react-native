using Microsoft.AppCenter.Ingestion.Models;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Channel
{
    public interface IChannelUnit : IChannel
    {

        /// <summary>
        /// Enqueue a log for processing
        /// </summary>
        /// <param name="log"></param>
        Task EnqueueAsync(Log log);
    }
}
