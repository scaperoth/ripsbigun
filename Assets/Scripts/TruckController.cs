using System.Collections;
using UnityEngine;

namespace RipsBigun
{
    public class TruckController : EnemyController
    {
        [Header("Truck Configuration")]
        [SerializeField]
        float _accelerator = .01f;
        float _adjustedMoveSpeed;

        [SerializeField]
        float _lifespan = 5f;
        float _lifeStartTime = 5f;
        Vector3 _direction = Vector3.right;
        bool _directionSet = false;

        private void OnEnable()
        {
            _lifeStartTime = Time.time;
            _currentTarget = Vector3.zero;
            _adjustedMoveSpeed = _moveSpeed;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _lifeStartTime = 0f;
            _directionSet = false;
        }

        // Update is called once per frame
        protected void Update()
        {
            Behavior();

            if (_lifeStartTime + (_lifespan * .7) < Time.time)
            {
                _adjustedMoveSpeed *= (1 + _accelerator);
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

            if (!_directionSet )
            {
                if (currentPos.x < _mainCameraTransform.position.x) {
                    FlipSprite(true);
                    _direction = Vector3.right;
                }
                else
                {
                    FlipSprite(false);
                    _direction = Vector3.left;
                }
                _directionSet = true;
            }
            _currentTarget = currentPos + (_direction * 3);
            MoveTowards(currentPos, _currentTarget, _adjustedMoveSpeed * Time.deltaTime);
        }
    }
}