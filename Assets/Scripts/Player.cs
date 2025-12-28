using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private AudioSource _audioSource;

    public float _speed = 3.5f;
    [SerializeField]
    private float _speedMultiplied = 2f;
    private float _boostedSpeed = 5.5f;
    private float _normalSpeed = 3.5f;
    [SerializeField]
    private float _boost = 100f;
    [SerializeField]
    private float _maxBoost = 100f;
    [SerializeField]
    private float _boostUsage = 25f;
    [SerializeField]
    private float _boostRecharge = 10f;
    [SerializeField]
    private float _cooldownTime = 3f;

    private bool _isBoosted = false;
    private bool _isRecharging = false;

    [SerializeField]
    private GameObject _zapPrefab;
    [SerializeField]
    private GameObject _tripleZap;
    [SerializeField]
    private GameObject _megaZap;
    [SerializeField]
    private GameObject _homingZapPrefab;
    private int _homingShotsRemaining = 0;

    [SerializeField]
    private float _fireRate = 0.50f;
    private float _canFire = -1f;
    [SerializeField]
    private int _ammoCount = 15;

    [SerializeField]
    private GameObject _firstHit;
    [SerializeField]
    private GameObject _SecondHit;

    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private int _score;

    [SerializeField]
    private bool _tripleZapActive = false;
    [SerializeField]
    private bool _megaZapActive = false;
    [SerializeField]
    private bool _homingZapActive = false;
    [SerializeField]
    private bool _shieldActive = false;
    [SerializeField]
    private GameObject _shieldFull;
    [SerializeField]
    private GameObject _shieldMid;
    [SerializeField]
    private GameObject _shieldLow;
    [SerializeField]
    private int _shieldLevel = 3;

    [SerializeField] private float magnetRadius = 4f;
    [SerializeField] private float magnetPullSpeed = 3f;
    private Transform magnetTarget = null;

    [SerializeField] private PlayerAudio _playerAudio;
    [SerializeField]
    private CameraShake _cameraShake;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(-4, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL");
        }

        if (_cameraShake == null)
        {
            Debug.LogError("CameraShake is NULL");
        }

        if (_audioSource == null)
        {
            Debug.LogError("Audio Source on the player is NULL");
        }
    }

    void Update()
    {
        Movement();
        SpeedBooster();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireWand();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            TryStartMagnetPull();
        }
        MagnetPullUpdate();
    }
    void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(direction * _speed * Time.deltaTime);

        if (transform.position.x >= -2f)
        {
            transform.position = new Vector3(-2f, transform.position.y, 0);

        }
        else if (transform.position.x <= -8f)
        {
            transform.position = new Vector3(-8f, transform.position.y, 0);
        }

        if (transform.position.y >= 3.5f)
        {
            transform.position = new Vector3(transform.position.x, 3.5f, 0);
        }
        else if (transform.position.y <= -3f)
        {
            transform.position = new Vector3(transform.position.x, -3f, 0);
        }
    }
    void SpeedBooster()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !_isBoosted && _boost > 0 && !_isRecharging)

        {
            StartCoroutine(BoostActive());
        }
    }
    IEnumerator BoostActive()
    {
        _isBoosted = true;
        _speed = _boostedSpeed;

        while (Input.GetKey(KeyCode.LeftShift) && _boost > 0)
        {
            _boost -= _boostUsage * Time.deltaTime;
            _boost = Mathf.Max(_boost, 0f);
            _uiManager.BoostUpdater(_boost);
            yield return null;
        }

        _isBoosted = false;
        _speed = _normalSpeed;

        if (!_isRecharging)
            StartCoroutine(BoostRecharge());
    }
    IEnumerator BoostRecharge()
    {
        _isRecharging = true;

        yield return new WaitForSeconds(_cooldownTime);

        while (_boost < _maxBoost && !_isBoosted)
        {
            _boost += _boostRecharge * Time.deltaTime;
            _boost = Mathf.Min(_boost, _maxBoost);
            _uiManager.BoostUpdater(_boost);
            yield return null;
        }
        _isRecharging = false;
    }
    private void FireWand()
    {
        _canFire = Time.time + _fireRate;

        if (_homingZapActive && _homingShotsRemaining > 0)
        {
            Instantiate(_homingZapPrefab, transform.position, Quaternion.identity);
            _homingShotsRemaining--;

            if (_homingShotsRemaining <= 0)
                _homingZapActive = false;
            _playerAudio.PlayZap();
            return;
        }
        if (_tripleZapActive == true)
        {
            Instantiate(_tripleZap, transform.position, Quaternion.identity);
        }
        else if (_megaZapActive == true)
        {
            Instantiate(_megaZap, transform.position, Quaternion.identity);
        }
        else
        {
            RegularZap();
        }
        _playerAudio.PlayZap();
    }
    void RegularZap()
    {
        if (_ammoCount >= 1)
        {
            Instantiate(_zapPrefab, transform.position + new Vector3(0.8f, 0, 0), Quaternion.identity);
            _ammoCount -= 1;
            CalculateAmmo(_ammoCount);
        }
        else if (_ammoCount == 0)
        {
            _uiManager.RechargeWand();
        }
    }
    void TryStartMagnetPull()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, magnetRadius);

        float closestDist = Mathf.Infinity;
        Transform closestPowerup = null;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Powerup"))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestPowerup = hit.transform;
                }
            }
        }
        if (closestPowerup != null)
        {
            magnetTarget = closestPowerup;
        }
    }
    void MagnetPullUpdate()
    {
        if (magnetTarget == null)
            return;

        magnetTarget.position = Vector3.MoveTowards(magnetTarget.position, transform.position, magnetPullSpeed * Time.deltaTime);

        if (Vector3.Distance(magnetTarget.position, transform.position) < 0.4f)
        {
            PowerUp powerUp = magnetTarget.GetComponent<PowerUp>();

            if (powerUp != null)
            {
                powerUp.Collect(this);
            }

            Destroy(magnetTarget.gameObject);
            magnetTarget = null;
        }
    }
    public void CalculateAmmo(int currentAmmo)
    {
        _ammoCount = currentAmmo;
        _uiManager.UpdateAmmo(_ammoCount);
    }
    public void Damage()
    {
        if (_shieldActive == true)
        {
            ShieldDamage();
        }
        else if (_shieldActive == false)
        {
            PlayerDamage();
        }
    }
    public void PlayerDamage()
    {
        _lives -= 1;
        StartCoroutine(_cameraShake.CameraShakeCoroutine(0.4f, 0.4f));

        if (_lives == 2)
        {
            _firstHit.SetActive(true);
        }
        else if (_lives == 1)
        {
            _SecondHit.SetActive(true);
        }
        _uiManager.UpdateLives(_lives);
        if (_lives < 1)
        {
            _playerAudio.PlayDeath();
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject, 1.5f);
        }
    }
    public void ShieldDamage()
    {
        _shieldLevel -= 1;

        if (_shieldLevel == 2)
        {
            _shieldFull.SetActive(false);
            _shieldMid.SetActive(true);
        }
        else if (_shieldLevel == 1)
        {
            _shieldMid.SetActive(false);
            _shieldLow.SetActive(true);
        }
        if (_shieldLevel == 0)
        {
            _shieldLow.SetActive(false);
            _shieldActive = false;
        }
    }
    public void ExtraLife()
    {
        _lives += 1;

        if (_lives == 4)
        {
            _lives = 3;
        }
        if (_lives == 3)
        {
            _firstHit.SetActive(false);
        }
        else if (_lives == 2)
        {
            _SecondHit.SetActive(false);
        }
        _uiManager.UpdateLives(_lives);
    }
    public void ShieldActive()
    {
        _shieldActive = true;
        _shieldLevel = 3;
        _shieldLow.SetActive(false);
        _shieldMid.SetActive(false);
        _shieldFull.SetActive(true);
    }
    public void TripleZapActive()
    {
        _tripleZapActive = true;
        StartCoroutine(TripleZapTimer());
    }
    IEnumerator TripleZapTimer()
    {
        yield return new WaitForSeconds(3);
        _tripleZapActive = false;
    }
    public void MegaZapActive()
    {
        _megaZapActive = true;
        StartCoroutine(MegaZapTimer());
    }
    IEnumerator MegaZapTimer()
    {
        yield return new WaitForSeconds(5);
        _megaZapActive = false;
    }
    public void HomingZapActive()
    {
        _homingZapActive = true;
        _homingShotsRemaining = 3;
    }
    public void SpeedBoostActive()
    {
        _speed *= _speedMultiplied;
        StartCoroutine(SpeedBoostTimer());
    }
    IEnumerator SpeedBoostTimer()
    {
        yield return new WaitForSeconds(5);
        _speed /= _speedMultiplied;
    }
    public void SetSpeedTemporary(float newSpeed, float duration)
    {
        StartCoroutine(TemporarySpeed(newSpeed, duration));
    }
    private IEnumerator TemporarySpeed(float newSpeed, float duration)
    {
        float originalSpeed = _speed;
        _speed = newSpeed;
        yield return new WaitForSeconds(duration);
        _speed = originalSpeed;
    }
    public void ZapRecharge()
    {
        _uiManager.RechargeText();
        _ammoCount = 15;
        CalculateAmmo(_ammoCount);
    }
    public void AddScore(int points)
    {
        _score += points;
        _uiManager.AddScore(_score);
    }
}

