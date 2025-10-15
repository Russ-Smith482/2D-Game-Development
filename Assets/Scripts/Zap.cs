using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zap : MonoBehaviour
{
    [SerializeField]
    private float _speed = 6.0f;
   
    void Update()
    {
        transform.Translate(Vector3.right * _speed * Time.deltaTime);

        if (transform.position.x >= 9.5f)
        {
            if (transform.parent != null)

            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }

    }
}
