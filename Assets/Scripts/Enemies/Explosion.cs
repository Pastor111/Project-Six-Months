using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float Radius;
    public float ExplosionForce_Enemy;
    public float ExplosionForce_Player;
    public int DamagePlayer;
    public int DamageEnemy;

    // Start is called before the first frame update
    void Start()
    {
        Explode();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Explode()
    {
        var coll = Physics.OverlapSphere(transform.position, Radius);

        for (int i = 0; i < coll.Length; i++)
        {
            if (coll[i].gameObject.CompareTag("Player"))
            {
                coll[i].attachedRigidbody.AddExplosionForce(ExplosionForce_Player, transform.position, Radius);
                Player p = coll[i].GetComponent<Player>();
                p.GetDamage(DamagePlayer);
                EZCameraShake.CameraShaker.Instance.Shake(EZCameraShake.CameraShakePresets.Explosion);
            }

            if (coll[i].gameObject.CompareTag("Enemy"))
            {
                coll[i].attachedRigidbody.AddExplosionForce(ExplosionForce_Enemy, transform.position, Radius);
                EnemyBehaviour p = coll[i].GetComponent<EnemyBehaviour>();
                p.GetDamage(DamageEnemy);
                //EZCameraShake.CameraShaker.Instance.Shake(EZCameraShake.CameraShakePresets.Explosion);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
