using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzelGames.FastIK;

public class SpiderEnemy : EnemyBehaviour
{
    public FastIKFabric[] legs;
    public float Height;
    public Vector3[] RayCastOffset;
    public LayerMask MASK;

    public Vector3 BalanceForce;
    public Vector3 ExtraGravity;

    Vector3[] startPos;

    Transform[] targets;
    Transform[] poles;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody>();

        SetUpLegs();


        startPos = new Vector3[legs.Length];

        for (int i = 0; i < legs.Length; i++)
        {
            startPos[i] = legs[i].transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void FixedUpdate()
    {
        rb.AddForce(BalanceForce);
        rb.AddForce(ExtraGravity);
    }

    private void LateUpdate()
    {
        for (int i = 0; i < legs.Length; i++)
        {
            if (Physics.Raycast(startPos[i] - new Vector3(0, Height, 0), Vector3.down, out RaycastHit hit, 100f, MASK))
            {
                targets[i].position = hit.point + RayCastOffset[i];
                //poles[i].position = targets[i].position;
            }
        }
    }

    void SetUpLegs()
    {
        targets = new Transform[legs.Length];
        poles = new Transform[legs.Length];

        for (int i = 0; i < legs.Length; i++)
        {
            targets[i] = new GameObject($"Leg {i} Target").transform;
            poles[i] = new GameObject($"Leg {i} Pole").transform;

            targets[i].parent = transform;
            poles[i].parent = transform;

            legs[i].Target = targets[i];
            legs[i].Pole = poles[i];
        }

    }

    public override void OnBulletCollide(Bullet bullet)
    {
        base.OnBulletCollide(bullet);
        //Life--;

        //GetComponent<Renderer>().material.color = DamageColor;
        //StartCoroutine(ShowDamage(0.2f, GetComponent<Renderer>()));


        //if (Life <= 0)
        //{
        //    FindObjectOfType<Gun>().ShowHitMarker(Color.red);
        //    Die();
        //}
        //else
        //{
        //    FindObjectOfType<Gun>().ShowHitMarker(Color.black);
        //}

    }

    public override void GetDamage(int damage)
    {
        base.GetDamage(damage);


        //GetComponent<Renderer>().material.color = DamageColor;
        //StartCoroutine(ShowDamage(0.2f, GetComponent<Renderer>()));


    }


    public override void OnKnifeCollide(Knife knife, int damage)
    {
        //base.OnKnifeCollide(knife, damage);

        //Life -= damage;

        //GetComponent<Renderer>().material.color = DamageColor;
        //StartCoroutine(ShowDamage(0.2f, GetComponent<Renderer>()));


        //if (Life <= 0)
        //{
        //    FindObjectOfType<Gun>().ShowHitMarker(Color.red);
        //    Die();
        //}
        //else
        //{
        //    FindObjectOfType<Gun>().ShowHitMarker(Color.black);
        //}
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && Application.isEditor)
        {
            for (int i = 0; i < legs.Length; i++)
            {
                Gizmos.DrawLine(startPos[i] - new Vector3(0, Height, 0), startPos[i] - new Vector3(0, Height, 0) + Vector3.down * 10f);
                Gizmos.DrawWireCube(targets[i].position, Vector3.one / 2f);
            }

        }

    }

    public override void Die()
    {
        //base.Die();
        //Destroy(gameObject);
        //Instantiate(DeathParticles, transform.position, Quaternion.identity);
    }


    public void OnCollisionEnter(Collision collision)
    {
        //if (collision.collider.transform == Player.instance.transform)
        //{
        //    Player.instance.GetDamage();
        //}
    }
}
