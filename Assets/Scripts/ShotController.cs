using UnityEngine;

namespace RipsBigun
{
    public class ShotController : MonoBehaviour
    {
        [SerializeField]
        private float _shotVelocity = 5f;
        [SerializeField]
        private float _lifeSpan = 5f;
        private float _spawnTime;
        private Transform _transform;
        private int _direction = 1;
        private SpriteRenderer _spriteRenderer;
        private SpriteRenderer _parentRenderer;

        // Start is called before the first frame update
        void Awake()
        {
            _spawnTime = Time.time;
            _transform = transform;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _parentRenderer = _transform.parent.gameObject.GetComponent<SpriteRenderer>();
            Init(_parentRenderer.flipX);
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.time - _spawnTime > _lifeSpan)
            {
                Destroy(gameObject);
            }
            else
            {
                _transform.Translate(new Vector3(_direction * _shotVelocity * Time.deltaTime, 0, 0));
            }
        }

        public void Init(bool flip)
        {
            if (flip)
            {
                _direction = -1;
                Vector3 localPos = _transform.localPosition;
                _transform.localPosition = new Vector3(localPos.x * _direction, localPos.y, localPos.z);
            }
            else
            {
                _direction = 1;
            }

            if (_spriteRenderer != null)
            {
                _spriteRenderer.flipX = flip;
            }
            _transform.parent = null;
        }
    }

}