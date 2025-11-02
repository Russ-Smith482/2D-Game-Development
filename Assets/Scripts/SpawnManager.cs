using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField] 
    private GameObject[] _potions;
    [SerializeField]
    private GameObject _enemyContainer;
    

    private bool _stopSpawning = false;

    // Start is called before the first frame update
    void Start()
    {
       
    }


    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPotionRoutine());
    }
    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (_stopSpawning == false)
        {
            float randomYSpawn = Random.Range(-4f, 4.2f);
            Vector3 spawnPosition = new Vector3(10f, randomYSpawn, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(Random.Range(3, 6));
        }
    }

    IEnumerator SpawnPotionRoutine()
    {
        yield return new WaitForSeconds(6f);
        while (_stopSpawning == false)
        {
            float randomYSpawnPos = Random.Range(-4f, 4.2f);
            int randomPotion = Random.Range(0, 3);
            GameObject newPotion = Instantiate(_potions[randomPotion], new Vector3(10f, randomYSpawnPos, 0), Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(5, 9));
        }
            
    }
    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

}
