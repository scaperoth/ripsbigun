using System.Collections;
using UnityEngine;

namespace RipsBigun
{
    public class TruckController : EnemyController
    {
        [Header("Truck Configuration")]
        [SerializeField]
        float _accelerator = .01f;

        [SerializeField]
        float _lifespan = 5f;
        float _lifeStartTime = 5f;

        private void OnEnable()
        {
            _lifeStartTime = Time.time;
            _currentTarget = Vector3.zero;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _lifeStartTime = 0f;
        }

        // Update is called once per frame
        void Update()
        {
            // initial settings for character:
            // gravity, movement, etc.
            ApplyGravity();
            Behavior();

            if (_lifeStartTime + (_lifespan * .7) < Time.time)
            {
                _moveSpeed *= (1 + _accelerator);
            }

            if (_lifeStartTime + _lifespan < Time.time)
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
    }
}