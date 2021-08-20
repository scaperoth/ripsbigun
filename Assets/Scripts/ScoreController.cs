
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
    }

}