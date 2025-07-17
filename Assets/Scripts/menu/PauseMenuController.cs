using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    private CanvasGroup cg;
    private bool isPaused = false;
    [Tooltip("Имя сцены главного меню")]
    public string mainMenuSceneName = "menu";

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        Hide(); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;      
        cg.alpha = 1f;            
        cg.interactable = true;   
        cg.blocksRaycasts = true; 
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;      
        Hide();
    }

    void Hide()
    {
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    public void OnBackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
