using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    [Tooltip("В секундах")]
    public float timeRemaining = 180f;
    public TextMeshProUGUI timerText;  

    private bool timerIsRunning = false;

    void Start()
    {
        timerIsRunning = true;
    }

    void Update()
    {
        if (!timerIsRunning) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            DisplayTime(timeRemaining);
        }
        else
        {
            timerIsRunning = false;
            RestartLevel();
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay = Mathf.Max(0, timeToDisplay);
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        
        timerText.text = string.Format("{0}:{1:00}", minutes, seconds);
    }

    public void StopTimer()
    {
        timerIsRunning = false;
    }

    void RestartLevel()
    {
        GameManager.Instance.NotifyLevelRestart();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
