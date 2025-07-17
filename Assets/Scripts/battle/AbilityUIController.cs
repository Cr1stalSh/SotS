using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class AbilityUIController : MonoBehaviour
{
    [Header("HP")]
    public TMP_Text healthText;

    [Header("Icon Images (Image Type = Filled)")]
    public Image iconFreeze;
    public Image iconRepel;
    public Image iconGhost;

    [Header("Key Labels (TextMeshPro)")]
    public TMP_Text labelFreeze;    
    public TMP_Text labelRepel;    
    public TMP_Text labelGhost;     

    [Header("Recharge Times (сек)")]
    public float freezeRecharge = 7f;
    public float repelRecharge = 5f;
    public float ghostRecharge = 10f;

    void Start()
    {
        // Скрываем или показываем иконки и их подписи
        UpdateUnlockedUI(Ability.Freeze, iconFreeze, labelFreeze);
        UpdateUnlockedUI(Ability.Repel, iconRepel, labelRepel);
        UpdateUnlockedUI(Ability.Ghost, iconGhost, labelGhost);

        PlayerBattle.OnHealthChanged += UpdateHealthDisplay;
        
        UpdateHealthDisplay(PlayerBattle.CurrentHealth, PlayerBattle.MaxHealth);
    }

    void OnEnable()
    {
        GameManager.Instance.OnAbilityUsed += HandleAbilityUsed;
        GameManager.Instance.OnAbilityUnlocked += HandleAbilityUnlocked;
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnAbilityUsed -= HandleAbilityUsed;
            GameManager.Instance.OnAbilityUnlocked -= HandleAbilityUnlocked;
        }
        PlayerBattle.OnHealthChanged -= UpdateHealthDisplay;
    }

    private void HandleAbilityUnlocked(Ability ability)
    {
        var pair = GetIconAndLabel(ability);
        Image icon = pair.icon;
        TMP_Text label = pair.label;

        if (icon != null) icon.fillAmount = 1f;
        if (label != null) label.gameObject.SetActive(true);
    }


    private void HandleAbilityUsed(Ability ability)
    {
        // Запускаем корутину перезарядки
        switch (ability)
        {
            case Ability.Freeze:
                StartCoroutine(RechargeIcon(iconFreeze, freezeRecharge));
                break;
            case Ability.Repel:
                StartCoroutine(RechargeIcon(iconRepel, repelRecharge));
                break;
            case Ability.Ghost:
                StartCoroutine(RechargeIcon(iconGhost, ghostRecharge));
                break;
        }
    }

    private IEnumerator RechargeIcon(Image icon, float rechargeTime)
    {
        float elapsed = 0f;
        icon.fillAmount = 0f;
        while (elapsed < rechargeTime)
        {
            elapsed += Time.deltaTime;
            icon.fillAmount = Mathf.Clamp01(elapsed / rechargeTime);
            yield return null;
        }
        icon.fillAmount = 1f;
    }

    private void UpdateUnlockedUI(Ability ability, Image icon, TMP_Text label)
    {
        bool unlocked = GameManager.Instance.IsAbilityUnlocked(ability);
       
        icon.gameObject.SetActive(true);
        icon.fillAmount = unlocked ? 1f : 0f;
        
        label.gameObject.SetActive(unlocked);
    }

    private (Image icon, TMP_Text label) GetIconAndLabel(Ability ability)
    {
        return ability switch
        {
            Ability.Freeze => (iconFreeze, labelFreeze),
            Ability.Repel => (iconRepel, labelRepel),
            Ability.Ghost => (iconGhost, labelGhost),
            _ => (null, null)
        };
    }

    private void UpdateHealthDisplay(int current, int max)
    {
        healthText.text = $"HP: {current} / {max}";
    }
}
