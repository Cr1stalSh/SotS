using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Tooltip("Текст подсказки взаимодействия")]
    public GameObject interactHint;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowHint(string message)
    {
        interactHint.SetActive(true);
        var txt = interactHint.GetComponent<Text>();
        if (txt != null) txt.text = message;
    }

    public void HideHint()
    {
        interactHint.SetActive(false);
    }
}
