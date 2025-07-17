using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class HubDoor : MonoBehaviour
{
    [Header("Настройки двери")]
    public LevelID levelID;
    [Tooltip("Точное имя сцены для загрузки")]
    public string sceneName;
    public Sprite openSprite;
    public Sprite closedSprite;

    private SpriteRenderer sr;
    private Collider2D col;
    private bool playerInZone;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        col.isTrigger = true;

        Debug.Log(
            $"[HubDoor Awake on '{gameObject.name}'] " +
            $"levelID = {levelID}, sceneName = '{sceneName}'"
        );
    }

    void Start()
    {
        UpdateDoorVisual();
    }

    void UpdateDoorVisual()
    {
        bool completed = GameManager.Instance.IsLevelCompleted(levelID);
        sr.sprite = completed ? closedSprite : openSprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInZone = true;

        if (!GameManager.Instance.IsLevelCompleted(levelID))
            UIManager.Instance.ShowHint("Нажмите H для взаимодействия");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInZone = false;
        UIManager.Instance.HideHint();
    }

    void Update()
    {
        if (playerInZone &&
            !GameManager.Instance.IsLevelCompleted(levelID) &&
            Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log(
                $"[HubDoor:{gameObject.name}] " +
                $"Loading scene '{sceneName}' for LevelID.{levelID}"
            );

            UIManager.Instance.HideHint();
            SceneManager.LoadScene(sceneName);
        }
    }
}
