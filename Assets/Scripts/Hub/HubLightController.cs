using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HubLightController : MonoBehaviour
{
    [Tooltip("����� ������� ������ ���� �������, ����� ������ ���� ����")]
    public LevelID levelID;

    [Tooltip("������������� �����, ����� ������� �������")]
    public float onIntensity = 100f;

    [Tooltip("������������� ����� �� ��������� (���������)")]
    public float offIntensity = 0f;

    private Light2D light2D;

    void Awake()
    {
        light2D = GetComponent<Light2D>();
        if (light2D == null)
            Debug.LogError("HubLightController: ��� ���������� Light2D �� �������!");
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
