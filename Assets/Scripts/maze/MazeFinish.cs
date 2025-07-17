using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MazeFinish : MonoBehaviour
{
    private LevelTimer levelTimer;

    void Start()
    {
        levelTimer = FindObjectOfType<LevelTimer>();
        if (levelTimer == null)
        {
            Debug.LogWarning("LevelTimer не найден на сцене!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (levelTimer != null)
                levelTimer.StopTimer();

            GameManager.Instance.UnlockAbility(Ability.Freeze);
            GameManager.Instance.CompleteLevel(LevelID.Maze);
            SceneManager.LoadScene("battle");
        }
    }
}
