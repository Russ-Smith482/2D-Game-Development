using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4f;

    private Player _player;

    private Animator _anim;

    private AudioSource _audioSource;

    private float _fireRate = 2f;
    private float _canFire = -1f;

    [SerializeField]
    private GameObject _seed;

    private bool _isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }

        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("Animator is NULL");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("Audio Source on the enemy is NULL");
        }
    }

    void Update()
    {
        if (_isDead) return;

        CalculateMovement();

        EnemyFire();
    }
    void EnemyFire()
    {
        if (Time.time > _canFire && _player != null)
        {
            _fireRate = Random.Range(1f, 5f);
            _canFire = Time.time + _fireRate;
            Instantiate(_seed, transform.position + new Vector3(-1f, 0, 0), Quaternion.identity);
        }
    }
    private void CalculateMovement()
    {
        transform.Translate(Vector3.left * _speed * Time.deltaTime);

        if (transform.position.x <= -9.75f)
        {
            float randomYSpawn = Random.Range(-4f, 4.2f);
            transform.position = new Vector3(10f, randomYSpawn, 0);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDead) return;

        if (other.tag == "Player")
        {
            _isDead = true;
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            WaveManager.Instance.EnemyDestroyed();
            Destroy(this.gameObject, 0.6f);
        }
        else if (other.tag == "Zap")
        {
            _isDead = true;
            WaveManager.Instance.EnemyDestroyed();
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(100);
            }
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 0.6f);
        }
    }
}
