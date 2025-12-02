using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mummy : MonoBehaviour

{
    [SerializeField]
    private float _speed = 2.5f;
    [SerializeField]
    private float _pauseDuration = 1f;

    [SerializeField] 
    private GameObject _bandage;
    private GameObject _currentBandage;
    [SerializeField] 
    private Transform _firePoint;

    private Player _player;
    private AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MovementPattern());

        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("Audio Source on the enemy is NULL");
        }
    }
    void Update()
    {
        if (transform.position.x <= -9.75f || transform.position.y <= -6f)
        {
            float randomYSpawn = Random.Range(-4f, 4.2f);
            transform.position = new Vector3(10f, randomYSpawn, 0);
        }
    }

   
    private IEnumerator MovementPattern()
    {
        while (true)
        {
            yield return MoveForDuration(new Vector2(-1f, 0f).normalized, 2f);

            yield return PauseAndFire(_pauseDuration);

            if (_currentBandage != null)
            {
                Destroy(_currentBandage);
            }

            yield return MoveForDuration(new Vector2(-1f, -1f).normalized, 0.75f);

            yield return PauseAndFire(_pauseDuration);

            if (_currentBandage != null)
            {
                Destroy(_currentBandage);
            }
        }
    }
    private IEnumerator MoveForDuration(Vector2 direction, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            transform.Translate(direction * _speed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null; 
        }
    }
    private IEnumerator PauseAndFire(float duration)
    {
        MummyAttack();
        yield return new WaitForSeconds(duration);
    }
    private void MummyAttack()
    {
        if (_bandage != null && _firePoint != null)
        {
            _currentBandage = Instantiate(_bandage, _firePoint.position, Quaternion.identity);

            _currentBandage.GetComponent<Bandage>().owner = this.gameObject;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            _speed = 0;
            _audioSource.Play();
            WaveManager.Instance.EnemyDestroyed();
            Destroy(this.gameObject, 0.6f);
        }
        else if (other.tag == "Zap")
        {
            WaveManager.Instance.EnemyDestroyed();
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(150);
            }
            _speed = 0;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 0.5f);
        }
    }
}
