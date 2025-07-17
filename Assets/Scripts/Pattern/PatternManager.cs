using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PatternManager : MonoBehaviour
{
    [Header("Настройки игрока")]
    public GameObject playerPrefab; 

    [Header("Настройки теней")]
    public GameObject shadowPrefab; 
    public int numberOfShadows = 15; 
    public float spawnRadiusMin = 2f;    
    public float spawnRadiusMax = 5f;    
    public float minDistanceBetweenShadows = 1f; 

    [Header("Логика уровня")]
    private int[] sequenceLengths = { 3, 4, 5 };
    private int currentIteration = 0;
    private List<Shadow> sequence;
    private int inputIndex = 0;
    private bool acceptingInput = false;
    
    private List<Shadow> shadows = new List<Shadow>();
    public List<Shadow> ShadowsOff;

    void Start()
    {
        SpawnPlayer();
        SpawnShadows();
        StartCoroutine(WaitForEToStart());
    }

    IEnumerator WaitForEToStart()
    {
        Debug.Log("Нажмите E, чтобы начать подсветку теней.");
        while (!Input.GetKeyDown(KeyCode.E))
        {
            yield return null;
        }
        StartCoroutine(RunIteration());
    }

    void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab не назначен!");
            return;
        }

        GameObject playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        playerObj.tag = "Player";
    }

    void SpawnShadows()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError("Игрок не найден на сцене!");
            return;
        }
        Vector2 playerPos = playerObj.transform.position;
        int spawnedCount = 0;
        int attempts = 0; 
        List<Vector2> spawnedPositions = new List<Vector2>();

        while (spawnedCount < numberOfShadows && attempts < 10000)
        {
            attempts++;

            float angle = Random.Range(0f, Mathf.PI * 2);
            float distance = Random.Range(spawnRadiusMin, spawnRadiusMax);
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
            Vector2 spawnPos = playerPos + offset;

            bool validPosition = true;
            foreach (Vector2 pos in spawnedPositions)
            {
                if (Vector2.Distance(pos, spawnPos) < minDistanceBetweenShadows)
                {
                    validPosition = false;
                    break;
                }
            }
            if (!validPosition)
                continue;

            GameObject shadowObj = Instantiate(shadowPrefab, spawnPos, Quaternion.identity);
            Shadow shadowComp = shadowObj.GetComponent<Shadow>();
            if (shadowComp != null)
            {
                shadows.Add(shadowComp);
            }
            spawnedPositions.Add(spawnPos);
            spawnedCount++;
        }

        if (spawnedCount < numberOfShadows)
        {
            Debug.LogWarning("Не удалось создать все тени. Проверьте параметры спавна.");
        }
    }

    IEnumerator RunIteration()
    {
        yield return new WaitForSeconds(1f);

        foreach (Shadow sh in shadows)
        {
            sh.TurnOff();
            yield return null;
            yield return null;
        }

        Debug.Log("Запускаем итерацию...");

        acceptingInput = false;
        inputIndex = 0;

        if (currentIteration >= sequenceLengths.Length)
        {
            Debug.Log("Уровень пройден!");
            yield break;
        }

        int sequenceCount = sequenceLengths[currentIteration];
        sequence = new List<Shadow>();
        ShadowsOff = new List<Shadow>();

        HashSet<Shadow> usedShadows = new HashSet<Shadow>(); // Хранит уже выбранные тени

        while (sequence.Count < sequenceCount)
        {
            Shadow selected = shadows[Random.Range(0, shadows.Count)];

            if (!usedShadows.Contains(selected)) 
            {
                sequence.Add(selected);
                usedShadows.Add(selected); // Добавляем в HashSet, чтобы избежать дубликатов
            }
        }

        foreach (Shadow s in sequence)
        {
            s.LightUp();
            yield return new WaitForSeconds(0.5f);
            s.TurnOff();
            yield return new WaitForSeconds(0.3f);
        }

        acceptingInput = true;
    }

    public void PlayerInput(Shadow selectedShadow)
    {
        if (!acceptingInput)
            return;

        if (selectedShadow == sequence[inputIndex])
        {
            inputIndex++;
            if (inputIndex >= sequence.Count)
            {
                acceptingInput = false;
                currentIteration++;
                if (currentIteration < sequenceLengths.Length)
                {
                    foreach (Shadow sh in shadows)
                    {
                        sh.Correct();
                    }
                    StartCoroutine(RunIteration());
                }
                else
                {
                    foreach (Shadow sh in shadows)
                    {
                        sh.Correct();
                    }
                    Debug.Log("Уровень пройден!");
                    
                    GameManager.Instance.UnlockAbility(Ability.Repel);
                    GameManager.Instance.CompleteLevel(LevelID.Pattern);
                    SceneManager.LoadScene("battle");
                }
            }
        }
        else
        {
            Debug.Log("Неправильная последовательность!");
            GameManager.Instance.NotifyLevelRestart();
            selectedShadow.Mistake();
            StartCoroutine(RunIteration());
        }
    }
}
