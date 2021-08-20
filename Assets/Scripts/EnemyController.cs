using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace RipsBigun
{
    public class EnemyController : MonoBehaviour
    {
        protected struct Boundary
        {
            public float min { get; private set; }
            public float max { get; private set; }
            public Boundary(float min, float max)
            {
                this.min = min;
                this.max = max;
            }
        }

        [Header("Base Class Configuration")]
        // serialized health parameters
        [SerializeField]
        protected float _startingHealth = 100f;
        [SerializeField]
        protected bool _invincible = false;
        [SerializeField]
        protected float _giveDamageAmount = 10f;
        [SerializeField]
        protected int _pointValue = 100;
        public int PointValue 
        {
            get
            {
                return _pointValue;
            }
        }

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
        protected Boundary _zBounds = new Boundary(-1.5f, 0f);
        [SerializeField]
        protected Boundary _yBounds = new Boundary(-.5f, .5f);
        [SerializeField]
        protected HealthController _healthBar;

        [SerializeField]
        FloatVariable _gameState;
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
        [SerializeField]
        EnemyEvent _onEnemyDeath; 

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
            if (_pooledObject.behaviour == null)
            {
                _pooledObject.behaviour = this;
            }
        }

        protected virtual void Update()
        {
            _transform.localPosition = new Vector3(
                _transform.localPosition.x,
                Mathf.Clamp(_transform.localPosition.y, _yBounds.min, _yBounds.max),
                Mathf.Clamp(_transform.localPosition.z, _zBounds.min, _zBounds.max)
            );
        }

        protected virtual void OnDisable()
        {
            _animator?.SetBool("dead", false);
            if (_spriteRenderer != null)
            {
                _spriteRenderer.enabled = true;
            }
            if (_collider != null)
            {
                _collider.enabled = true;
            }
            _grounded = false;
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

            if (other.gameObject.layer == 8)
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
                _onEnemyDeath?.Raise(this);
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
