using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Text _ammoText;
    [SerializeField]
    private Image _livesImg;
    [SerializeField]
    private Sprite[] _livesSprites;
    [SerializeField]
    private Text _gameOver;
    [SerializeField]
    private Text _restart;
    [SerializeField]
    public Text _recharge;
    [SerializeField]
    private Text _waveText;

    private GameManager _gameManager;

    [Header("Boost UI")]
    [SerializeField] private Slider _boostBar;
    [SerializeField] private Image _fillImage; // The "Fill" image inside the slider

    [Header("Colors")]
    [SerializeField] private Color _normalColor = Color.cyan;
    [SerializeField] private Color _fullColor = Color.green;

    [Header("Flash Settings")]
    [SerializeField] private float _flashDuration = 0.1f;
    [SerializeField] private int _flashCount = 4;

    private bool _isFlashing = false;
    private bool _flashRecharge = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {

        _scoreText.text = "Score: " + 0;
        _gameOver.gameObject.SetActive(false);
        _restart.gameObject.SetActive(false);
        _recharge.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("GameManager is NULL");
        }

    }
    public void UpdateWave(int waveNumber)
    {
        _waveText.text = $"Wave {waveNumber}";
    }
    public void ShowWaveComplete(int waveNumber)
    {
        _waveText.text = $"Wave {waveNumber} Complete!";
    }
    public void AddScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        if (currentLives < 0 || currentLives > _livesSprites.Length)
            return;

        _livesImg.sprite = _livesSprites[currentLives];

        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }

    public void UpdateAmmo(int currentAmmo)

    {
        _ammoText.text = "Wand Energy " + currentAmmo.ToString();
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOver.gameObject.SetActive(true);
        _restart.gameObject.SetActive(true);
        StartCoroutine(GameOverFlicker());
    }

    IEnumerator GameOverFlicker()
    {
        while (true)
        {
            _gameOver.text = "TRY AGAIN WITCH...";
            yield return new WaitForSeconds(0.5f);
            _gameOver.text = "";
            yield return new WaitForSeconds(0.5f);
        }

    }

    public void BoostUpdater(float _boostLimit)
    {
        // Clamp to slider range (just in case)
        _boostLimit = Mathf.Clamp(_boostLimit, _boostBar.minValue, _boostBar.maxValue);

        // Update slider value
        _boostBar.value = _boostLimit;

        // Calculate percentage fill
        float fillPercent = _boostBar.value / _boostBar.maxValue;

        // When full, change color and optionally flash
        if (fillPercent >= 0.995f)
        {
            _fillImage.color = _fullColor;

            if (!_isFlashing)
                StartCoroutine(FlashWhenFull());
        }
        else
        {
            _fillImage.color = _normalColor;
        }
    }

    private IEnumerator FlashWhenFull()
    {
        _isFlashing = true;

        for (int i = 0; i < _flashCount; i++)
        {
            _fillImage.color = Color.white;
            yield return new WaitForSeconds(_flashDuration);
            _fillImage.color = _fullColor;
            yield return new WaitForSeconds(_flashDuration);
        }

        _isFlashing = false;
    }

    public void RechargeWand()
    {
        _recharge.gameObject.SetActive(true);

        if (_flashRecharge == false)
        {
            StartCoroutine(Recharge());
        }
    }
    private IEnumerator Recharge()
    {
        _flashRecharge = true;

        for (int i = 0; i < _flashCount; i++)
        {
            _recharge.color = Color.white;
            yield return new WaitForSeconds(_flashDuration);
            _recharge.color = Color.red;
            yield return new WaitForSeconds(_flashDuration);
        }

        _flashRecharge = false;
    }
    public void RechargeText()
    {
        _recharge.gameObject.SetActive(false);
    }
}

