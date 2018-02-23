using System;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion;

namespace Microsoft.AppCenter.Test
{
    public static class TaskExtension
    {
        public static Task GetCompletedTask()
        {
            var completedTask = Task.Delay(0);
            completedTask.Wait();
            return completedTask;
        }

        public static Task GetFaultedTask(Exception e)
        {
            Task task = null;
            try
            {
                task = Task.Factory.StartNew(() => { throw e; });
                task.Wait();
            }
            catch (Exception)
            {

            }
            return task;
        }

        public static Task<T> GetCompletedTask<T>(T retVal)
        {
            var completedTask = Task<T>.Factory.StartNew(() => retVal);
            completedTask.Wait();
            return completedTask;
        }

        public static Task<T> GetFaultedTask<T>(T retVal)
        {
            var task = Task.Factory.StartNew<T>(() => { throw new IngestionException(); });
            try
            {
                task.Wait();
            }
            catch (Exception)
            {

            }
            return task;
        }
    }
}
