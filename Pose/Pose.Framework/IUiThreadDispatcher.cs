using System;
using System.Threading.Tasks;

namespace Pose.Framework
{
    public interface IUiThreadDispatcher
    {
        void Invoke(Action action);
        void BeginInvoke(Action action);
        bool IsSynchronized { get; }
        Task InvokeAsync(Action action);
    }
}