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

    private int _hitCount = 0;

    [Header("Movement Target")]
    public Vector3 stopPosition = new Vector3(4f, 0f, 0f);

    [Header("Effects")]
    public ScreenFlash screenFlash; 

    private Player _player;
    private bool _reachedStopPoint = false;

    // NEW
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
    public GameObject sweepPrefab;       // Assign your Sweep prefab
    public float sweepChargeTime = 1f;
    private bool sweepUsed = false;

    [Header("Gaze Attack")]
    [SerializeField] private float gazeCooldown = 10f;  // Cooldown before gaze can happen again
    private bool canUseGaze = true;

    [SerializeField]
    private GameObject _gazeBeam;

    [SerializeField]
    private GameObject _shield;


    // Start is called before the first frame update
    void Start()
    {
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
    }
    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (!_isInvulnerable && !sweepUsed && ((float)_lives / 20f) <= 0.50f)
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

        _lives -= 1;

        if (screenFlash)
            screenFlash.Flash();
        UpdateHealthBar();

        _hitCount++;

        if (_reachedStopPoint && _hitCount >= 3)
        {
            Teleport();
            _hitCount = 0;
        }

        // --- Trigger gaze at 75% and 25% health ---
        float healthFraction = (float)_lives / 20f; // assuming max lives = 20
        if (canUseGaze && (healthFraction <= 0.75f || healthFraction <= 0.25f))
        {
            ShootGazeAtPlayer();
            StartCoroutine(GazeCooldownRoutine());
        }

        if (_lives <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)_lives / 20f;
        }
    }
    private int CalculateBatsToSpawn()
    {
        float healthFraction = (float)_lives / 20f;  // Assuming max lives = 20

        if (healthFraction > 2f / 3f)       // Above 2/3 health
            return 2;
        else if (healthFraction > 1f / 3f)  // Between 1/3 and 2/3 health
            return 3;
        else                               // Last third health
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
    // Pulsing glow coroutine
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

        // Decide top or bottom half
        bool topHalf = Random.value > 0.5f;
        float yPos = topHalf ? 1.75f : -1.75f;

        // Teleport boss
        transform.position = new Vector3(7.5f, yPos, 0f);
        yield return new WaitForSeconds(0.5f); // small pause

        // Spawn sweep behind boss
        GameObject sweep = Instantiate(sweepPrefab, transform.position + Vector3.right * 1.5f, Quaternion.identity);
        SweepAttack s = sweep.GetComponent<SweepAttack>();
        s.bossTransform = transform;
        s.targetHeight = 4f;
        s.verticalExpandSpeed = 6f;
        s.expandUp = topHalf; // determines vertical growth direction

        // 3-second charge
        float t = 0f;
        float chargeTime = 3f;
        while (t < chargeTime)
        {
            t += Time.deltaTime;
            yield return null;
        }

        // Boss rush left
        float rushSpeed = 12f;
        while (transform.position.x > -11f)
        {
            transform.position += Vector3.left * rushSpeed * Time.deltaTime;
            yield return null;
        }

        // Remove sweep
        Destroy(sweep);

        // Teleport boss elsewhere
        transform.position = new Vector3(Random.Range(0f, 7.5f), Random.Range(-2.5f, 3.5f), 0f);

        // End invulnerability
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
        Debug.Log("Boss defeated!");
        Destroy(gameObject);
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