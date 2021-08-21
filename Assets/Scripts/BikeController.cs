using UnityEngine;

namespace RipsBigun
{
    public class BikeController : EnemyController
    {
        // Update is called once per frame
        protected void Update()
        {
            Behavior();

            if (_spriteRenderer.enabled == false)
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
            Vector3 targetPos = _playerTransform.position;

            if (!_targetSet)
            {
                if (currentPos.x < targetPos.x)
                {
                    _currentTarget = targetPos + (Vector3.right * 2f);
                    FlipSprite(true);
                }
                else
                {
                    _currentTarget = targetPos + (Vector3.left * 2f);
                    FlipSprite(false);
                }

                _targetSet = true;
                return;
            }


            _currentTarget = new Vector3(_currentTarget.x, _currentTarget.y, targetPos.z);
            float distanceToTarget = Vector3.Distance(currentPos, _currentTarget);
            if (distanceToTarget > .6f)
            {
                MoveTowards(currentPos, _currentTarget, _moveSpeed * Time.deltaTime);
            }
            else if (distanceToTarget < .6f)
            {
                _targetSet = false;
            }
        }
    }

}