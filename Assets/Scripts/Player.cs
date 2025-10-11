using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.5f;
    [SerializeField]
    private GameObject _zapPrefab;
    [SerializeField] 
    private GameObject _tripleZap;
    [SerializeField]
    private float _fireRate = 0.2f;
    private float _canFire = -1f;
    [SerializeField] 
    private int _lives = 3;
    private SpawnManager _spawnManager;

    [SerializeField]
    private bool _tripleZapActive = false;
    //bool for is TripleZap Active

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(-4, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
      
        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireWand();
        }
    }

    void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(direction * _speed * Time.deltaTime);


        if (transform.position.x >= -3f)
        {
            transform.position = new Vector3(-3f, transform.position.y, 0);

        }

        else if (transform.position.x <= -8f)
        {
            transform.position = new Vector3(-8f, transform.position.y, 0);
        }


        if (transform.position.y >= 4.2f)
        {
            transform.position = new Vector3(transform.position.x, 4.2f, 0);
        }

        else if (transform.position.y <= -4f)
        {
            transform.position = new Vector3(transform.position.x, -4f, 0);
        }

      
    }

    void FireWand()
    {
        _canFire = Time.time + _fireRate;


        if (_tripleZapActive == true) 
        {
            Instantiate(_tripleZap, transform.position, Quaternion.identity);
        }
        else if (_tripleZapActive == false)
        {
            Instantiate(_zapPrefab, transform.position + new Vector3(0.8f, 0, 0), Quaternion.identity);
        }

    }
    public void Damage()
    {
        _lives -= 1;

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }
}

