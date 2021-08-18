using UnityEngine;


namespace RipsBigun
{
    public sealed class HealthController : MonoBehaviour
    {
        [SerializeField]
        SpriteRenderer _healthBarSprite;
        private float _startingHealthbarSize;

        private void Start()
        {
            if (_healthBarSprite != null)
            {
                _startingHealthbarSize = _healthBarSprite.size.x;
            }
        }

        public void UpdateHealth(float percDamage)
        {
            if (_healthBarSprite == null)
            {
                return;
            }

            float sizeChange = _startingHealthbarSize * percDamage;

            float newSize = _healthBarSprite.size.x - sizeChange;
            Debug.Log($"CHANGING HEALTH TO {newSize} because of size change {sizeChange} from {_startingHealthbarSize}");

            _healthBarSprite.size = new Vector2(Mathf.Clamp(newSize, 0, _startingHealthbarSize), _healthBarSprite.size.y);
        }
    }
}
