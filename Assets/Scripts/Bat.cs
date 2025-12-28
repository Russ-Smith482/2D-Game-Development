using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour
{
    private Player _player;
    private AudioSource _audioSource;
    private Animator _anim;

    [SerializeField]
    private float _normalSpeed = 3.5f;
    [SerializeField]
    private float _ramSpeed = 6f;
    [SerializeField]
    private float _ramRadius = 6f;

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
            Debug.LogError("Audio Source on BatR is NULL");
        }
        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("Animator on BatR is NULL");
        }
    }
    void Update()
    {
        CalculateMovement();
    }
    private void CalculateMovement()
    {
        if (_player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

        if (distanceToPlayer <= _ramRadius)
        {
            Vector3 directionToPlayer = (_player.transform.position - transform.position).normalized;
            transform.Translate(directionToPlayer * _ramSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.left * _normalSpeed * Time.deltaTime);
        }

        if (transform.position.x <= -9.75f)
        {
            float randomYSpawn = Random.Range(-4f, 4.2f);
            transform.position = new Vector3(10f, randomYSpawn, 0);
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
            _ramSpeed = 0;
            _normalSpeed = 0;
            _audioSource.Play();
            WaveManager.Instance.EnemyDestroyed();
            Destroy(this.gameObject, 1f);
        }
        else if (other.tag == "Zap")
        {
            WaveManager.Instance.EnemyDestroyed();
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(200);
            }
            _anim.SetTrigger("Hit");
            _normalSpeed = 0;
            _ramSpeed = 0;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 1f);
        }
    }
}
