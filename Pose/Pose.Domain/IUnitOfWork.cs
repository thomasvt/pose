using System;

namespace Pose.Domain
{
    internal interface IUnitOfWork : IDisposable

    {
    internal void Execute(IEvent @event);
    internal ulong GetNewEntityId();
    }
}