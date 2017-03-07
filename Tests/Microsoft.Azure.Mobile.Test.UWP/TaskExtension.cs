using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Test
{
    public static class TaskExtension
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

        public static Task GetCompletedTask()
        {
            var completedTask = Task.Delay(0);
            completedTask.Wait();
            return completedTask;
        }
        public static Task<string> GetCompletedTaskString()
        {
            var completedTask = Task<string>.Factory.StartNew(() => "hello");
            completedTask.Wait();
            return completedTask;
        }
    }
}
