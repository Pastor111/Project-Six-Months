using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveEnemy : EnemyBehaviour
{
    public float Timer;
    float countdown;
    public GameObject Explosion;
    public float MinDisntanceToExplode;

    public Color DamageColor;
    public GameObject DeathParticles;

    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<Player>().transform;
        startColor = GetComponent<Renderer>().material;

        countdown = Timer;
    }

    // Update is called once per frame
    void Update()
    {
        float dis = Vector3.Distance(transform.position, target.position);

        if (dis > MinDisntanceToExplode)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            countdown = Timer;


        }
        else
        {
            agent.isStopped = true;

            countdown -= Time.deltaTime;

            float t = Mathf.Sin(countdown);

            GetComponent<Renderer>().material.color = Color.Lerp(Color.green, Color.white, t);

            if (countdown <= 0)
            {
                Explode();
            }

        }
    }

    public void Explode()
    {
        Instantiate(Explosion, transform.position, Quaternion.identity);
        Die();
    }

    public override void GetDamage(int damage)
    {
        base.GetDamage(damage);


        GetComponent<Renderer>().material.color = DamageColor;
        StartCoroutine(ShowDamage(0.2f, GetComponent<Renderer>()));


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

}
