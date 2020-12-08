namespace Pose.Domain.Documents
{
    public class Sequence
    {
        private ulong _currentValue;

        public Sequence(ulong currentValue = 0)
        {
            _currentValue = currentValue;
        }

        public ulong GetNext()
        {
            return ++_currentValue;
        }

        public ulong GetCurrentValue() => _currentValue;
    }
}
