using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovePhase { DownTo3, LeftTo0, DownToNeg3, LeftForever }

public class Shielded_Ghost : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;

    private Player _player;
    private AudioSource _audioSource;

    private MovePhase phase = MovePhase.DownTo3;
    private Vector3 targetPos;

    private Animator _anim;

    [SerializeField]
    private bool _hasBeenHit = false;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(Random.Range(9.5f, 11.5f), (Random.Range(4f, 6f)), 0);

        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("Animator is NULL");
        }


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

    // Update is called once per frame
    void Update()
    {
        MoveToTarget();
    }
    private void MoveToTarget()
    {
        // Move toward target
        transform.position = Vector3.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);

        // When reached, select next target
        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            AdvancePhase();
            SetNextTarget();
        }
    }
    private void AdvancePhase()
    {
        switch (phase)
        {
            case MovePhase.DownTo3:
                phase = MovePhase.LeftTo0;
                break;
            case MovePhase.LeftTo0:
                phase = MovePhase.DownToNeg3;
                break;
            case MovePhase.DownToNeg3:
                phase = MovePhase.LeftForever;
                break;
        }
    }
    private void SetNextTarget()
    {
        Vector3 p = transform.position;

        switch (phase)
        {
            case MovePhase.DownTo3:
                targetPos = new Vector3(p.x, Random.Range(2f, 3.5f), p.z);
                break;

            case MovePhase.LeftTo0:
                targetPos = new Vector3(Random.Range(-1.5f, 0f), p.y, p.z);
                break;

            case MovePhase.DownToNeg3:
                targetPos = new Vector3(p.x, Random.Range(-4.5f, -2.5f), p.z);
                break;

            case MovePhase.LeftForever:
                targetPos = new Vector3(-10f, p.y, p.z);
                if (transform.position.x <= -9.75f)
                {
                    Destroy(this.gameObject);
                }
                // off-screen destroy zone
                break;
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

            if (_hasBeenHit == true)
            {
                _speed = 0;
                _audioSource.Play();
                WaveManager.Instance.EnemyDestroyed();
                Destroy(this.gameObject, 0.6f);
            }
            else if (_hasBeenHit == false)
            {
                _hasBeenHit = true;
                if (_anim != null)
                {
                    _anim.SetTrigger("Hit");
                }
            }
        }
        if (other.tag == "Zap")
        {
            if (_hasBeenHit == true)
            {
                WaveManager.Instance.EnemyDestroyed();
                Destroy(other.gameObject);
                if (_player != null)
                {
                    _player.AddScore(100);
                }
                _speed = 0;
                _audioSource.Play();
                Destroy(GetComponent<Collider2D>());
                Destroy(this.gameObject, 0.5f);

            }
            else if (_hasBeenHit == false)
            {
                _hasBeenHit = true;
                Destroy(other.gameObject);
                if (_anim != null)
                {
                    _anim.SetTrigger("Hit");
                }
            }
        }
    }
}

