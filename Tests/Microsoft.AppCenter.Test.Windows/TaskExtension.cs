using System;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion;

namespace Microsoft.AppCenter.Test
{
    public static class TaskExtension
    {
        public static T RunNotAsync<T>(this Task<T> @this)
        {
            return @this.GetAwaiter().GetResult();
        }

        public static void RunNotAsync(this Task @this)
        {
            @this.GetAwaiter().GetResult();
        }

        public static Task GetCompletedTask()
        {
            return Task.FromResult(false);
        }

        public static Task GetFaultedTask(Exception e)
        {
            var task = Task.Factory.StartNew(() => throw e);
            try
            {
                task.Wait();
            }
            catch
            {
                // ignored
            }

            return task;
        }

        public static Task<T> GetCompletedTask<T>(T retVal)
        {
            return Task.FromResult(retVal);
        }

        public static Task<T> GetFaultedTask<T>(Exception exception)
        {
            var task = Task.Factory.StartNew<T>(() => throw exception);
            try
            {
                task.Wait();
            }
            catch
            {
                // ignored
            }
            return task;
        }
        
        public static Task<string> ToTask(this IServiceCall @this)
        {
            var source = new TaskCompletionSource<string>();
            @this.ContinueWith(serviceCall =>
            {
                if (serviceCall.IsCanceled)
                {
                    source.SetCanceled();
                }
                else if (serviceCall.IsFaulted)
                {
                    source.SetException(serviceCall.Exception);
                }
                else
                {
                    source.SetResult(serviceCall.Result);
                }
            });
            return source.Task;
        }
    }
}
