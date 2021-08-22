
using UnityEngine;
using TMPro;


namespace RipsBigun
{
    [ExecuteInEditMode]
    public class ScoreController : MonoBehaviour
    {
        [SerializeField]
        TextMeshPro _scoreValue;

        public void UpdateScore(FloatVariable score)
        {
            string padding = new string('0', score.MaxValueDigits);
            _scoreValue.text = score.Value.ToString(padding);
        }
        public void UpdateScoreText(FloatVariable score, bool withPadding)
        {
            if (withPadding)
            {
                UpdateScore(score);
            }
            else
            {
                _scoreValue.text = score.Value.ToString();
            }
        }
    }

}