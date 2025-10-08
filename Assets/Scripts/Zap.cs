using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zap : MonoBehaviour
{
    [SerializeField]
    private float _speed = 6.0f;
   
    void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y >= 6)
        {
            Destroy(this.gameObject);
        }

    }
}
