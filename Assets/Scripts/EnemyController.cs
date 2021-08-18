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
    }

}
