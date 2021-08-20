using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace RipsBigun
{
    [CreateAssetMenu]
    public class FloatVariable : ScriptableObject
    {
        [SerializeField]
        private int _initialValue = 0;
        [SerializeField]
        private int _runtimeValue = 0;
        [SerializeField]
        private int _maxValue = 9999999;

        public UnityEvent<int> OnValueChanged;

        public int Value
        {
            get
            {
                return _runtimeValue;
            }
        }

        public int MaxValueDigits
        {
            get
            {
                return (int)Mathf.Floor(Mathf.Log10(_maxValue) + 1);
            }
        }

        public void OnAfterDeserialize()
        {
            _runtimeValue = _initialValue;
        }

        public void OnBeforeSerialize() { }

        private void OnValidate()
        {
            _initialValue = Mathf.Clamp(_initialValue, 0, _maxValue);
            _runtimeValue = Mathf.Clamp(_runtimeValue, 0, _maxValue);

            if (_runtimeValue < _initialValue)
            {
                _runtimeValue = _initialValue;
            }
            OnValueChanged?.Invoke(_runtimeValue);
        }


        private void OnEnable()
        {
            _runtimeValue = 0;
        }

        public void AddToValue(int amountToAdd)
        {
            int newValue = _runtimeValue + amountToAdd;
            UpdateValue(newValue);
        }

        public void RemoveFromValue(int amountToSubtract)
        {
            int newValue = _runtimeValue - amountToSubtract;
            UpdateValue(newValue);
        }

        public void UpdateValue(int newValue)
        {
            _runtimeValue = Mathf.Clamp(newValue, 0, _maxValue);
            OnValueChanged?.Invoke(_runtimeValue);
        }
    }

}