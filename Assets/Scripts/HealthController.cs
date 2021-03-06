using UnityEngine;

namespace RipsBigun
{
    public sealed class HealthController : MonoBehaviour
    {
        [SerializeField]
        SpriteRenderer _healthBarSprite;
        private float _startingHealthbarSize;

        private void Awake()
        {
            if (_healthBarSprite != null)
            {
                _startingHealthbarSize = _healthBarSprite.size.x;
            }
        }

        public void ShowHealth(bool active)
        {
            gameObject.SetActive(active);
        }

        public void ResetHealth()
        {
            if (_healthBarSprite == null)
            {
                return;
            }
            _healthBarSprite.size = new Vector2(_startingHealthbarSize, _healthBarSprite.size.y);
        }

        public void UpdateHealth(float ratioDamage)
        {
            if (_healthBarSprite == null)
            {
                return;
            } 

            float newSize = _startingHealthbarSize * ratioDamage;
            _healthBarSprite.size = new Vector2(Mathf.Clamp(newSize, 0, _startingHealthbarSize), _healthBarSprite.size.y);
        }

        public void UpdateHealth(FloatVariable healthVariable)
        {
            UpdateHealth(healthVariable.Value/healthVariable.InitialValue);
        }
    }
}
