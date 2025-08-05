namespace QuotaChecker
{
    public class Scale
    {
        private static Scale? _scale;
        private static readonly object _lock = new object();
        private bool isQuotaReached;
        private int _unit;
        private int _quotaLimit = 10;
        private readonly Timer upTimer;
        private readonly Timer quotaTimer;

        public event Action<int> UnitChanged;          
        public event Action<int> QuotaReached;

        private Scale()
        {
            _unit = 0;
            upTimer = new Timer(_ => Increase(), null, 1000, 2000);
            quotaTimer = new Timer(_ => HasReachedQuota(), null, 1000, 1000);
        }

        public bool IsQuotaReached
        {
            get { lock (_lock) return isQuotaReached; }
        }

        public int Unit
        {
            get { lock (_lock) return _unit; }
        }

        public int QuotaLimit
        {
            get { lock (_lock) return _quotaLimit; }
            set { lock (_lock) _quotaLimit = value; }
        }

        public static Scale GetScale()
        {
            if (_scale == null)
            {
                lock (_lock)
                {
                    if (_scale == null)
                    {
                        _scale = new Scale();
                    }
                }
            }
            return _scale;
        }

        private void Increase()
        {
            lock (_lock)
            {
                if (_unit < _quotaLimit)
                {
                    _unit++;
                    UnitChanged?.Invoke(_unit);
                }
            }
        }

        private void HasReachedQuota()
        {
            lock (_lock)
            {
                if (!isQuotaReached && _unit >= _quotaLimit)
                {
                    isQuotaReached = true;
                    QuotaReached?.Invoke(_unit);

                    upTimer.Dispose();
                    quotaTimer.Dispose();
                }
            }
        }
    }
}
