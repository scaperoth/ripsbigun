using UnityEngine;
using UnityEngine.Events;

namespace RipsBigun
{
    public class PlayerEventListener : MonoBehaviour
    {
        public PlayerEvent Event;
        public UnityEvent<PlayerController> Response;

        private void OnEnable()
        { Event.RegisterListener(this); }

        private void OnDisable()
        { Event.UnregisterListener(this); }

        public void OnEventRaised(PlayerController player)
        { Response.Invoke(player); }
    }

}