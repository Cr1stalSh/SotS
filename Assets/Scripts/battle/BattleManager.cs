using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [Header("��������� ������")]
    public GameObject playerPrefab;

    [Header("��������� ����� (������)")]
    public GameObject shadowPrefab;
    public int totalShadows = 20;
    public float spawnRadiusMin = 2f;
    public float spawnRadiusMax = 5f;

    [Header("��������� ������")]
    public int initialSpawnCount = 5;
    public int spawnBatchCount = 3;
    public float spawnDelay = 3f;

    private bool allSpawned = false;
    private bool battleFinished = false;

    void Start()
    {
        AdjustShadowCountByCompletedLevels();
        SpawnPlayer();
        StartCoroutine(SpawnShadowsGradually());
    }

    void AdjustShadowCountByCompletedLevels()
    {
        if (GameManager.Instance != null)
        {
            int completed = GameManager.Instance.GetCompletedLevelCount();
           
            totalShadows = Mathf.Max(1, completed) * 25;
            Debug.Log($"[BattleManager] �������� �������: {completed}, ����� ������: {totalShadows}");
        }
        else
        {
            Debug.LogWarning("[BattleManager] GameManager �� ������, totalShadows ������� = " + totalShadows);
        }
    }

    void Update()
    {
        // ��� ������ ��� ����� ������������ � ���������� � ��������� � ���
        if (allSpawned && !battleFinished)
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 0)
            {
                battleFinished = true;  
                LoadHub();
            }
        }
    }

    void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab �� ��������!");
            return;
        }
        var playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        playerObj.tag = "Player";
    }

    IEnumerator SpawnShadowsGradually()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError("����� �� ������ �� �����!");
            yield break;
        }

        int spawnedCount = 0;
        int attempts = 0;

        // ������ ������
        while (spawnedCount < initialSpawnCount && spawnedCount < totalShadows && attempts < 1000000)
        {
            attempts++;
            var spawnPos = GenerateSpawnPosition(playerObj.transform.position);
            Instantiate(shadowPrefab, spawnPos, Quaternion.identity);
            spawnedCount++;
        }

        // ��������� ������
        while (spawnedCount < totalShadows)
        {
            yield return new WaitForSeconds(spawnDelay);
            int batchCount = 0;
            attempts = 0;
            while (batchCount < spawnBatchCount && spawnedCount < totalShadows && attempts < 10000)
            {
                attempts++;
                var spawnPos = GenerateSpawnPosition(playerObj.transform.position);
                Instantiate(shadowPrefab, spawnPos, Quaternion.identity);
                spawnedCount++;
                batchCount++;
            }
        }

        allSpawned = true;
    }

    Vector2 GenerateSpawnPosition(Vector2 playerPos)
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        float distance = Random.Range(spawnRadiusMin, spawnRadiusMax);
        return playerPos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
    }

    void LoadHub()
    {
       SceneManager.LoadScene("hub"); 
    }
}
