using UnityEngine;
using UnityEngine.Events;

namespace RipsBigun
{
    public class EnemyController : MonoBehaviour
    {
        [Header("Base Class Configuration")]
        [SerializeField]
        protected float _startingHealth = 100f;
        protected float _health;

        [SerializeField]
        protected float _giveDamageAmount = 10f;

        [Header("Enemy Class Configuration")]

        private UnityEvent<EnemyController> _onDeath;
        public UnityEvent<EnemyController> OnDeath { get { return _onDeath; } }

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
            if (_onDeath != null)
            {
                _onDeath.RemoveAllListeners();
            }
        }

        public void SetPlayerTransform(Transform playerTransform)
        {
            _playerTransform = playerTransform;
        }

        protected virtual void HandleDeath()
        {
            if (_onDeath != null)
            {
                _onDeath.Invoke(this);
            }
        }
    }

}
