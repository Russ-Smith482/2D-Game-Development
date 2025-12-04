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
        // Show wave complete text
        UIManager.Instance.ShowWaveComplete(currentWave);
        // Stop all spawning temporarily
        SpawnManager.Instance.PauseSpawning();
        // Empty quiet moment
        yield return new WaitForSeconds(wavePauseDuration);
        // Move to next wave
        currentWave++;
        // -----------------------------
        // FINAL WAVE LOGIC
        // -----------------------------
        if (currentWave == finalWave)
        {
            // Dramatic warning before final wave
            UIManager.Instance.ShowBossWarning();
            yield return new WaitForSeconds(2.5f);
            // Show FINAL WAVE UI
            UIManager.Instance.ShowFinalWaveStart();
        }
        else if (currentWave > finalWave)
        {
            // Player wins!
            UIManager.Instance.ShowFinalWaveComplete();
            SpawnManager.Instance.StopAllSpawning();
            yield break;
        }
        else
        {
            // Normal wave start
            UIManager.Instance.UpdateWave(currentWave);
        }

        // Reset counters for new wave
        enemiesDestroyed = 0;
        enemiesRequired += 6;
        // Resume spawning for the new wave
        SpawnManager.Instance.ResumeSpawning(currentWave);

        isWavePaused = false;
    }
}


