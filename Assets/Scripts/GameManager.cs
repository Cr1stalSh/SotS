using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public enum Ability
{
    Freeze,
    Repel,
    Ghost
}
public enum LevelID
{
    Maze,
    Pattern,
    Dialogue,
    Ending
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private HashSet<Ability> unlockedAbilities = new HashSet<Ability>();
    private HashSet<LevelID> completedLevels = new HashSet<LevelID>();

    private int levelRestartCount = 0;

    public void NotifyLevelRestart()
    {
        levelRestartCount++;
        Debug.Log($"[GameManager] Уровни перезапущены {levelRestartCount} раз(а).");
    }

    public int GetLevelRestartCount()
    {
        return levelRestartCount;
    }

    public event Action<Ability> OnAbilityUsed;

    public event Action<Ability> OnAbilityUnlocked;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void UnlockAbility(Ability ability)
    {
        if (unlockedAbilities.Add(ability))
        {
            Debug.Log($"Разблокирована способность: {ability}");
            OnAbilityUnlocked?.Invoke(ability);
        }
    }

    public bool IsAbilityUnlocked(Ability ability) =>
        unlockedAbilities.Contains(ability);

    public void CompleteLevel(LevelID level)
    {
        if (completedLevels.Add(level))
            Debug.Log($"Уровень пройден: {level}");
    }

    public bool IsLevelCompleted(LevelID level) =>
        completedLevels.Contains(level);

    public void NotifyAbilityUsed(Ability ability)
    {
        OnAbilityUsed?.Invoke(ability);
    }

    public int GetCompletedLevelCount()
    {
        return completedLevels.Count;
    }

    public void ResetAllData()
    {
        unlockedAbilities.Clear();
        completedLevels.Clear();
        levelRestartCount = 0;
        Debug.Log("[GameManager] Все данные сброшены.");
    }

}
