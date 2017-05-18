using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Storage
{
    public partial class TaskLock
    {
        public class TaskLockSource : IDisposable
        {
            private readonly object _lockObject = new object();
            private readonly SemaphoreSlim _taskSemaphore = new SemaphoreSlim(1);
            private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);
            private bool _isShutdown;
            private int _numTasksRemaning;

            public bool Shutdown(TimeSpan timeout)
            {
                lock (_lockObject)
                {
                    _isShutdown = true;
                }
                return _taskSemaphore.Wait(timeout);
            }

            public TaskLock GetTaskLock()
            {
                lock (_lockObject)
                {
                    if (_isShutdown)
                    {
                        throw new StorageException("Trying to execute task after shutdown requested");
                    }
                    _numTasksRemaning++;
                    if (_taskSemaphore.CurrentCount == 1)
                    {
                        _taskSemaphore.Wait();
                    }
                }
                _mutex.Wait();
                return new TaskLock(this);
            }


            public async Task<TaskLock> GetTaskLockAsync()
            {
                lock (_lockObject)
                {
                    if (_isShutdown)
                    {
                        throw new StorageException("Trying to execute task after shutdown requested");
                    }
                    _numTasksRemaning++;
                    if (_taskSemaphore.CurrentCount == 1)
                    {
                        _taskSemaphore.Wait();
                    }
                }
                await _mutex.WaitAsync().ConfigureAwait(false);
                return new TaskLock(this);
            }

            public void TaskHandlerWasDisposed()
            {
                _mutex.Release();
                lock (_lockObject)
                {
                    _numTasksRemaning--;
                    if (_numTasksRemaning == 0 && _isShutdown && _taskSemaphore.CurrentCount == 0)
                    {
                        _taskSemaphore.Release();
                    }

                }
            }

            public void Dispose()
            {
                _taskSemaphore.Dispose();
                _mutex.Dispose();
            }
        }
    }
}