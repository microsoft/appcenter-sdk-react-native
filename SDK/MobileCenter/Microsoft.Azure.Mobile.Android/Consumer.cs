using System;
using System.Threading.Tasks;
using Com.Microsoft.Azure.Mobile.Utils.Async;

namespace Microsoft.Azure.Mobile
{
    public class Consumer<T> : Java.Lang.Object, IMobileCenterConsumer
    {
        T _result;

        public Task<T> Task { get; }

        public Consumer()
        {
            Task = new Task<T>(() => _result);
        }

        public void Accept(Java.Lang.Object result)
        {
            if (result != null)
            {
                _result = (T)Convert.ChangeType(result, typeof(T));
            }
            Task.Start();
        }
    }
}
