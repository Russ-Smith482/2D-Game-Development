using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _frequentEnemySpawn;
    [SerializeField]
    private GameObject[] _standardEnemySpawn;
    [SerializeField]
    private GameObject[] _rareEnemySpawn;
    [SerializeField]
    private GameObject[] _frequentPowerUp;
    [SerializeField]
    private GameObject[] _standardPowerUp;
    [SerializeField]
    private GameObject[] _rarePowerUp;
    
  
    [SerializeField]
    private GameObject _enemyContainer;

    private bool _stopSpawning = false;

    [SerializeField] private float _minSpawnDelay = 3f;
    [SerializeField] private float _maxSpawnDelay = 6f;

    public static SpawnManager Instance;

    private void Awake()
    {
        Instance = this;

    }
    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemies());
        StartCoroutine(SpawnPowerUps());
    }
    
    IEnumerator SpawnPowerUps()
    {
        yield return new WaitForSeconds(7f);

        while (!_stopSpawning)
        {
            float randomY = Random.Range(-2.9f, 3.5f);

            int roll = Random.Range(0, 100);

            GameObject prefabToSpawn = null;

            if (roll < 50)
            {
                prefabToSpawn = _frequentPowerUp[Random.Range(0, _frequentPowerUp.Length)];
            }
            else if (roll < 90)
            {
                prefabToSpawn = _standardPowerUp[Random.Range(0, _standardPowerUp.Length)];
            }
            else 
            {
                prefabToSpawn = _rarePowerUp[Random.Range(0, _rarePowerUp.Length)];
            }

            Instantiate(prefabToSpawn, new Vector3(10f, randomY, 0), Quaternion.identity);

            // Balanced interval
            yield return new WaitForSeconds(Random.Range(7f, 18f));
        }
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(4f);

        while (!_stopSpawning)
        {
            float randomY = Random.Range(-2.9f, 3.5f);


            int roll = Random.Range(0, 100);

            GameObject prefabToSpawn = null;

            if (roll < 55)
            {
                prefabToSpawn = _frequentEnemySpawn[Random.Range(0, _frequentEnemySpawn.Length)];
            }
            else if (roll < 85)
            {
                prefabToSpawn = _standardEnemySpawn[Random.Range(0, _standardEnemySpawn.Length)];
            }
            else
            {
                prefabToSpawn = _rareEnemySpawn[Random.Range(0, _rareEnemySpawn.Length)];
            }

            Instantiate(prefabToSpawn, new Vector3(10f, randomY, 0), Quaternion.identity);

            // Balanced interval
            yield return new WaitForSeconds(Random.Range(3f, 5f));
        }
    }
    public void OnWaveChanged(int wave)
    {
        // Example difficulty scaling:
        _minSpawnDelay = Mathf.Max(1f, _minSpawnDelay - 0.2f);
        _maxSpawnDelay = Mathf.Max(2f, _maxSpawnDelay - 0.2f);

        //Debug.Log("Spawn rate increased for wave " + wave);
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
