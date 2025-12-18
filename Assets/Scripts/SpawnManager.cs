using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [Header("Enemy Spawns")]
    [SerializeField] private GameObject[] frequentEnemies;
    [SerializeField] private GameObject[] standardEnemies;
    [SerializeField] private GameObject[] rareEnemies;

    [SerializeField]
    private GameObject _boss;

    [Header("Power-Up Spawns")]
    [SerializeField] private GameObject[] frequentPowerUps;
    [SerializeField] private GameObject[] standardPowerUps;
    [SerializeField] private GameObject[] rarePowerUps;

    private bool spawningPaused = false;
    private bool stopAllSpawning = false;

    private float enemyMinDelay = 3f;
    private float enemyMaxDelay = 5f;

    private Coroutine enemyRoutine;
    private Coroutine powerRoutine;

    private bool bossSpawned = false;

    private void Awake()
    {
        Instance = this;
    }

    public void StartSpawning()
    {
        enemyRoutine = StartCoroutine(SpawnEnemies());
        powerRoutine = StartCoroutine(SpawnPowerUps());
    }

    public void PauseSpawning()
    {
        spawningPaused = true;
    }

    public void ResumeSpawning(int wave)
    {
        spawningPaused = false;

        enemyMinDelay = Mathf.Max(1f, 3f - (wave * 0.25f));
        enemyMaxDelay = Mathf.Max(2f, 5f - (wave * 0.2f));
    }

    public void StopAllSpawning()
    {
        stopAllSpawning = true;
    }
    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(2f);

        while (!stopAllSpawning)
        {
            if (spawningPaused)
            {
                yield return null;
                continue;
            }

            int wave = WaveManager.Instance.currentWave;

            if (wave == 7)
            {
                if (!bossSpawned)
                {
                    bossSpawned = true;

                    float y = 0f;
                    Instantiate(_boss, new Vector3(10f, y, 0), Quaternion.identity);
                }

                yield return null;
                continue;
            }

            float yPos = Random.Range(-3f, 3.5f);
            int roll = Random.Range(0, 100);

            GameObject prefab = null;

            if (roll < 50)
            {
                prefab = frequentEnemies[Random.Range(0, frequentEnemies.Length)];
            }
            else if (roll < 85 && wave >= 2)
            {
                prefab = standardEnemies[Random.Range(0, standardEnemies.Length)];
            }
            else if (wave >= 5)
            {
                prefab = rareEnemies[Random.Range(0, rareEnemies.Length)];
            }
            else
            {
                prefab = frequentEnemies[Random.Range(0, frequentEnemies.Length)];
            }

            Instantiate(prefab, new Vector3(10f, yPos, 0), Quaternion.identity);

            yield return new WaitForSeconds(Random.Range(enemyMinDelay, enemyMaxDelay));
        }
    }

    IEnumerator SpawnPowerUps()
    {
        yield return new WaitForSeconds(4f);

        while (!stopAllSpawning)
        {
            if (spawningPaused)
            {
                yield return null;
                continue;
            }

            float y = Random.Range(-3f, 3.5f);
            int roll = Random.Range(0, 100);

            GameObject prefab = null;

            int wave = WaveManager.Instance.currentWave;

            if (roll < 45)
            {
                prefab = frequentPowerUps[Random.Range(0, frequentPowerUps.Length)];
            }
            else if (roll < 80)
            {
                prefab = standardPowerUps[Random.Range(0, standardPowerUps.Length)];
            }
            else if (wave >= 2)
            {
                prefab = rarePowerUps[Random.Range(0, rarePowerUps.Length)];
            }
            else
            {
                prefab = frequentPowerUps[Random.Range(0, frequentPowerUps.Length)];
            }

            Instantiate(prefab, new Vector3(10f, y, 0), Quaternion.identity);

            yield return new WaitForSeconds(Random.Range(5f, 12f));
        }
    }
    public void OnPlayerDeath()
    {
        stopAllSpawning = true;
    }
}
