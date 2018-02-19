using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Utils
{
    internal static class TaskExtension
    {
        public static T RunNotAsync<T>(this Task<T> @this)
        {
            try
            {
                @this.Wait();
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }

            return @this.Result;
        }

        public static void RunNotAsync(this Task @this)
        {
            try
            {
                @this.Wait();
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }
        }
    }
}