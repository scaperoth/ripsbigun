using UnityEngine;
using TMPro;

namespace RipsBigun
{
    [RequireComponent(typeof(TextMeshPro))]
    public class FlashText : MonoBehaviour
    {
        [SerializeField]
        float _flashTime = .5f;
        float _lastFlashTime = 0f;

        TextMeshPro _tmPro;

        private void Start()
        {
            _tmPro = GetComponent<TextMeshPro>();
        }

        private void Update()
        {
            if (_lastFlashTime + _flashTime < Time.time)
            {
                _tmPro.enabled = !_tmPro.enabled;
                _lastFlashTime = Time.time;
            }
        }
    }
}
