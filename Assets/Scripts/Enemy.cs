using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y <= -5.5f)
        {
            float randomX = Random.Range(-8, 8f);
            transform.position = new Vector3(randomX, 6f, 0);
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
            Destroy(this.gameObject);
        }
        
        else if (other.tag == "Zap")
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
      
    }
}
