using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HubLightController : MonoBehaviour
{
    [Tooltip("Какой уровень должен быть пройден, чтобы зажечь этот свет")]
    public LevelID levelID;

    [Tooltip("Интенсивность света, когда уровень пройден")]
    public float onIntensity = 100f;

    [Tooltip("Интенсивность света по умолчанию (выключено)")]
    public float offIntensity = 0f;

    private Light2D light2D;

    void Awake()
    {
        light2D = GetComponent<Light2D>();
        if (light2D == null)
            Debug.LogError("HubLightController: нет компонента Light2D на объекте!");
    }

    void Start()
    {
        UpdateLightState();
    }

    public void UpdateLightState()
    {
        bool completed = GameManager.Instance.IsLevelCompleted(levelID);
        light2D.intensity = completed ? onIntensity : offIntensity;
    }
}
