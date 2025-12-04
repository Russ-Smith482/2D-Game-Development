using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostYellow : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;

    [SerializeField]
    private float dodgeRange = 2.5f;
    [SerializeField]
    private float dodgeAmount = 1.5f;
    [SerializeField]
    private float dodgeSpeed = 4f;

    private bool _isDodging = false;
    private Vector3 _dodgeTarget;

    private Player _player;
    private AudioSource _audioSource;
    private Animator _anim;
    private enum MovePhase { UpToNeg3, LeftTo0, UpTo3, LeftForever }
    private MovePhase phase = MovePhase.UpToNeg3;
    // Start is called before the first frame update
    void Start()
    {

        transform.position = new Vector3(Random.Range(7.5f, 9.5f), -5.5f, 0);
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
            Debug.LogError("Animator is NULL");
        }

    }
    // Update is called once per frame
    void Update()
    {
        DodgeCheck();
        DodgeMovement();

        if (_isDodging)
            return;

        switch (phase)
        {
            case MovePhase.UpToNeg3:
                MoveUpToNeg3();
                break;

            case MovePhase.LeftTo0:
                MoveLeftTo0();
                break;

            case MovePhase.UpTo3:
                MoveUpTo3();
                break;

            case MovePhase.LeftForever:
                MoveLeftDestroy();
                break;
        }
    }
    private void DodgeCheck()
    {
        if (_isDodging) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, dodgeRange);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Zap"))
            {
                ChooseRandomDodge();
                break;
            }
        }
    }
    private void ChooseRandomDodge()
    {
        int choice = Random.Range(0, 3); // 0 = up, 1 = down, 2 = stay

        switch (choice)
        {
            case 0:
                _dodgeTarget = transform.position + new Vector3(0, dodgeAmount, 0);
                break;
            case 1:
                _dodgeTarget = transform.position - new Vector3(0, dodgeAmount, 0);
                break;
            default:
                _dodgeTarget = transform.position;
                break;
        }

        _isDodging = true;
    }
    private void DodgeMovement()
    {
        if (!_isDodging) return;

        transform.position = Vector3.MoveTowards(transform.position, _dodgeTarget, dodgeSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _dodgeTarget) < 0.1f)
        {
            _isDodging = false;
        }
    }
    private void MoveUpToNeg3()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        if (transform.position.y >= Random.Range(-2f, -3.5f))
        {
            phase = MovePhase.LeftTo0;
        }
    }
    private void MoveLeftTo0()
    {
        transform.Translate(Vector3.left * _speed * Time.deltaTime);
        if (transform.position.x <= Random.Range(-1.5f, 0f))
        {
            phase = MovePhase.UpTo3;
        }
    }
    private void MoveUpTo3()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        if (transform.position.y >= Random.Range(2.5f, 4.5f))
        {
            phase = MovePhase.LeftForever;
        }
    }
    private void MoveLeftDestroy()
    {
        transform.Translate(Vector3.left * _speed * Time.deltaTime);
        if (transform.position.x <= -9.75f)
        {
            transform.position = new Vector3(Random.Range(7.5f, 9.5f), -5.5f, 0);
            phase = MovePhase.UpToNeg3;
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
            if (_anim != null)
            {
                _anim.SetTrigger("Hit");
            }
            Destroy(this.gameObject, 0.6f);
        }
        else if (other.tag == "Zap")
        {
            WaveManager.Instance.EnemyDestroyed();
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(200);
            }
            _speed = 0;
            _audioSource.Play();
            if (_anim != null)
            {
                _anim.SetTrigger("Hit");
            }
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 0.5f);
        }
    }
}
