using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    
    //private GameObject _tripleZapPotion;
    [SerializeField]
    private float _speed = 3f;
    //ID for potions : 0=TripleZap, 1=Speed, 2=Shield
    [SerializeField]
    private int _potionID;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        {
            transform.Translate(Vector3.left * _speed * Time.deltaTime);

            if (transform.position.x <= -9.75f)
            {
               Destroy(this.gameObject);
            }
        }
     
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                switch(_potionID)
                {
                    case 0:
                        player.TripleZapActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldActive();
                        break;
                    default:
                        Debug.Log("Default Vault");
                        break;
                }

            }
                Destroy(this.gameObject);
            
        }
    }
}
