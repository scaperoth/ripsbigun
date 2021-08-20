
using UnityEngine;
using TMPro;


namespace RipsBigun
{
    [ExecuteInEditMode]
    public class ScoreController : MonoBehaviour
    {
        [SerializeField]
        FloatVariable _score;
        [SerializeField]
        TextMeshPro _scoreValue;

        private void Start()
        {
            _score.OnValueChanged.AddListener(UpdateScore);
        }

        private void OnDisable() { 
        
            _score.OnValueChanged.RemoveListener(UpdateScore);
        }

        private void OnDestroy()
        {
            _score.OnValueChanged.RemoveListener(UpdateScore);
        }

        public void UpdateScore(int score)
        {
            string padding = new string('0', _score.MaxValueDigits);
            _scoreValue.text = score.ToString(padding);
        }
    }

}