using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.5f;
    [SerializeField]
    private float _speedMultiplied = 2f;
    [SerializeField]
    private GameObject _zapPrefab;
    [SerializeField]
    private GameObject _tripleZap;
    
    [SerializeField]
    private float _fireRate = 0.2f;
    private float _canFire = -1f;

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
    private GameObject _shieldVisualizer;


    // variable to store audio clip
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


        if (transform.position.y >= 4.2f)
        {
            transform.position = new Vector3(transform.position.x, 4.2f, 0);
        }

        else if (transform.position.y <= -4f)
        {
            transform.position = new Vector3(transform.position.x, -4f, 0);
        }


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
            Instantiate(_zapPrefab, transform.position + new Vector3(0.8f, 0, 0), Quaternion.identity);
        }

        _audioSource.Play();
        //play zap audio clip

    }
    public void Damage()
    {
        if (_shieldActive == true)
        {
            _shieldActive = false;
            _shieldVisualizer.SetActive(false);
            return;
        }

        else if (_shieldActive == false)
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
         }

            if (_lives < 1)
            {
                _spawnManager.OnPlayerDeath();
                Destroy(this.gameObject);

            }
        
    }

    public void ShieldActive()
    {
        _shieldActive = true;
        _shieldVisualizer.SetActive(true);
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

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.AddScore(_score);
    }
    //method to add 100 to score
    //communicate with ui to add score
}

