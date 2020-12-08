using System;
using System.Collections.Generic;
using System.Linq;
using Pose.Domain.Documents;

namespace Pose.Domain
{
    internal class UnitOfWork : IUnitOfWork
    {
        public string Label { get; }
        private readonly IEditableDocument _document;
        private bool _isCommitted;
        private readonly List<IEvent> _events;

        internal UnitOfWork(IEditableDocument document, string label)
        {
            Label = label;
            _document = document;
            _events = new List<IEvent>();
        }

        /// <summary>
        /// Immediately plays an event and records it in this <see cref="UnitOfWork"/>.
        /// </summary>
        /// <param name="event"></param>
        void IUnitOfWork.Execute(IEvent @event)
        {
            if (_isCommitted)
                throw new Exception("UnitOfWork is frozen and committed.");

            _events.Add(@event);
            @event.PlayForward(_document);
        }

        public void PlayForward()
        {
            foreach (var @event in _events)
            {
                @event.PlayForward(_document);
            }
        }

        public void PlayBackward()
        {
            foreach (var @event in ((IEnumerable<IEvent>)_events).Reverse())
            {
                @event.PlayBackward(_document);
            }
        }

        ulong IUnitOfWork.GetNewEntityId()
        {
            return _document.GetNextEntityId();
        }

        public void Dispose()
        {
            if (_isCommitted)
                throw new Exception("UnitOfWork was already disposed earlier.");

            Committed?.Invoke(this, EventArgs.Empty);
            _isCommitted = true;
        }

        public event EventHandler Committed;

        public bool IsEmpty => !_events.Any();

        public ulong Version { get; internal set; }
    }
}
