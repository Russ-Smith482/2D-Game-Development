using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;
    //ID for potions : 0=TripleZap, 1=Speed, 2=Shield, 3=ZapRecharge, 4=LifePowerUp
    [SerializeField]
    private int _powerUpID;
    [SerializeField]
    private AudioClip _clip;

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

            AudioSource.PlayClipAtPoint(_clip, transform.position);

            if (player != null)
            {
                switch (_powerUpID)
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
                    case 3:
                        player.ZapRecharge();
                        break;

                    case 4:
                        player.ExtraLife();
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
