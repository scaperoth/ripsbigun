using System.Collections;
using UnityEngine;

namespace RipsBigun
{
    public class ShotController : PlayerWeapon
    {
        [SerializeField]
        private float _shotVelocity = 5f;
        [SerializeField]
        private float _lifeSpan = 5f;

        private float _spawnTime = 0f;
        private Transform _transform;
        private int _direction = 1;
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        PooledObject _pooledObject;
        private Transform _startTransform;

        // Start is called before the first frame update
        void Awake()
        {
            _transform = transform;
            _startTransform = _transform;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            _pooledObject = GetComponent<PooledObject>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_spawnTime + _lifeSpan < Time.time || _spriteRenderer.enabled == false)
            {
                _pooledObject.Finish();
                _spriteRenderer.enabled = true;
            }
            else
            {
                _transform.Translate(new Vector3(_direction * _shotVelocity * Time.deltaTime, 0, 0));
            }
        }

        public void Init(bool flip)
        {
            _spawnTime = Time.time;
            _transform.position = _startTransform.position;
            if (flip)
            {
                _direction = -1;
                _transform.localPosition += Vector3.left * .8f;
            }
            else
            {
                _direction = 1;
            }

            if (_spriteRenderer != null)
            {
                _spriteRenderer.flipX = flip;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == 7)
            {
                HandleHit();
            }
        }

        void HandleHit()
        {
            _direction = 0;
            _animator.SetBool("hit", true);
        }
    }

}