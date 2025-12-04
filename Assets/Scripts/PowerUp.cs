using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2.5f;
    //ID for potions : 0=TripleZap, 1=Speed, 2=Shield, 3=ZapRecharge, 4=LifePowerUp, 5=MegaZap, 6=FirePotion
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
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            Collect(player);
            Destroy(this.gameObject);
        }
        else if (other.CompareTag("EnemyFire"))
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }

    public void Collect(Player player)
    {
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
                case 5:
                    player.MegaZapActive();
                    break;
                case 6:
                    player.Damage();
                    break;
                default:
                    Debug.Log("Default Vault");
                    break;
            }
        }
    }
}
