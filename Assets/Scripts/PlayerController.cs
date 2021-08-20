using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace RipsBigun
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float _startingHealth = 100f;
        private float _health;
        [SerializeField]
        private float _playerSpeed = 2.0f;
        [SerializeField]
        private float _playerVeritcalSpeed = 2.0f;
        [SerializeField]
        private float jumpHeight = 2.0f;
        [SerializeField]
        private PooledObject _gunShot;
        [SerializeField]
        private float _shootDelay = .5f;
        [SerializeField]
        private float _hurtDelay = .5f;
        [SerializeField]
        private float _minZBoundary = -1.5f;
        [SerializeField]
        private float _maxZBoundary = 0f;
        [SerializeField]
        UnityEvent<float> OnTakeDamage;

        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        private Rigidbody _rb;
        private Transform _transform;

        private float _lastShootTime = 0f;
        private float _runBoundary = .7f;
        private bool _isGrounded = false;
        private bool _hurt = false;

        private void Start()
        {
            _transform = transform;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody>();
            _health = _startingHealth;
        }

        void Update()
        {
            if (_hurt)
            {
                return;
            }

            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            HandleGrounded();

            CorrectSpriteOrientation(move);

            AnimateCharacterMovement(move);

            HandleShoot();

            move = HandleJump(move);
            move.x *= _playerSpeed;
            move.z *= _playerVeritcalSpeed;

            _rb.velocity = ClampBoundary(move);
        }

        Vector3 ClampBoundary(Vector3 move)
        {
            Vector3 clampedMove = move;

            // boundaries
            if (_transform.localPosition.z > _maxZBoundary && move.z > 0)
            {
                clampedMove.z = 0;
            }

            if (_transform.localPosition.z < _minZBoundary && move.z < 0)
            {
                clampedMove.z = 0;
            }

            return clampedMove;
        }

        void HandleShoot()
        {

            //Changes the height position of the player..
            if (Input.GetButtonDown("Fire1"))
            {
                _animator.SetBool("shoot", true);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                _animator.SetBool("shoot", false);
            }

            if (_lastShootTime + _shootDelay > Time.time)
            {
                return;
            }

            if (_animator.GetBool("shoot") && _gunShot != null)
            {
                _lastShootTime = Time.time;
                Vector3 gunPos = _gunShot.transform.localPosition;
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
            //Changes the height position of the player..
            if (Input.GetButtonDown("Jump") && _isGrounded)
            {
                move.y = jumpHeight;
                _animator.SetBool("jump", true);
            }
            else
            {
                move.y = _rb.velocity.y;
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
            if (_rb.velocity == Vector3.zero || (absHMovement < 0.01 && absVMovement < 0.01))
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

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.name == "FloorPlane")
            {
                _isGrounded = true;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.name == "FloorPlane")
            {
                _isGrounded = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 7)
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
            _rb.velocity = new Vector3(-hitSide, _rb.velocity.y, _rb.velocity.z);
        }

        void TakeDamageFromOther(GameObject other)
        {
            EnemyController enemyController = other.GetComponent<EnemyController>();
            if (enemyController != null && OnTakeDamage != null)
            {
                float damage = enemyController.GiveDamageAmount;
                OnTakeDamage.Invoke(damage / _startingHealth);
                _health -= damage;
            }
        }

        IEnumerator HandleHurtAnimation()
        {
            _hurt = true;
            _animator.SetBool("hurt", true);
            _animator.SetBool("jump", false);
            _animator.SetBool("shoot", false);
            _animator.SetBool("run", false);
            _animator.SetBool("walk", false);

            int n = 10;
            for (int i = 0; i < n;  i++) {
                _spriteRenderer.enabled = true;
                yield return new WaitForSeconds(_hurtDelay/ (n * 2f));
                _spriteRenderer.enabled = false;
                yield return new WaitForSeconds(_hurtDelay / (n * 2f));
                _spriteRenderer.enabled = true;
            }
            //yield return new WaitForSeconds(_hurtDelay);
            _hurt = false;
            _spriteRenderer.enabled = true;
            _animator.SetBool("hurt", false);
        }
    }

}