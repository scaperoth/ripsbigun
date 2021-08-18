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

        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        private Rigidbody _rb;
        [SerializeField]
        private float _playerSpeed = 2.0f;
        [SerializeField]
        private float _playerVeritcalSpeed = 2.0f;
        [SerializeField]
        private float jumpHeight = 2.0f;
        [SerializeField]
        private GameObject _gunShot;
        [SerializeField]
        private float _shootDelay = .5f;
        [SerializeField]
        private float _hurtDelay = .5f;
        private float _lastShootTime = 0f;

        private float _runBoundary = .7f;
        private bool _isGrounded = false;
        private bool _hurt = false;
        private Transform _transform;

        [SerializeField]
        UnityEvent<float> OnTakeDamage;

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
                _spriteRenderer.enabled = !_spriteRenderer.enabled;
                return;
            }

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

            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            if (move.x < -.001f)
            {
                _spriteRenderer.flipX = true;
            }
            else if (move.x > .001f)
            {
                _spriteRenderer.flipX = false;
            }

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

            //Changes the height position of the player..
            if (Input.GetButtonDown("Fire1"))
            {
                _animator.SetBool("shoot", true);
                if (_gunShot != null && _lastShootTime < (Time.time + _shootDelay))
                {
                    Instantiate(_gunShot, _transform);
                    _lastShootTime = Time.time;
                }
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                _animator.SetBool("shoot", false);
            }

            // boundaries
            if (_transform.localPosition.z > 0 && move.z > 0)
            {
                move.z = 0;
            }

            if (_transform.localPosition.z < -1.5 && move.z < 0)
            {
                move.z = 0;
            }

            move.x *= _playerSpeed;
            move.z *= _playerVeritcalSpeed;

            _rb.velocity = move;
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

                // get hit and have some recoil
                Transform otherBody = other.GetComponent<Transform>();
                float hitSide = Mathf.Sign(otherBody.position.x - _transform.position.x);
                _rb.velocity = new Vector3(-hitSide, _rb.velocity.y, _rb.velocity.z);

                EnemyController enemyController = other.GetComponent<EnemyController>();
                if (enemyController != null && OnTakeDamage != null)
                {
                    float damage = enemyController.GiveDamageAmount;
                    OnTakeDamage.Invoke(damage/_startingHealth);
                    _health -= damage;
                }

                StartCoroutine("HandleHurtAnimation");
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

            yield return new WaitForSeconds(_hurtDelay);
            _hurt = false;
            _spriteRenderer.enabled = true;
            _animator.SetBool("hurt", false);
        }
    }

}