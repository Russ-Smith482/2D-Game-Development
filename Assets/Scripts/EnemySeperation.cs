using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySeperation : MonoBehaviour

{
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private float force = 2f;
    private int enemyLayer;
    private void Start()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
    }
    private void Update()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.transform == transform) continue;

            Vector3 dir = transform.position - hit.transform.position;
            transform.position += dir.normalized * force * Time.deltaTime;
        }
    }
}
