using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweepAttack : MonoBehaviour
{
    public float verticalExpandSpeed = 5f; // units/sec
    public float horizontalSpeed = 12f;    // follows boss
    public float targetHeight = 4f;

    [HideInInspector] public Transform bossTransform;
    public bool expandUp = true; // direction of vertical expansion

    private BoxCollider2D _col;

    void Awake()
    {
        _col = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        // 1. Vertical expansion
        if (transform.localScale.y < targetHeight)
        {
            float newY = transform.localScale.y + verticalExpandSpeed * Time.deltaTime;
            newY = Mathf.Min(newY, targetHeight);

            // Apply scale
            transform.localScale = new Vector3(transform.localScale.x, newY, 1f);

            // Adjust collider
            _col.size = new Vector2(_col.size.x, newY);

            // Adjust position so it expands in correct direction
            Vector3 pos = transform.position;
            if (expandUp)
            {
                // pivot at bottom, grow upward: position stays same
            }
            else
            {
                // pivot at top, grow downward: shift center down by half growth
                transform.position = new Vector3(pos.x, pos.y - newY / 2f, pos.z);
            }
        }

        // 2. Follow boss horizontally
        if (bossTransform != null)
        {
            float xOffset = 1.5f; // behind boss
            transform.position = new Vector3(bossTransform.position.x + xOffset, transform.position.y, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player p = other.GetComponent<Player>();
            if (p != null)
                p.Damage();
        }
    }
}
