using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    [SerializeField]
    private float _speed = 6.0f;

    public float direction = -1f;

    private Player _player;

    [SerializeField]
    private AudioClip _clip;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * direction * _speed * Time.deltaTime);

        if (transform.position.x <= -9.5f || transform.position.x >= 9.5f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {

            Player player = other.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_clip, transform.position);

            if (player != null)
            {
                player.Damage();
                Destroy(this.gameObject);
            }
        }
    }
}
