using UnityEngine;
using UnityEngine.Events;

namespace RipsBigun
{
    public class FloatVariableEventListener : MonoBehaviour
    {
        public FloatVariable Event;
        public UnityEvent<FloatVariable> Response;

        private void OnEnable()
        { Event.RegisterListener(this); }

        private void OnDisable()
        { Event.UnregisterListener(this); }

        public void OnEventRaised(FloatVariable variable)
        { Response.Invoke(variable); }
    }

}