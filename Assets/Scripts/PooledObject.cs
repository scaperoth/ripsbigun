using System;
using UnityEngine;
using UnityEngine.Events;

namespace RipsBigun
{
    public class PooledObject : MonoBehaviour
    {
        [HideInInspector]
        public int id;
        public Action<PooledObject> Finished;
        public UnityEvent<PooledObject> OnDespawn;

        // A cached component for fast-access -- avoids calls to GetComponent<>().
        public Component behaviour;

        public T As<T>() where T : Component
        {
            return behaviour as T;
        }

        public void Finish()
        {
            if (Finished != null)
            {
                Finished(this);

                if (OnDespawn != null)
                {
                    OnDespawn.Invoke(this);
                }
            }
        }

        // Convenience method to call finish when particles finish.
        // Needs ParticleSystem stop action to be set to "Callback".
        private void OnParticleSystemStopped()
        {
            Finish();
        }
    }
}