using System.Collections;
using UnityEngine;

public class TipController : MonoBehaviour
{
    [Tooltip("Сама панель, которую нужно скрыть")]
    public GameObject tipPanel;

    [Tooltip("Время в секундах до скрытия")]
    public float hideDelay = 5f;

    void Start()
    {
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);
        if (tipPanel != null)
            tipPanel.SetActive(false);
        else
            Debug.LogWarning("TipController: tipPanel не назначена!");
    }
}
