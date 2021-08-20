using System.Collections.Generic;
using UnityEngine;

namespace RipsBigun
{
    [ExecuteInEditMode]
    public class FloatVariable : ScriptableObject
    {
        [SerializeField]
        private float _initialValue = 0;
        [SerializeField]
        private float _runtimeValue = 0;
        [SerializeField]
        private bool _runtimeLessThanIntial = false;
        [SerializeField]
        private bool _runtimeGreaterThanInitial = false;
        [SerializeField]
        private int _minValue = 0;
        [SerializeField]
        private int _maxValue = 9999999;

        public float InitialValue
        {
            get
            {
                return _initialValue;
            }
        }

        public float Value
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

        private List<FloatVariableEventListener> listeners =
            new List<FloatVariableEventListener>();

        void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
                listeners[i].OnEventRaised(this);
        }

        public void RegisterListener(FloatVariableEventListener listener)
        { listeners.Add(listener); }

        public void UnregisterListener(FloatVariableEventListener listener)
        { listeners.Remove(listener); }

        public void OnAfterDeserialize()
        {
            _runtimeValue = _initialValue;
        }

        public void OnBeforeSerialize() { }

        private void OnValidate()
        {
            _initialValue = Mathf.Clamp(_initialValue, _minValue, _maxValue);
            _runtimeValue = Mathf.Clamp(_runtimeValue, _minValue, _maxValue);

            if (_runtimeLessThanIntial && _runtimeValue > _initialValue)
            {
                _runtimeValue = _initialValue;
            }

            if (_runtimeGreaterThanInitial && _runtimeValue < _initialValue)
            {
                _runtimeValue = _initialValue;
            }
            Raise();
        }


        private void OnEnable()
        {
            _runtimeValue = _initialValue;
        }

        public void AddToValue(float amountToAdd)
        {
            float newValue = _runtimeValue + amountToAdd;
            UpdateValue(newValue);
        }

        public void SubtractFromValue(float amountToSubtract)
        {
            float newValue = _runtimeValue - amountToSubtract;
            UpdateValue(newValue);
        }

        public void UpdateValue(float newValue)
        {
            _runtimeValue = Mathf.Clamp(newValue, 0, _maxValue);
            Raise();
        }
    }

}