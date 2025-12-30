namespace DiceBattle
{
    public class Observable<T>
    {
        public delegate void OnValueChanged(T current, T previous);

        public event OnValueChanged ValueChanged;

        public T Value
        {
            get => _value;
            set
            {
                if (_value == null && value == null)
                    return;

                if (_value != null && _value.Equals(value))
                    return;

                var prev = _value;
                _value = value;
                ValueChanged?.Invoke(_value, prev);
            }
        }

        private T _value;

        public static implicit operator T(Observable<T> observable) => observable._value;
        public static implicit operator Observable<T>(T value) => new() { _value = value };

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
