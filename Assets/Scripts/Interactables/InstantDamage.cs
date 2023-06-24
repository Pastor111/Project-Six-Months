using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDamage : MonoBehaviour
{
    public bool DamagePlayer;
    public int Player_Damage;
    public bool DamageEnemies;
    public int Enemy_Damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (DamagePlayer)
            {
                other.GetComponent<Player>().GetDamage(Player_Damage);
            }
        }

        if (other.CompareTag("Enemy"))
        {
            if (DamageEnemies)
            {
                other.GetComponent<EnemyBehaviour>().GetDamage(Enemy_Damage);
            }
        }
    }
}
