using System.Collections;
using UnityEngine;

namespace RipsBigun
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        FloatVariable _playerHealth;
        [SerializeField]
        private float _xMoveSpeed = 2.0f;
        [SerializeField]
        private float _zMoveSpeed = 5.0f;
        [SerializeField]
        private bool _applyGravity = true;
        [SerializeField]
        private float _jumpSpeed = 2.0f;
        [SerializeField]
        private float _jumpHeight = 2.0f;
        [SerializeField]
        private PooledObject _gunShot;
        [SerializeField]
        private float _shootDelay = .5f;
        [SerializeField]
        private float _hurtDelay = .5f;
        public float HurtDelay
        {
            get
            {
                return _hurtDelay;
            }
        }

        [SerializeField]
        private RestrictedVector3 _levelBounds;
        [SerializeField]
        PlayerEvent _playerHurtEvent;
        [SerializeField]
        PlayerEvent _playerDeathEvent;
        [SerializeField]
        GameEvent _gameOverEvent;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private Animator _animator;

        private Transform _transform;

        private float _lastShootTime = 0f;
        private float _runBoundary = .7f;
        private bool _isGrounded = false;
        private bool _hurt = false;
        private bool _jumping = false;
        private bool _dead = false;
        private Vector3 _move;

        private void Start()
        {
            _transform = transform;
        }

        private void OnEnable()
        {
            _dead = false;
        }

        void Update()
        {
            CheckForDeath();

            if (_dead)
            {
                if (_transform.position.y != 0f)
                {
                    Vector3 centerScreen = new Vector3(_transform.position.x, 0f, _transform.position.z);
                    _transform.position = Vector3.MoveTowards(_transform.position, centerScreen, .5f * Time.deltaTime);
                }

                return;
            }

            CheckForGrounded();

            _move = Vector3.zero;
            if (!_hurt)
            {
                Vector3 inputs = new Vector3(
                    Input.GetAxis("Horizontal"),
                    0,
                    Input.GetAxis("Vertical")
                );

                _move = inputs;

                HandleGrounded();

                CorrectSpriteOrientation(_move);

                AnimateCharacterMovement(_move);

                HandleShoot();

                _move.x *= _xMoveSpeed;
                _move.z *= _zMoveSpeed;
                _move = HandleJump(_move);
            }

            _move *= Time.deltaTime;
            _transform.position = ClampBoundary(_transform.position + _move);
        }

        void CheckForGrounded()
        {
            if (Mathf.Approximately(_transform.position.y, _levelBounds.Min.y))
            {
                _isGrounded = true;
            }
            else
            {
                _isGrounded = false;
            }
        }

        Vector3 ClampBoundary(Vector3 move)
        {
            Vector3 clampedMove = new Vector3(
                move.x,
                Mathf.Clamp(move.y, _levelBounds.Min.y, _levelBounds.Max.y),
                Mathf.Clamp(move.z, _levelBounds.Min.z, _levelBounds.Max.z)
            );

            return clampedMove;
        }

        void HandleShoot()
        {
            float time = Time.time;
            //Changes the height position of the player..
            if (Input.GetButtonDown("Fire1"))
            {
                _animator.SetBool("shoot", true);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                _animator.SetBool("shoot", false);
            }

            if (_lastShootTime + _shootDelay > time)
            {
                return;
            }

            if (_animator.GetBool("shoot") && _gunShot != null)
            {
                _lastShootTime = time;
                Vector3 gunPos = _gunShot.CachedTransform.localPosition;
                if (_animator.GetBool("run"))
                {
                    gunPos += new Vector3(.1f, -.05f, 0f);
                }
                Vector3 shotPosition = _transform.localPosition + gunPos;

                PooledObject instance = Pool.Instance.Spawn(_gunShot, shotPosition, Quaternion.Euler(0f, 0f, 0f));
                instance.As<ShotController>().Init(_spriteRenderer.flipX);
            }
        }

        void CorrectSpriteOrientation(Vector3 move)
        {
            if (move.x < -.001f)
            {
                _spriteRenderer.flipX = true;
            }
            else if (move.x > .001f)
            {
                _spriteRenderer.flipX = false;
            }
        }

        Vector3 HandleJump(Vector3 move)
        {
            float clampedJumpHeight = Mathf.Min(_jumpHeight, _levelBounds.Max.y);
            if (_jumping && _transform.position.y < clampedJumpHeight)
            {
                float realSpeed = (1f / _jumpHeight) * _jumpSpeed;
                float distanceToPeakJump = (clampedJumpHeight - _transform.position.y) + Time.deltaTime;
                move += new Vector3(0f, _jumpHeight * (realSpeed * distanceToPeakJump), 0);
            }
            else if (_transform.position.y >= clampedJumpHeight)
            {
                _jumping = false;
            }

            if (!_jumping && _applyGravity)
            {
                move += Vector3.down * 3;
            }

            //Changes the height position of the player..
            if (_isGrounded && Input.GetButtonDown("Jump"))
            {
                _animator.SetBool("jump", true);
                _jumping = true;
            }
            return move;
        }

        void HandleGrounded()
        {
            if (!_isGrounded)
            {
                _animator.SetBool("jump", true);
                _animator.SetBool("run", false);
                _animator.SetBool("walk", false);
                _animator.SetBool("shoot", false);
            }
            else
            {
                _animator.SetBool("jump", false);
            }
        }

        void AnimateCharacterMovement(Vector3 move)
        {
            float absHMovement = Mathf.Abs(move.x);
            float absVMovement = Mathf.Abs(move.z);
            if ((absHMovement < 0.01 && absVMovement < 0.01))
            {
                _animator.SetBool("run", false);
                _animator.SetBool("walk", false);
            }
            else if (_isGrounded && absHMovement > _runBoundary)
            {
                _animator.SetBool("run", true);
                _animator.SetBool("walk", false);
            }
            else if ((absHMovement > 0.01 || absVMovement > 0.001) && _isGrounded)
            {
                _animator.SetBool("run", false);
                _animator.SetBool("walk", true);
            }
            else
            {
                _animator.SetBool("run", false);
                _animator.SetBool("walk", true);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_dead && other.gameObject.layer == 7)
            {
                if (_hurt)
                {
                    return;
                }

                RecoilFromOther(other.gameObject);
                TakeDamageFromOther(other.gameObject);
                StartCoroutine("HandleHurtAnimation");
            }
        }

        void RecoilFromOther(GameObject other)
        {
            // get hit and have some recoil
            Transform otherBody = other.GetComponent<Transform>();
            float hitSide = Mathf.Sign(otherBody.position.x - _transform.position.x);
            if (hitSide < 0)
            {
                _spriteRenderer.flipX = true;
            }
            else
            {
                _spriteRenderer.flipX = false;
            }
            Vector3 hitreaction = new Vector3(-hitSide, _transform.position.y, _transform.position.z);
            transform.position = Vector3.MoveTowards(_transform.position, hitreaction, Time.deltaTime);
        }

        void TakeDamageFromOther(GameObject other)
        {
            EnemyController enemyController = other.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                float damage = enemyController.GiveDamageAmount;
                _playerHealth.SubtractFromValue(damage);
                _playerHurtEvent.Raise(this);
                if(_playerHealth.Value == 0)
                {
                    _dead = true;
                }
            }
        }

        IEnumerator HandleHurtAnimation()
        {
            _hurt = true;
            _animator.SetBool("hurt", true);
            _animator.SetBool("jump", false);
            _animator.SetBool("run", false);
            _animator.SetBool("walk", false);
            _animator.SetBool("shoot", false);

            yield return new WaitForSeconds(_hurtDelay);

            _hurt = false;
            _spriteRenderer.enabled = true;
            _animator.SetBool("hurt", false);
        }

        void CheckForDeath()
        {
            if (_dead && _playerHealth.Value == 0)
            {
                _spriteRenderer.sortingOrder += 2;
                _animator.SetBool("hurt", false);
                _animator.SetBool("jump", false);
                _animator.SetBool("run", false);
                _animator.SetBool("walk", false);
                _animator.SetBool("shoot", false);
                _animator.SetTrigger("deathTrigger");
                _animator.SetBool("dead", true);
                _playerDeathEvent.Raise(this);
                _gameOverEvent.Raise();
                _playerHealth.UpdateValue(_playerHealth.InitialValue);
            }
        }
    }
}