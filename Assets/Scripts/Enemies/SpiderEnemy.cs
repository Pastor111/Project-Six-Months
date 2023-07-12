using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderEnemy : EnemyBehaviour
{
    public Color DamageColor;
    public GameObject DeathParticles;

    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<PlayerMovement>().transform;
        startColor = GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.position);
    }

    public override void OnBulletCollide(Bullet bullet)
    {
        base.OnBulletCollide(bullet);
        Life--;

        GetComponent<Renderer>().material.color = DamageColor;
        StartCoroutine(ShowDamage(0.2f, GetComponent<Renderer>()));


        if (Life <= 0)
        {
            FindObjectOfType<Gun>().ShowHitMarker(Color.red);
            Die();
        }
        else
        {
            FindObjectOfType<Gun>().ShowHitMarker(Color.black);
        }

    }

    public override void GetDamage(int damage)
    {
        base.GetDamage(damage);


        GetComponent<Renderer>().material.color = DamageColor;
        StartCoroutine(ShowDamage(0.2f, GetComponent<Renderer>()));


    }


    public override void OnKnifeCollide(Knife knife, int damage)
    {
        base.OnKnifeCollide(knife, damage);

        Life -= damage;

        GetComponent<Renderer>().material.color = DamageColor;
        StartCoroutine(ShowDamage(0.2f, GetComponent<Renderer>()));


        if (Life <= 0)
        {
            FindObjectOfType<Gun>().ShowHitMarker(Color.red);
            Die();
        }
        else
        {
            FindObjectOfType<Gun>().ShowHitMarker(Color.black);
        }
    }

    public override void Die()
    {
        base.Die();
        Destroy(gameObject);
        Instantiate(DeathParticles, transform.position, Quaternion.identity);
    }


    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.transform == Player.instance.transform)
        {
            Player.instance.GetDamage();
        }
    }
}
