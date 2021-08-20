using System.Collections;
using UnityEngine;

namespace RipsBigun
{
    public class TruckController : EnemyController
    {
        [Header("Truck Configuration")]
        [SerializeField]
        float _moveSpeed = 2f;
        [SerializeField]
        float _gravityModifier = 1f;
        [SerializeField]
        float _lifespan = 5f;
        [SerializeField]
        SpriteRenderer _spriteRenderer;

        bool _grounded = false;
        float _enabledTime = 5f;

        Transform _transform;
        PooledObject _pooledObject;
        Transform _cameraTransform;
        Rigidbody _rb;

        Vector3 _currentTarget = Vector3.zero;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            _transform = transform;
            _pooledObject = GetComponent<PooledObject>();
            _rb = GetComponent<Rigidbody>();
            _pooledObject.behaviour = this;
            _cameraTransform = Camera.main.transform;
        }

        private void OnEnable()
        {
            _enabledTime = Time.time;
            _currentTarget = Vector3.zero;
        }

        private void OnDisable()
        {
            _enabledTime = 0f;
        }

        // Update is called once per frame
        void Update()
        {
            // initial settings for character:
            // gravity, movement, etc.
            ApplyGravity();
            Behavior();

            if(_enabledTime + (_lifespan *.7) < Time.time)
            {
                _moveSpeed *= 1.01f;
            }

            if (_enabledTime + _lifespan < Time.time)
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
            Vector3 currentPos = _transform.position;

            if (_currentTarget == Vector3.zero)
            {
                _currentTarget = new Vector3(_cameraTransform.position.x - 50, currentPos.y, currentPos.z);
                if (currentPos.x < _cameraTransform.position.x)
                {
                    _spriteRenderer.flipX = true;
                    _currentTarget.x += 100;
                }
            }
            Vector3 move = Vector3.MoveTowards(currentPos, _currentTarget, _moveSpeed * Time.deltaTime);
            _transform.position = new Vector3(move.x, move.y, move.z);
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

        void OnTriggerEnter(Collider other)
        {
            // if you hit the floor plane, you're grounded
            if (other.name == "FloorPlane")
            {
                _grounded = true;
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
    }

}