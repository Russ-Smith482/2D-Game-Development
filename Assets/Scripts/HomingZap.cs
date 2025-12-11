using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingZap : MonoBehaviour

{
    public float speed = 6f;
    public float rotateSpeed = 500f;

    private Transform target;

    void Start()
    {
        FindClosestEnemy();
    }

    void Update()
    {
        if (target == null)
        {
            FindClosestEnemy();
            if (target == null)
            {
                transform.Translate(Vector3.right * speed * Time.deltaTime);
                return;
            }
        }
        // Direction to enemy
        Vector3 direction = target.position - transform.position;
        direction.Normalize();

        // Rotate smoothly toward enemy
        float rotateAmount = Vector3.Cross(direction, transform.right).z;
        transform.Rotate(0, 0, -rotateAmount * rotateSpeed * Time.deltaTime);

        // Move forward
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (transform.position.x >= 9.5f)
        {
            if (transform.parent != null)

            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float shortestDist = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < shortestDist)
            {
                shortestDist = distance;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null)
        {
            target = nearestEnemy.transform;
        }
    }
}
