using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private float _speedMultiplied = 2f;

    private float _boostedSpeed = 5.5f;
    private float _normalSpeed = 3.5f;


    [SerializeField]
    private float _boost = 100f;
    [SerializeField]
    private float _maxBoost = 100f;
    [SerializeField]
    private float _boostUsage = 25f;  // per second
    [SerializeField]
    private float _boostRecharge = 10f; // per second
    [SerializeField]
    private float _cooldownTime = 3f;

    private bool _isBoosted = false;

    private bool _isRecharging = false;


    [SerializeField]
    private GameObject _zapPrefab;
    [SerializeField]
    private GameObject _tripleZap;

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

    private SpawnManager _spawnManager;
    private UIManager _uiManager;

    [SerializeField]
    private bool _tripleZapActive = false;

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


    [SerializeField]
    private AudioClip _zapSoundEffect;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(-4, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();


        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL");
        }

        if (_audioSource == null)
        {
            Debug.LogError("Audio Source on the player is NULL");
        }
        else
        {
            _audioSource.clip = _zapSoundEffect;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        SpeedBooster();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireWand();
        }
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
    void FireWand()
    {
        _canFire = Time.time + _fireRate;


        if (_tripleZapActive == true)
        {
            Instantiate(_tripleZap, transform.position, Quaternion.identity);
        }
        else if (_tripleZapActive == false)
        {
            RegularZap();
        }

        _audioSource.Play();

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
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
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
    public void ZapRecharge()
    {
        _ammoCount = 15;
        CalculateAmmo(_ammoCount);
    }
    public void AddScore(int points)
    {
        _score += points;
        _uiManager.AddScore(_score);
    }
}

