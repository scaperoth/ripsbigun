
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RipsBigun
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField]
        string _startSceneName = "Gameplay";

        private void Update()
        {
            if (Input.GetKey(KeyCode.Return))
            {
                SceneManager.LoadScene(_startSceneName);
            }

            if (Input.GetKey(KeyCode.Escape))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
                return;
            }
        }
    }
}
