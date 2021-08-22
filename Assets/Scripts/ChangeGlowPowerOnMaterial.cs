
using UnityEngine;
using TMPro;

namespace RipsBigun
{
    [RequireComponent(typeof(TextMeshPro))]
    public class ChangeGlowPowerOnMaterial : MonoBehaviour
    {
        [SerializeField]
        [Range(10, 100)]
        [Tooltip("The higher the value, the slower the change")]
        int _glowFramesToChange = 100;

        [SerializeField]
        float _maxGlow = 1;
        [SerializeField]
        float _minGlow = .5f;

        float _startingGlow;
        float _targetGlow;

        TextMeshPro _tmPro;
        Material _tmProMaterial;
        bool _decreasing = false;
        float _currentStep = 0;
        Vector3 _cachedVector = Vector3.right;

        // Start is called before the first frame update
        void Start()
        {
            _tmPro = GetComponent<TextMeshPro>();
            _tmProMaterial = _tmPro.fontSharedMaterial;
            _startingGlow = _maxGlow;
            _targetGlow = _minGlow;
            SetGlowPower(_maxGlow);
        }

        // Update is called once per frame
        void Update()
        {
            float currentGlow = _tmProMaterial.GetFloat(ShaderUtilities.ID_GlowPower);

            if (!_decreasing && Mathf.Approximately(_maxGlow, currentGlow))
            {
                _decreasing = true;
                _startingGlow = _maxGlow;
                _targetGlow = _minGlow;
                _currentStep = 0;
            }
            else if (_decreasing && Mathf.Approximately(currentGlow, _minGlow))
            {
                _decreasing = false;
                _startingGlow = _minGlow;
                _targetGlow = _maxGlow;
                _currentStep = 0;
            }

            // total steps is calculated by getting the inverse of the speed
            // and that is multiplied by thte static number of frames we want to use for animation
            float totalSteps = _glowFramesToChange;
            float stepRatio = _currentStep / totalSteps;
            Vector3 lerp = Vector3.Lerp(_cachedVector * _startingGlow, _cachedVector * _targetGlow, stepRatio);
            SetGlowPower(lerp.x);
            _currentStep++;
        }

        void SetGlowPower(float glowPower)
        {
            _tmProMaterial.SetFloat(ShaderUtilities.ID_GlowPower, glowPower);
            // Since some of the material properties can affect the mesh (size) you would need to update the padding values.
            _tmPro.UpdateMeshPadding();
        }
    }
}