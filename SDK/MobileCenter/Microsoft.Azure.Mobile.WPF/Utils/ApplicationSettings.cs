using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Utils
{
    public class ApplicationSettings : IApplicationSettings
    {
        public object this[string key]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            throw new NotImplementedException();
        }
    }
}
