using System.Collections;
using UnityEngine;

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
        protected bool _useGravity = true;
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
        protected float _gravityValue = -9.8f;
        [SerializeField]
        protected Boundary _zBounds = new Boundary(-1.5f, 0f);
        [SerializeField]
        protected Boundary _yBounds = new Boundary(-.5f, .5f);
        [SerializeField]
        protected HealthController _healthBar;

        [SerializeField]
        FloatVariable _score;
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
        protected bool _isGrounded = false;
        protected bool _isDead = false;
        protected bool _gameOver = false;

        private bool _updatingPositionThisFrame = false;
        private Vector3 _newPositionForFrame = Vector3.zero;
        private bool _flipX = false;
        private bool _updatingSpriteXThisFrame = false;

        protected Transform _playerTransform;
        protected Transform _transform;
        protected Transform _mainCameraTransform;
        protected Vector3 _cachedPosition;

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
            _mainCameraTransform = Camera.main.transform;
            if (_pooledObject.behaviour == null)
            {
                _pooledObject.behaviour = this;
            }
        }

        protected virtual void LateUpdate()
        {
            if (_isDead || _gameOver)
            {
                return;
            }

            if (_updatingSpriteXThisFrame)
            {
                _spriteRenderer.flipX = _flipX;
                _updatingSpriteXThisFrame = false;
            }

            if (!_updatingPositionThisFrame)
            {
                _newPositionForFrame = _transform.position;
            }

            if (!_isGrounded && _useGravity)
            {
                _newPositionForFrame += Physics.gravity * Time.deltaTime;
            }

            // make sure that the character stays within defined boundaries
            float yBounds = Mathf.Clamp(_newPositionForFrame.y, _yBounds.min, _yBounds.max);
            float zBounds = Mathf.Clamp(_newPositionForFrame.z, _zBounds.min, _zBounds.max);

            _transform.position = new Vector3(
                _newPositionForFrame.x,
                yBounds,
                zBounds
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
            _isGrounded = false;
            _isDead = false;
            _healthBar?.ShowHealth(true);
            _healthBar?.ResetHealth();
            _health = _startingHealth;
            _updatingPositionThisFrame = false;
        }

        public void SetPlayerTransform(Transform playerTransform)
        {
            _playerTransform = playerTransform;
        }

        protected void MoveTowards(Vector3 currentPosition, Vector3 target, float speed)
        {
            Vector3 move = Vector3.Lerp(currentPosition, target, speed);
            _newPositionForFrame = new Vector3(move.x, currentPosition.y, move.z);
            if (!_updatingPositionThisFrame)
            {
                _updatingPositionThisFrame = true;
            }
        }

        protected void FlipSprite(bool flipX)
        {
            _flipX = flipX;
            _updatingSpriteXThisFrame = true;
        }

        protected virtual void HandleDeath()
        {
            _healthBar?.ShowHealth(false);
            _animator?.SetBool("dead", true);
            _isDead = true;
            _collider.enabled = false;
            _score?.AddToValue(PointValue);
            _onEnemyDeath?.Raise(this);
        }

        void OnTriggerEnter(Collider other)
        {
            // if you hit the floor plane, you're grounded
            if (other.name == "FloorPlane")
            {
                _isGrounded = true;
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
                _health -= damage;
                _healthBar?.UpdateHealth(_health / _startingHealth);
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

        public void HandleGameOver()
        {
            _gameOver = true;
        }

        void OnTriggerExit(Collider other)
        {
            // if you leave the floor plane, you're not grounded
            if (other.name == "FloorPlane")
            {
                _isGrounded = false;
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
