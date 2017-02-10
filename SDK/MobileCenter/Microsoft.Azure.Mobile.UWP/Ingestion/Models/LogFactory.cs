using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.Azure.Mobile.Ingestion.Models
{
    public class LogFactory<T> : ILogFactory where T : Log, new()
    {
        public Log Create()
        {
            return new T();
        }

        public Type LogType
        {
            get
            {
                return typeof(T);
            }

        }
    }
}
