using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Microsoft.Azure.Mobile.Storage
{
    public partial class TaskLock : IDisposable
    {
        private readonly TaskLockSource _source;
    
        private TaskLock(TaskLockSource source)
        {
            _source = source;
        }
    
        public void Dispose()
        {
            _source.TaskHandlerWasDisposed();
        }
    }
}
