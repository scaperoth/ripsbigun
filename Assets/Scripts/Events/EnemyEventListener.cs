using UnityEngine;
using UnityEngine.Events;

namespace RipsBigun
{
    public class EnemyEventListener : MonoBehaviour
    {
        public EnemyEvent Event;
        public UnityEvent<EnemyController> Response;

        private void OnEnable()
        { Event.RegisterListener(this); }

        private void OnDisable()
        { Event.UnregisterListener(this); }

        public void OnEventRaised(EnemyController enemy)
        { Response.Invoke(enemy); }
    }

}