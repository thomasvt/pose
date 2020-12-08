using System;

namespace Pose.Domain
{
    public interface IUnitOfWork : IDisposable

    {
    internal void Execute(IEvent @event);
    internal ulong GetNewEntityId();
    }
}