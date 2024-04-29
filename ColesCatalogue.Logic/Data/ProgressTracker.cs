using System;

namespace ColesCatalogue.Logic.Data
{
    public class ProgressTracker
    {
        private int _current;
        private int? _max;

        public int Current
        {
            get => _current;
            set
            {
                if (_current != value)
                {
                    if (_max == null || _current > _max.Value)
                    {
                        throw new ArgumentException("Current cannot be greater than max in progress tracker.");
                    }

                    _current = value;
                    ProgressChanged?.Invoke();
                }
            }
        }

        public int? Max
        {
            get => _max;
            set
            {
                if (_max != value)
                {
                    if (value == null || value.Value < _current)
                    {
                        throw new ArgumentException("Current cannot be greater than max in progress tracker.");
                    }

                    _max = value ?? throw new ArgumentNullException(nameof(value));
                    ProgressChanged?.Invoke();
                }
            }
        }

        public event Action ProgressChanged;
    }
}
