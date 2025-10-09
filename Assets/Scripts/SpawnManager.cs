using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;

    private bool _stopSpawning = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    IEnumerator SpawnRoutine()
    {
        while (_stopSpawning == false)
        {
            float randomX = Random.Range(-8, 8f);
            GameObject newEnemy = Instantiate(_enemyPrefab, transform.position = new Vector3(randomX, 6f, 0), Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5);
        }


    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

}
