using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class EndGameController : MonoBehaviour
{
    [Tooltip("Текстовый объект с надписью 'Конец игры'")]
    public TMP_Text endGameText;  

    [Tooltip("Через сколько секунд вернуться в главное меню")]
    public float delayBeforeReturn = 5f;

    [Tooltip("Имя сцены главного меню")]
    public string mainMenuSceneName = "menu";

    void Start()
    {
        if (endGameText != null)
            endGameText.text = "Конец игры";

        StartCoroutine(ReturnToMainMenuAfterDelay());
    }

    private IEnumerator ReturnToMainMenuAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeReturn);

        if (GameManager.Instance != null)
            GameManager.Instance.ResetAllData();

        SceneManager.LoadScene(mainMenuSceneName);
    }
}
