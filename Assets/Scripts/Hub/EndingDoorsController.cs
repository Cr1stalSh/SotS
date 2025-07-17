using UnityEngine;

public class EndingDoorsController : MonoBehaviour
{
    [Tooltip("Дверь, ведущая в Рай")]
    public GameObject heavenDoor;

    [Tooltip("Дверь, ведущая в Ад")]
    public GameObject hellDoor;

    [Tooltip("Сколько перезапусков не даёт доступ в Рай?")]
    public int heavenThreshold = 10;

    void Awake()
    {
        heavenDoor.SetActive(false);
        hellDoor.SetActive(false);
    }

    void Start()
    {
        int restarts = 0;
        if (GameManager.Instance != null)
            restarts = GameManager.Instance.GetLevelRestartCount();
        else
            Debug.LogWarning("[EndingDoorsController] GameManager отсутствует!");

        int completedLevels = GameManager.Instance.GetCompletedLevelCount();

        if (restarts < heavenThreshold && completedLevels>=3)
        {
            heavenDoor.SetActive(true);
            Debug.Log($"[EndingDoors] Рестартов={restarts} < {heavenThreshold} → включаем дверь в Рай");
        }
        else if (restarts >= heavenThreshold && completedLevels >= 3)
        {
            hellDoor.SetActive(true);
            Debug.Log($"[EndingDoors] Рестартов={restarts} ≥ {heavenThreshold} → включаем дверь в Ад");
        }
    }
}
