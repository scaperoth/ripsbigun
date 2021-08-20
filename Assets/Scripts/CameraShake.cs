using UnityEngine;
using Cinemachine;
using System;
using System.Collections;

namespace RipsBigun
{
    public class CameraShake : MonoBehaviour
    {
        [Serializable]
        private struct ShakeParams
        {
            [SerializeField]
            float _amplitude;
            public float Amplitude
            {
                get
                {
                    return _frequency;
                }
            }


            [SerializeField]
            float _frequency;
            public float Frequency
            {
                get
                {
                    return _frequency;
                }
            }

            public ShakeParams(float amplitude, float frequency)
            {
                this._amplitude = amplitude;
                this._frequency = frequency;
            }
        }

        [SerializeField]
        ShakeParams _shakeParams = new ShakeParams(0f, 0f);

        CinemachineVirtualCamera _cineVirtualCam;
        CinemachineBasicMultiChannelPerlin _noise;


        // Start is called before the first frame update
        void Start()
        {
            _cineVirtualCam = GetComponent<CinemachineVirtualCamera>();
            if (_cineVirtualCam != null)
            {
                _noise = _cineVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            }
        }

        public void ShakeCamera(PlayerController player)
        {
            StartCoroutine(Noise(player.HurtDelay));
        }

        IEnumerator Noise(float delay)
        {
            if (_noise == null)
            {
                yield return null;
            }
            _noise.m_AmplitudeGain = _shakeParams.Amplitude;
            _noise.m_FrequencyGain = _shakeParams.Frequency;
            yield return new WaitForSeconds(delay);
            _noise.m_AmplitudeGain = 0;
            _noise.m_FrequencyGain = 0;
        }
    }
}