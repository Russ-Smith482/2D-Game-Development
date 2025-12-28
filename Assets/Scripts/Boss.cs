using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public UnityEngine.UI.Image healthBarFill;

    [SerializeField]
    private float _speed = 1.5f;

    [SerializeField]
    private int _lives = 20;
    [SerializeField]
    private int maxLives = 20;

    private int _hitCount = 0;

    [Header("Movement Target")]
    public Vector3 stopPosition = new Vector3(4f, 0f, 0f);

    [Header("Effects")]
    public ScreenFlash screenFlash;

    private Player _player;
    private Animator _anim;

    private bool _reachedStopPoint = false;
    private bool _isInvulnerable = true;
    private SpriteRenderer _spriteRenderer;
    private Color _normalColor;
    private Coroutine _glowCoroutine;

    [Header("Minions")]
    public GameObject[] batPrefabs;
    public int batsPerTeleport = 3;
    public float batSpawnX = 10f;
    public float batMinY = -4f;
    public float batMaxY = 4.2f;

    [Header("Sweep Attack (50% HP)")]
    
    public float sweepChargeTime = 1f;
    private bool sweepUsed = false;

    [Header("Gaze Attack")]
    [SerializeField] private float gazeCooldown = 10f; 
    private bool canUseGaze = true;

    [SerializeField]
    private GameObject _gazeBeam;
    [SerializeField]
    private GameObject _shield;
    private bool _isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        if (_lives == 0) _lives = maxLives; 
        UpdateHealthBar();

        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null)
        {
            _normalColor = _spriteRenderer.color;
            StartInvulnerabilityGlow();
        }
        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("Animator is NULL");
        }
    }
    void Update()
    {
        CalculateMovement();
        if (!_isInvulnerable && !sweepUsed && ((float)_lives / maxLives) <= 0.50f)
        {
            sweepUsed = true;
            StartCoroutine(DoSweepAttack());
        }
    }
    private void CalculateMovement()
    {
        if (_reachedStopPoint)
            return;

        _shield.SetActive(true);

        transform.position = Vector3.MoveTowards(transform.position, stopPosition, _speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, stopPosition) < 0.1f)
        {
            _reachedStopPoint = true;
            _shield.SetActive(false);
            _isInvulnerable = false;

            StopInvulnerabilityGlow();
            SpawnBats();
        }
    }
    public void Damage()
    {
        if (_isInvulnerable)
            return;

        _lives = Mathf.Max(_lives - 1, 0);  

        if (screenFlash != null)
            screenFlash.Flash();
        _hitCount++;
        UpdateHealthBar();

        if (_reachedStopPoint && _hitCount >= 3)
        {
            Teleport();
            _hitCount = 0;
        }
        float healthFraction = (float)_lives / maxLives;
        if (canUseGaze && (healthFraction <= 0.75f || healthFraction <= 0.25f))
        {
            ShootGazeAtPlayer();
            StartCoroutine(GazeCooldownRoutine());
        }

        if (_lives <= 0)
            Die();
    }
    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            float fill = Mathf.Clamp01((float)_lives / maxLives);
            healthBarFill.fillAmount = fill;
        }
    }
    private int CalculateBatsToSpawn()
    {
        float healthFraction = (float)_lives / 20f;  

        if (healthFraction > 2f / 3f)       
            return 2;
        else if (healthFraction > 1f / 3f)  
            return 3;
        else                              
            return 4;
    }
    private void Teleport()
    {
        float newX = Random.Range(0f, 7.5f);
        float newY = Random.Range(-2.75f, 3.25f);

        transform.position = new Vector3(newX, newY, transform.position.z);
        StartCoroutine(TemporaryInvulnerability(2f));
        SpawnBats();
    }
    private IEnumerator TemporaryInvulnerability(float duration)
    {
        _shield.SetActive(true);
        _isInvulnerable = true;
        StartInvulnerabilityGlow();

        yield return new WaitForSeconds(duration);

        _isInvulnerable = false;
        _shield.SetActive(false);
        StopInvulnerabilityGlow();
    }
    private void StartInvulnerabilityGlow()
    {
        if (_spriteRenderer != null)
        {
            if (_glowCoroutine != null)
                StopCoroutine(_glowCoroutine);

            _glowCoroutine = StartCoroutine(GlowRoutine());
        }
    }
    private void StopInvulnerabilityGlow()
    {
        if (_glowCoroutine != null)
            StopCoroutine(_glowCoroutine);

        if (_spriteRenderer != null)
            _spriteRenderer.color = _normalColor;
    }
    private IEnumerator GlowRoutine()
    {
        float pulseSpeed = 2f;
        Color glowColor = new Color(1f, 0.2f, 0.2f);

        while (true)
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            _spriteRenderer.color = Color.Lerp(_normalColor, glowColor, t);
            yield return null;
        }
    }
    private void SpawnBats()
    {
        int batsToSpawn = CalculateBatsToSpawn();

        for (int i = 0; i < batsToSpawn; i++)
        {
            float randomY = Random.Range(batMinY, batMaxY);
            Vector3 spawnPos = new Vector3(batSpawnX, randomY, 0f);
            if (batPrefabs.Length > 0)
            {
                int randomIndex = Random.Range(0, batPrefabs.Length);
                Instantiate(batPrefabs[randomIndex], spawnPos, Quaternion.identity);
            }
        }
    }
    private IEnumerator DoSweepAttack()
    {
        _isInvulnerable = true;
        _shield.SetActive(true);
        StartInvulnerabilityGlow();

        bool topHalf = Random.value > 0.5f;
        float yPos = topHalf ? 1.75f : -1.75f;

        transform.position = new Vector3(7.5f, yPos, 0f);
        yield return new WaitForSeconds(0.5f); 

        float t = 0f;
        float chargeTime = 3f;
        while (t < chargeTime)
        {
            t += Time.deltaTime;
            yield return null;
        }

        float rushSpeed = 12f;
        while (transform.position.x > -11f)
        {
            transform.position += Vector3.left * rushSpeed * Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(Random.Range(0f, 7.5f), Random.Range(-2.5f, 3.5f), 0f);

        _isInvulnerable = false;
        _shield.SetActive(false);
        StopInvulnerabilityGlow();
    }
    private IEnumerator GazeCooldownRoutine()
    {
        canUseGaze = false;
        yield return new WaitForSeconds(gazeCooldown);
        canUseGaze = true;
    }
    private void ShootGazeAtPlayer()
    {
        GameObject gaze = Instantiate(_gazeBeam, transform.position, Quaternion.identity);
        GazeProjectile gp = gaze.GetComponent<GazeProjectile>();
        gp.SetTarget(_player.transform, _player);
    }
    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        StopAllCoroutines();   
        DestroyAllEnemies();

        _anim.SetTrigger("Death");
        FindObjectOfType<GameManager>().BossDefeated();

        Destroy(gameObject, 1.5f);
    }
    private void DestroyAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Zap")
        {
            Damage();
            Destroy(other.gameObject);
        }
    }
}