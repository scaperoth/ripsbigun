using UnityEngine;
using UnityEngine.Events;

namespace RipsBigun
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField]
        protected float _startingHealth = 100f;
        protected float _health;

        [SerializeField]
        protected float _giveDamageAmount = 10f;
        [SerializeField]
        protected UnityEvent<float> OnTakeDamage;

        public UnityEvent<EnemyController> OnDeath;

        protected Transform _playerTransform;

        public float GiveDamageAmount
        {
            get
            {
                return _giveDamageAmount;
            }
        }

        protected virtual void Start()
        {
            _health = _startingHealth;

        }

        private void OnDisable()
        {
            if(OnDeath != null)
            {
                OnDeath.RemoveAllListeners();
            }
        }

        public void SetPlayerTransform(Transform playerTransform)
        {
            _playerTransform = playerTransform;
        }

        protected virtual void HandleDeath()
        {
            if (OnDeath != null)
            {
                OnDeath.Invoke(this);
            }
        }
    }

}
