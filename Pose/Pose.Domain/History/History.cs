using System;
using System.Collections.Generic;
using System.Linq;
using Pose.Domain.Documents;
using Pose.Domain.History.Messages;
using Pose.Framework.Messaging;

namespace Pose.Domain.History
{
    internal class History : IHistory
    {
        private readonly Sequence _versionSequence;
        private readonly IMessageBus _messageBus;
        private readonly IEditableDocument _document;
        private readonly Stack<UnitOfWork> _undoStack;
        private readonly Stack<UnitOfWork> _redoStack;
        private UnitOfWork _currentUow;

        public History(IMessageBus messageBus, IEditableDocument document)
        {
            _versionSequence = new Sequence();
            _messageBus = messageBus;
            _document = document;
            _undoStack = new Stack<UnitOfWork>();
            _redoStack = new Stack<UnitOfWork>();
        }

        public IUnitOfWork StartUnitOfWork(string name)
        {
            if (_currentUow != null)
                throw new Exception("A UnitOfWork is still open. Nesting of UnitOfWork is not supported. Or did you forget to add the using pattern to the previous one?");
            _currentUow = new UnitOfWork(_document, name);
            _currentUow.Committed += UnitOfWorkCommitted;
            return _currentUow;
        }

        private void UnitOfWorkCommitted(object sender, EventArgs e)
        {
            var uow = (UnitOfWork) sender;
            if (uow != _currentUow)
                throw new Exception("Received a commit from another UnitOfWork than expected. Nesting of UnitOfWork is not supported.");
            uow.Version = _versionSequence.GetNext();
            uow.Committed -= UnitOfWorkCommitted;
            _currentUow = null;

            if (uow.IsEmpty)
                return; // only record a UOW if it actually does something.

            AddHistoryItem(uow);
            _document.MarkDirty();
        }

        private void AddHistoryItem(UnitOfWork uow)
        {
            _undoStack.Push(uow);
            if (_redoStack.Any())
            {
                _messageBus.Publish(new HistoryRemovedAfter(_redoStack.Peek().Version));
                _redoStack.Clear();
            }

            _messageBus.Publish(new HistoryItemCommitted(uow.Version, uow.Label));
            RaiseOnCursorChanged();
        }

        public void Undo()
        {
            if (!_undoStack.Any())
                return;

            var uow = _undoStack.Pop();
            uow.PlayBackward();
            _redoStack.Push(uow);

            RaiseOnCursorChanged();
            _document.MarkDirty();
        }

        private void RaiseOnCursorChanged()
        {
            _messageBus.Publish(new HistoryCursorChanged(CurrentVersion));
        }

        private ulong CurrentVersion => _undoStack.Any() ? _undoStack.Peek().Version : 0;

        public void Redo()
        {
            if (!_redoStack.Any())
                return;

            var uow = _redoStack.Pop();
            uow.PlayForward();
            _undoStack.Push(uow);

            RaiseOnCursorChanged();
            _document.MarkDirty();
        }

        public bool CanRedo()
        {
            return _redoStack.Any();
        }

        public bool CanUndo()
        {
            return _undoStack.Any();
        }

        public void JumpToVersion(in ulong version)
        {
            if (version == CurrentVersion)
                return;
            if (version > CurrentVersion)
            {
                while (_redoStack.Any() && _redoStack.Peek().Version <= version)
                {
                    Redo();
                }
            }
            else
            {
                while (_undoStack.Any() && _undoStack.Peek().Version > version) // don't play the actual version, because the version's state is the state AFTER the UOW.
                {
                    Undo();
                }
            }
        }
    }
}
