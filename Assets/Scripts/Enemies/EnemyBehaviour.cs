using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public float Life;

    public NavMeshAgent agent;
    public Color DamageColor;
    public GameObject DeathParticles;
    Transform target;


    Color startColor;

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

    public void OnBulletCollide(Bullet bullet)
    {
        Life--;

        GetComponent<Renderer>().material.color = DamageColor;
        StartCoroutine(ShowDamage(0.2f));


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

    public void Die()
    {
        Destroy(gameObject);
        Instantiate(DeathParticles, transform.position, Quaternion.identity);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.transform == Player.instance.transform)
        {
            Player.instance.GetDamage();
        }
    }

    IEnumerator ShowDamage(float t)
    {
        yield return new WaitForSeconds(t);
        GetComponent<Renderer>().material.color = startColor;
    }
}
