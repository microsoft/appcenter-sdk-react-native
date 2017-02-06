using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.UWP.Ingestion.Models
{
    public interface ILog
    {
        long TOffset { get; set; }
        long SId { get; set; }
        Device Device { get; set; }
    }
}
