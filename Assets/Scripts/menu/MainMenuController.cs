using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Tooltip("Имя сцены хаба")]
    public string firstSceneName = "hub";

    public void OnStartGame()
    {
        SceneManager.LoadScene(firstSceneName);
    }

    public void OnQuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
