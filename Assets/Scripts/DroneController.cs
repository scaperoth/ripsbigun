using System.Collections;
using UnityEngine;

namespace RipsBigun
{
    public class DroneController : EnemyController
    {
        [SerializeField]
        float _droneMoveSpeed = 1.5f;
        [SerializeField]
        float _floatSpeed = 1f;
        [SerializeField]
        float _floatMagnitude = 1f;
        [SerializeField]
        float _gravityModifier = 1f;
        [SerializeField]
        float _maxZBounds = 0;
        [SerializeField]
        float _minZBounds = -1.6f;
        [SerializeField]
        float _turningTime = .3f;
        [SerializeField]
        GameObject _healthBar;

        bool _grounded = false;
        bool _turning = false;
        float _lastTurnTime = 0f;
        Vector3 _currentTarget;
        bool _targetSet = false;
        bool _isDead = false;

        Transform _transform;
        Rigidbody _rb;
        Animator _animator;
        SpriteRenderer _spriteRenderer;
        PooledObject _pooledObject;
        CapsuleCollider _collider;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            _transform = transform;
            _rb = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _pooledObject = GetComponent<PooledObject>();
            _collider = GetComponent<CapsuleCollider>();
            _pooledObject.behaviour = this;
            _currentTarget = _playerTransform.position;
        }

        private void OnDisable()
        {
            _animator?.SetBool("dead", false);
            if(_spriteRenderer != null) { 
                _spriteRenderer.enabled = true;
            }
            if(_collider != null)
            {
                _collider.enabled = true;
            }
            _isDead = false;
        }

        // Update is called once per frame
        void Update()
        {
            // initial settings for character:
            // gravity, movement, etc.
            ApplyGravity();
            FLoat();
            Behavior();

            if(_spriteRenderer.enabled == false)
            {
                _pooledObject.Finish();
            }
        }

        /// <summary>
        /// perform this enemy's behavior
        /// </summary>
        /// <param name="currentPos"></param>
        void Behavior()
        {
            if (_isDead)
            {
                return;
            }

            Vector3 currentPos = _transform.position;
            float posX = currentPos.x;
            Vector3 playerPos = _playerTransform.position;

            if (_turning)
            {
                if (_lastTurnTime + _turningTime < Time.time)
                {
                    _turning = false;
                    _animator.SetBool("turn", false);
                    _spriteRenderer.flipX = !_spriteRenderer.flipX;
                }
                return;
            }

            if (!_targetSet)
            {
                if (posX < playerPos.x)
                {
                    _currentTarget = playerPos + (Vector3.right * 2f);
                    _spriteRenderer.flipX = true;
                }
                else
                {
                    _currentTarget = playerPos + (Vector3.left * 2f);
                    _spriteRenderer.flipX = false;
                }

                _targetSet = true;
                return;
            }


            _currentTarget = new Vector3(_currentTarget.x, _currentTarget.y, playerPos.z);
            float distanceToTarget = Vector3.Distance(currentPos, _currentTarget);
            if (distanceToTarget > .6f)
            {
                Vector3 move = Vector3.MoveTowards(currentPos, _currentTarget, _droneMoveSpeed * Time.deltaTime);
                _transform.position = new Vector3(move.x, currentPos.y, move.z);
            }
            else if (distanceToTarget < .6f)
            {
                _animator.SetBool("turn", true);
                _turning = true;
                _targetSet = false;
                _lastTurnTime = Time.time;
            }
        }

        /// <summary>
        /// apply gravity when character not grounded
        /// </summary>
        void ApplyGravity()
        {
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
        void FLoat()
        {
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
                PlayerWeapon weapon = other.GetComponent<PlayerWeapon>();
                if (weapon != null && OnTakeDamage != null)
                {
                    float damage = weapon.GiveDamageAmount;
                    OnTakeDamage.Invoke(damage / _startingHealth);
                    _health -= damage;

                    if (Mathf.Approximately(_health, 0f))
                    {
                        HandleDeath();
                    }
                    else
                    {
                        StartCoroutine("HandleHurtAnimation");
                    }
                }
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

        protected override void HandleDeath()
        {
            base.HandleDeath();
            _animator.SetBool("dead", true);
            _isDead = true;
            _collider.enabled = false;
            Destroy(_healthBar);
        }

        IEnumerator HandleHurtAnimation()
        {
            _spriteRenderer.material.color = Color.red;
            yield return new WaitForSeconds(.1f);
            _spriteRenderer.material.color = Color.white;
        }
    }

}