using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace RipsBigun
{
    public class EnemyController : MonoBehaviour
    {
        [Header("Base Class Configuration")]
        // serialized health parameters
        [SerializeField]
        protected float _startingHealth = 100f;
        [SerializeField]
        protected bool _invincible = false;
        [SerializeField]
        protected float _giveDamageAmount = 10f;

        // serialized movement parameters
        [SerializeField]
        protected float _moveSpeed = 1.5f;
        [SerializeField]
        protected float _floatSpeed = 1f;
        [SerializeField]
        protected float _floatMagnitude = 1f;
        [SerializeField]
        protected float _gravityModifier = 1f;
        [SerializeField]
        protected HealthController _healthBar;

        [SerializeField]
        protected Rigidbody _rb;
        [SerializeField]
        protected Animator _animator;
        [SerializeField]
        protected SpriteRenderer _spriteRenderer;
        [SerializeField]
        protected PooledObject _pooledObject;
        [SerializeField]
        protected Collider _collider;

        private UnityEvent<EnemyController> _onDeath;
        public UnityEvent<EnemyController> OnDeath { get { return _onDeath; } }

        protected Vector3 _currentTarget = Vector3.zero;
        protected bool _targetSet = false;
        protected float _health;
        protected bool _grounded = false;
        protected bool _isDead = false;

        protected Transform _transform;
        protected Transform _cameraTransform;
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
            _transform = transform;
            _cameraTransform = Camera.main.transform;
            if(_pooledObject.behaviour == null)
            {
                _pooledObject.behaviour = this;
            }
        }

        protected virtual void OnDisable()
        {
            _onDeath?.RemoveAllListeners();
            _animator?.SetBool("dead", false);
            if (_spriteRenderer != null)
            {
                _spriteRenderer.enabled = true;
            }
            if (_collider != null)
            {
                _collider.enabled = true;
            }
            _isDead = false;
            _healthBar?.ShowHealth(true);
            _healthBar?.ResetHealth();
            _health = _startingHealth;
        }

        public void SetPlayerTransform(Transform playerTransform)
        {
            _playerTransform = playerTransform;
        }

        protected virtual void HandleDeath()
        {
            _onDeath?.Invoke(this);
            _healthBar?.ShowHealth(false);
            _animator?.SetBool("dead", true);
            _isDead = true;
            _collider.enabled = false;
        }

        /// <summary>
        /// apply gravity when character not grounded
        /// </summary>
        protected void ApplyGravity()
        {
            if (_isDead)
            {
                return;
            }

            if (!_grounded)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, -_gravityModifier, _rb.velocity.z);
            }
            else
            {
                _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            }
        }

        /// <summary>
        /// float character using sine wave and
        /// the transform position
        /// </summary>
        protected void FLoat()
        {
            if (_isDead)
            {
                return;
            }

            if (_grounded)
            {
                // float
                _rb.velocity = (transform.up * Mathf.Sin(Time.time * _floatSpeed) * _floatMagnitude);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            // if you hit the floor plane, you're grounded
            if (other.name == "FloorPlane")
            {
                _grounded = true;
            }
            else if (other.gameObject.layer == 8)
            {
                if (_invincible)
                {
                    return;
                }

                PlayerWeapon weapon = other.GetComponent<PlayerWeapon>();
                TakeDamage(weapon);
            }
        }

        protected virtual void TakeDamage(PlayerWeapon weapon)
        {
            if (weapon != null)
            {
                float damage = weapon.GiveDamageAmount;
                _healthBar?.UpdateHealth(damage / _startingHealth);
                _health -= damage;
            }

            if (Mathf.Approximately(_health, 0f))
            {
                HandleDeath();
            }
            else
            {
                StartCoroutine("HandleHurtAnimation");
            }
        }

        void OnTriggerExit(Collider other)
        {
            // if you leave the floor plane, you're not grounded
            if (other.name == "FloorPlane")
            {
                _grounded = false;
            }
        }

        IEnumerator HandleHurtAnimation()
        {
            _spriteRenderer.material.color = Color.red;
            yield return new WaitForSeconds(.1f);
            _spriteRenderer.material.color = Color.white;
        }
    }

}
