using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatPurple : MonoBehaviour
{
    private Player _player;
    private AudioSource _audioSource;
    private Animator _anim;
    [SerializeField]
    private float _speed = 2.5f;

    private float _fireRate = 2f;
    private float _canFire = -1f;

    [SerializeField]
    private GameObject _sonarWave;

    // Start is called before the first frame update
    void Start()
    {
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
        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("Animator on Ghost P is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        EnemyFire();
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
    void EnemyFire()
    {
        if (Time.time > _canFire && _player != null)
        {
            _fireRate = Random.Range(1f, 3f);
            _canFire = Time.time + _fireRate;
            Instantiate(_sonarWave, transform.position + new Vector3(-1.25f, 0, 0), Quaternion.identity);
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
            _anim.SetTrigger("Hit");
            _speed = 0;
            _audioSource.Play();
            WaveManager.Instance.EnemyDestroyed();
            Destroy(this.gameObject, 0.9f);
        }
        else if (other.tag == "Zap")
        {
            WaveManager.Instance.EnemyDestroyed();
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(100);
            }
            _anim.SetTrigger("Hit");
            _speed = 0;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 0.9f);
        }
    }
}
