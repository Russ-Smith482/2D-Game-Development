using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    public int currentWave = 1;
    public int enemiesDestroyed = 0;
    public int enemiesRequired = 10; // enemies needed to reach next wave

    [SerializeField] private float wavePauseDuration = 2f;
    public bool isWavePaused { get; private set; } = false;

    private void Awake()
    {
        Instance = this;
    }

    public void EnemyDestroyed()
    {
        enemiesDestroyed++;

        if (enemiesDestroyed >= enemiesRequired)
        {
            StartCoroutine(StartNextWave());
        }
    }
    IEnumerator StartNextWave()
    {
        isWavePaused = true;

        // Notify UIManager to show wave complete
        UIManager.Instance.ShowWaveComplete(currentWave);

        yield return new WaitForSeconds(wavePauseDuration);

        currentWave++;
        enemiesDestroyed = 0;
        enemiesRequired += 5; // optional: scale difficulty

        isWavePaused = false;

        // Notify UIManager to update wave display
        UIManager.Instance.UpdateWave(currentWave);

        // Notify SpawnManager (adjust spawn rate)
        SpawnManager.Instance.OnWaveChanged(currentWave);
    }
}

