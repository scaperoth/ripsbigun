using UnityEngine;

namespace RipsBigun
{
    public class DroneController : EnemyController
    {
        [Header("Drone Configuration")]
        [SerializeField]
        float _turningTime = .3f;
        bool _turning = false;
        float _lastTurnTime = 0f;

        // Update is called once per frame
        protected void Update()
        {
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
                if (currentPos.x < playerPos.x)
                {
                    _currentTarget = playerPos + (Vector3.right * 2f);
                    FlipSprite(true);;
                }
                else
                {
                    _currentTarget = playerPos + (Vector3.left * 2f);
                    FlipSprite(false); ;
                }

                _targetSet = true;
                return;
            }


            _currentTarget = new Vector3(_currentTarget.x, _currentTarget.y, playerPos.z);
            float distanceToTarget = Vector3.Distance(currentPos, _currentTarget);
            if (distanceToTarget > .6f)
            {
                MoveTowards(currentPos, _currentTarget, _moveSpeed * Time.deltaTime);
            }
            else if (distanceToTarget < .6f)
            {
                _animator.SetBool("turn", true);
                _turning = true;
                _targetSet = false;
                _lastTurnTime = Time.time;
            }
        }
    }

}