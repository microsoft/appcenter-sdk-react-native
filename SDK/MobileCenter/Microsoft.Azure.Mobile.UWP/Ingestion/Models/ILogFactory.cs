using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.Mobile.Ingestion.Models
{
    public interface ILogFactory
    {
        Log Create();
        Type LogType { get; }
    }
}
