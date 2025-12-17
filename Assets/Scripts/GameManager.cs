using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool _isGameOver;

    [SerializeField] private UIManager _uiManager;

    [Header("Victory Effects")]
    [SerializeField] private GameObject fireworkPrefab;
    [SerializeField] private float fireworkDuration = 1f;
    [SerializeField] private float fireworkSpawnRate = 0.1f;

    private void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL");
        }
    }
    private void Update()
    {
        if (_isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    // Called when PLAYER dies
    public void GameOver()
    {
        _isGameOver = true;
    }

    // 🔥 Called when BOSS dies
    public void BossDefeated()
    {
        if (_isGameOver) return;

        _isGameOver = true;
        _uiManager.ShowFinalWaveComplete();
        StartCoroutine(FireworkShow());
    }

    private IEnumerator FireworkShow()
    {
        float timer = 0f;

        while (timer < fireworkDuration)
        {
            Vector2 spawnPos = new Vector2(
                Random.Range(-7.5f, 7.5f),
                Random.Range(-3.5f, 3.5f)
            );

            Instantiate(fireworkPrefab, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(fireworkSpawnRate);
            timer += fireworkSpawnRate;
        }

        Debug.Log("Victory! Press R to Restart");
    }
}
