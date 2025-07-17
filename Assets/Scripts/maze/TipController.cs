using System.Collections;
using UnityEngine;

public class TipController : MonoBehaviour
{
    [Tooltip("���� ������, ������� ����� ������")]
    public GameObject tipPanel;

    [Tooltip("����� � �������� �� �������")]
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
            Debug.LogWarning("TipController: tipPanel �� ���������!");
    }
}
