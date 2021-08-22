using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RipsBigun
{
    public class GameOverOverlay : MonoBehaviour
    {
        [SerializeField]
        SpriteRenderer _spriteRenderer;
        [SerializeField]
        float _fadeSpeed = 5f;
        [SerializeField]
        FloatVariable _score;
        [SerializeField]
        ScoreController _scoreController;
        [SerializeField]
        string _mainMenuScene = "Home";

        bool _interactable = false;

        private void Update()
        {
            if (_interactable && Input.GetKey(KeyCode.Escape))
            {
                SceneManager.LoadScene(_mainMenuScene);
                return;
            }
            if (_interactable && AnyButtonPressed())
            {
                _score.UpdateValue(_score.InitialValue);
                Scene activeScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(activeScene.name);
            }
        }

        bool AnyButtonPressed()
        {
            for (int i = 0; i < 20; i++)
            {
                if (Input.GetKeyDown("joystick 1 button " + i))
                {
                    return true;
                }
            }

            return Input.anyKeyDown;
        }

        public void FadeToBlack()
        {
            _scoreController.UpdateScoreText(_score, false);
            StartCoroutine(FadeRoutine(true));
        }

        IEnumerator FadeRoutine(bool fadeToBlack)
        {
            Color spriteColor = _spriteRenderer.color;
            float fadeAmount;

            if (fadeToBlack)
            {
                while (spriteColor.a < 1)
                {
                    fadeAmount = spriteColor.a + (_fadeSpeed * Time.deltaTime);
                    spriteColor = new Color(spriteColor.r, spriteColor.g, spriteColor.b, fadeAmount);
                    _spriteRenderer.color = spriteColor;
                    yield return null;
                }
            }
            else
            {

                while (spriteColor.a > 1)
                {
                    fadeAmount = spriteColor.a - (_fadeSpeed * Time.deltaTime);
                    spriteColor = new Color(spriteColor.r, spriteColor.g, spriteColor.b, fadeAmount);
                    _spriteRenderer.color = spriteColor;
                    yield return null;
                }
            }

            yield return new WaitForSeconds(2);
            _interactable = true;
        }
    }
}