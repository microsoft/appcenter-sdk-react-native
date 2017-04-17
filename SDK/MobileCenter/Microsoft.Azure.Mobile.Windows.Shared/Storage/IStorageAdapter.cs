using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Storage
{
    internal interface IStorageAdapter
    {
        Task<List<T>> GetAsync<T>(Predicate<T> pred, int limit);
    }
}
