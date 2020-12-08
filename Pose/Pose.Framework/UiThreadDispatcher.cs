using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Pose.Framework
{
    public class UiThreadDispatcher : IUiThreadDispatcher
    {
        private readonly Dispatcher _dispatcher;

        public UiThreadDispatcher() : this(Dispatcher.CurrentDispatcher)
        {
        }

        public UiThreadDispatcher(Dispatcher dispatcher)
        {
            Debug.Assert(dispatcher != null);

            _dispatcher = dispatcher;
        }

        public void Invoke(Action action)
        {
            Debug.Assert(action != null);

            _dispatcher.Invoke(action);
        }

        public void BeginInvoke(Action action)
        {
            Debug.Assert(action != null);

            _dispatcher.BeginInvoke(action);
        }

        public async Task InvokeAsync(Action action)
        {
            await _dispatcher.InvokeAsync(action);
        }

        public bool IsSynchronized => this._dispatcher.Thread == Thread.CurrentThread;
    }
}
