using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class EndGameController : MonoBehaviour
{
    [Tooltip("��������� ������ � �������� '����� ����'")]
    public TMP_Text endGameText;  

    [Tooltip("����� ������� ������ ��������� � ������� ����")]
    public float delayBeforeReturn = 5f;

    [Tooltip("��� ����� �������� ����")]
    public string mainMenuSceneName = "menu";

    void Start()
    {
        if (endGameText != null)
            endGameText.text = "����� ����";

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
