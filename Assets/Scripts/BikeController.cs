using System.Collections;
using UnityEngine;

namespace RipsBigun
{
    public class BikeController : EnemyController
    {
        // Update is called once per frame
        protected override void Update()
        {
            // initial settings for character:
            // gravity, movement, etc.
            ApplyGravity();
            FLoat();
            Behavior();

            if (_spriteRenderer.enabled == false)
            {
                _pooledObject.Finish();
            }
            base.Update();
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
                    _spriteRenderer.flipX = true;
                }
                else
                {
                    _currentTarget = targetPos + (Vector3.left * 2f);
                    _spriteRenderer.flipX = false;
                }

                _targetSet = true;
                return;
            }


            _currentTarget = new Vector3(_currentTarget.x, _currentTarget.y, targetPos.z);
            float distanceToTarget = Vector3.Distance(currentPos, _currentTarget);
            if (distanceToTarget > .6f)
            {
                Vector3 move = Vector3.MoveTowards(currentPos, _currentTarget, _moveSpeed * Time.deltaTime);
                _transform.position = new Vector3(move.x, currentPos.y, move.z);
            }
            else if (distanceToTarget < .6f)
            {
                _targetSet = false;
            }
        }
    }

}