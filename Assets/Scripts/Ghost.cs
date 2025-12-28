using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    private Player _player;
    private AudioSource _audioSource;
    private Animator _anim;

    [SerializeField]
    private float _speed = 3f;
    private enum MovePhase { DownTo3, LeftTo0, DownToNeg3, LeftForever }
    private MovePhase phase = MovePhase.DownTo3;
    void Start()
    {
        transform.position = new Vector3(Random.Range(7.5f, 9.5f),6, 0);
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
            Debug.LogError("Anumator on GhostW is NULL");
        }
    }
    void Update()
    {
        switch (phase)
        {
            case MovePhase.DownTo3:
                MoveDownTo3();
                break;

            case MovePhase.LeftTo0:
                MoveLeftTo0();
                break;

            case MovePhase.DownToNeg3:
                MoveDownToNeg3();
                break;

            case MovePhase.LeftForever:
                MoveLeftDestroy();
                break;
        }
    }
    private void MoveDownTo3()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y <= Random.Range(2f, 3.5f))
        {
            phase = MovePhase.LeftTo0;
        }
    }
    private void MoveLeftTo0()
    {
        transform.Translate(Vector3.left * _speed * Time.deltaTime);
        if (transform.position.x <= Random.Range(-1.5f, 0f))
        {
            phase = MovePhase.DownToNeg3;
        }
    }
    private void MoveDownToNeg3()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y <= Random.Range(2.5f, -4.5f))
        {
            phase = MovePhase.LeftForever;
        }
    }
    private void MoveLeftDestroy()
    {
        transform.Translate(Vector3.left * _speed * Time.deltaTime);
        if (transform.position.x <= -9.75f)
        {
            Destroy(this.gameObject);
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
