using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour

{
    public static WaveManager Instance;

    public int currentWave = 1;
    public int enemiesDestroyed = 0;
    public int enemiesRequired = 10;
    [SerializeField] private float wavePauseDuration = 5f;
    public bool isWavePaused { get; private set; } = false;
    private int finalWave = 7;
    private void Awake()
    {
        Instance = this;
    }
    public void EnemyDestroyed()
    {
        enemiesDestroyed++;

        if (enemiesDestroyed >= enemiesRequired && !isWavePaused)
        {
            StartCoroutine(StartNextWave());
        }
    }
    IEnumerator StartNextWave()
    {
        isWavePaused = true;
        UIManager.Instance.ShowWaveComplete(currentWave);
        SpawnManager.Instance.PauseSpawning();
        yield return new WaitForSeconds(wavePauseDuration);
        currentWave++;
        if (currentWave == finalWave)
        {
            UIManager.Instance.ShowBossWarning();
            yield return new WaitForSeconds(2.5f);
            UIManager.Instance.ShowFinalWaveStart();
        }
        else if (currentWave > finalWave)
        {
            UIManager.Instance.ShowFinalWaveComplete();
            SpawnManager.Instance.StopAllSpawning();
            yield break;
        }
        else
        {
            UIManager.Instance.UpdateWave(currentWave);
        }
        enemiesDestroyed = 0;
        enemiesRequired += 6;
        SpawnManager.Instance.ResumeSpawning(currentWave);

        isWavePaused = false;
    }
}


