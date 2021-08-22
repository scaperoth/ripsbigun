using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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

        bool _interactable = false;

        private void Update()
        {
            if (_interactable && Input.GetKey("escape"))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                return;
            }

            if (_interactable && AnyButtonPressed())
            {
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
            _scoreController.UpdateScore(_score, false);
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