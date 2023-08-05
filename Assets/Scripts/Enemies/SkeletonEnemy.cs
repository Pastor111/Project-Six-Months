using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonEnemy : EnemyBehaviour
{
    public Animator anim;
    public Material DamageMaterial;
    public GameObject DeathParticles;
    public float DeathForce;
    public Renderer renderer;

    public AudioClip Die_Sound;

    [Space]
    [Space]
    [Space]
    public float AttackDistance;
    public float AttackRadius;
    public float AttackTimer;
    public float AttackDelay;
    [Space]
    public int Damage;
    bool canAttack = true;


    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<Player>().transform;
        startColor = renderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        if(GetDistanceToTarget() > AttackDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
        else
        {
            agent.isStopped = true;
            transform.LookAt(target.position);
        }

        anim.SetBool("Walking", !agent.isStopped);

        if (agent.isStopped)
        {
            if (canAttack)
            {
                StartCoroutine(WaitAttack());
            }
        }
        
    }

    float GetDistanceToTarget()
    {
        return Vector3.Distance(target.position, transform.position);
    }


    public override void OnBulletCollide(Bullet bullet)
    {
        base.OnBulletCollide(bullet);
        Life -= bullet.Damage;


        renderer.material = DamageMaterial;
        StartCoroutine(ShowDamage(0.2f, renderer));


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


        renderer.material = DamageMaterial;
        StartCoroutine(ShowDamage(0.2f, renderer));


    }

    IEnumerator WaitAttack()
    {
        canAttack = false;
        yield return new WaitForSeconds(AttackTimer);
        
        //if(!(GetDistanceToTarget() > agent.stoppingDistance))
        //{
            Attack();
        //}
    }

    public void Attack()
    {
        anim.SetTrigger("Attack #1");

        StartCoroutine(DoDamage(AttackDelay));

        canAttack = true;
    }

    IEnumerator DoDamage(float t)
    {
        yield return new WaitForSeconds(t);
        bool h = Physics.SphereCast(transform.position, AttackRadius, transform.forward, out RaycastHit hit, AttackDistance * 1.2f);

        if(h == true)
        {
            if(hit.collider.transform == target)
                Player.instance.GetDamage(Damage);
        }

    }


    public override void OnKnifeCollide(Knife knife, int damage)
    {
        base.OnKnifeCollide(knife, damage);

        Life -= damage;

        renderer.material = DamageMaterial;
        StartCoroutine(ShowDamage(0.2f, renderer));


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
        var room = LevelGenerator.generator.GetRoomInFloor(0, LevelGenerator.generator.CurrentPlayerRoom);
        var ragdoll = Instantiate(DeathParticles, transform.position, Quaternion.identity, room.transform);

        var dir = (transform.position - target.position).normalized;

        var rb = ragdoll.GetComponentsInChildren<Rigidbody>();

        for (int i = 0; i < rb.Length; i++)
        {
            rb[i].AddForce(dir * DeathForce);
        }

        AudioManager.PlaySound3D(Die_Sound, false, 1.0f, transform.position, 2f, 50f, Random.Range(0.9f, 1.1f), SoundEffectsMixer);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, AttackDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * AttackDistance));
        Gizmos.DrawWireSphere(transform.position + (transform.forward * AttackDistance), AttackRadius);
        Gizmos.color = Color.black;
    }


    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.transform == Player.instance.transform)
        {
            //Player.instance.GetDamage();
        }

        if(collision.collider.GetComponent<SlimeEnemy>() != null)
        {
            GetDamage(1000);
        }
    }
}
