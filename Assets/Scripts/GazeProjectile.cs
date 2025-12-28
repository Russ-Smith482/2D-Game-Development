using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeProjectile : MonoBehaviour
{
    public Player _player;

    public float speed = 7f;
    public float pauseDuration = 1f;
    private Transform target;
    private bool isMoving = false;
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }
        StartCoroutine(LaunchRoutine());
    }
    private IEnumerator LaunchRoutine()
    {
        yield return new WaitForSeconds(pauseDuration);
        isMoving = true;
    }
    void Update()
    {
        if (isMoving && target != null)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_player != null)
            {
                _player.SetSpeedTemporary(1f, 3f);
            }
            Destroy(gameObject);
        }
    }
    public void SetTarget(Transform t, Player p)
    {
        target = t;
        _player = p;
    }
}