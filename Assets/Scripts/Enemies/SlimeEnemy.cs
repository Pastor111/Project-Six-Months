using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeEnemy : EnemyBehaviour
{
    public Renderer renderer;
    public Animator anim;
    public int Damage;
    public float JumpTimer;
    public float Speed;
    public Vector3 JumpForce;
    public float RotationSpeed;
    public float PathUpdateTime;
    public GameObject MiniVersion;
    public int ReSpawns;


    public float Height;
    public float GroundLevel = 2f;

    public float PushForce;
    public Material DamageColor;
    public GameObject DeathParticles;
    public GameObject JumpParticles;

    public AudioClip JumpSound;
    public AudioClip DieSound;

    bool isGrounded
    {
        get
        {
            return Physics.Raycast(transform.position - new Vector3(0, Height, 0), Vector3.down, GroundLevel);
        }
    }

    bool WasGrounded;
    Rigidbody rb;
    float time;

    Vector3 currentLookPos;

    NavMeshPath path;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = FindObjectOfType<Player>().transform;
        rb = GetComponent<Rigidbody>();
        startColor = renderer.material;
        Jump();
        currentLookPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if(time >= PathUpdateTime)
        {
            time -= PathUpdateTime;
            GetNewPath(Player.instance.movement.GetGroundPosition(), out path);
            //Debug.Log();


        }

        if(path != null)
        {
            //Debug.Log(path.corners.Length);
            for (int i = 0; i < path.corners.Length - 1; i++)
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red, 1f);


            currentLookPos = Vector3.MoveTowards(currentLookPos, new Vector3(path.corners[1].x, transform.position.y, path.corners[1].z), RotationSpeed * Time.deltaTime);

            transform.LookAt(currentLookPos);

        }

        if(!WasGrounded && isGrounded)
        {
            OnEnterGround();
        }

        if(WasGrounded && !isGrounded)
        {
            OnExitGround();
        }

        WasGrounded = isGrounded;
    }

    public void OnEnterGround()
    {
        anim.SetTrigger("Land");
        Instantiate(JumpParticles, transform.position - new Vector3(0, Height, 0), Quaternion.identity);
        AudioManager.PlaySound3D(JumpSound, false, 1.0f, transform.position, 2f, 50f, Random.Range(0.9f, 1.1f), SoundEffectsMixer);
    }

    public void OnExitGround()
    {
        anim.SetTrigger("Jump");
        Instantiate(JumpParticles, transform.position - new Vector3(0, Height, 0), Quaternion.identity);
        AudioManager.PlaySound3D(JumpSound, false, 1.0f, transform.position, 2f, 50f, Random.Range(0.9f, 1.1f), SoundEffectsMixer);
    }

    public override void OnBulletCollide(Bullet bullet)
    {
        base.OnBulletCollide(bullet);
        Life -= bullet.Damage;

        renderer.material = DamageColor;
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


        renderer.material = DamageColor;
        StartCoroutine(ShowDamage(0.2f, renderer));


    }


    public override void OnKnifeCollide(Knife knife, int damage)
    {
        base.OnKnifeCollide(knife, damage);

        Life -= damage;

        renderer.material = DamageColor;
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
        Instantiate(DeathParticles, transform.position, Quaternion.identity);
        AudioManager.PlaySound3D(DieSound, false, 1.0f, transform.position, 2f, 50f, Random.Range(0.9f, 1.1f), SoundEffectsMixer);

        for (int i = 0; i < ReSpawns; i++)
        {
           Vector3 pos = transform.position + (Random.insideUnitSphere * 4f);
           pos.y = transform.position.y;
            var room = LevelGenerator.generator.GetRoomInFloor(0, LevelGenerator.generator.CurrentPlayerRoom).GetComponent<LevelRoom>();
            var e = Instantiate(MiniVersion, pos, Quaternion.identity);
            room.Enemies.Add(e);
        }

    }


    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.transform == Player.instance.transform)
        {
            Player.instance.GetDamage(Damage);
            var a = -collision.contacts[0].normal.normalized;

            Debug.Log(a);

            if (Mathf.Abs(a.x) >= 0.1f)
            {
                if (a.x > 0)
                    a.x = 1.0f;
                else
                    a.x = -1.0f;

            }
            else
            {
                a.x = 0.0f;
            }

            if (Mathf.Abs(a.y) >= 0.1f)
            {
                if (a.y > 0)
                    a.y = 1.0f;
                else
                    a.y = -1.0f;

            }
            else
            {
                a.y = 0.0f;
            }

            if (Mathf.Abs(a.z) >= 0.1f)
            {
                if (a.z > 0)
                    a.z = 1.0f;
                else
                    a.z = -1.0f;

            }
            else
            {
                a.z = 0.0f;
            }


            Player.instance.movement.GetRigidbody().AddForce(a * PushForce);
        }
    }

    public void Jump()
    {
        StartCoroutine(Move());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position - new Vector3(0, Height, 0), (transform.position - new Vector3(0, Height, 0)) + Vector3.down * GroundLevel);
    }

    IEnumerator Move()
    {
        while (true)
        {

            if (path != null && isGrounded)
            {
                var dif = path.corners[1] - transform.position;
                dif.y = 1f;
                dif.x *= JumpForce.x;
                dif.z *= JumpForce.z;
                dif.y *= JumpForce.y * Speed;
                rb.AddForce(dif);
            }
            yield return new WaitForSeconds(JumpTimer);
        }
    }

    public bool GetNewPath(Vector3 target, out NavMeshPath path)
    {
        //agent.destination = target;

        agent.enabled = true;

        var p = new NavMeshPath();
        var b = agent.CalculatePath(target, p);

        if (p.status != NavMeshPathStatus.PathInvalid)
        {
            //do whatever
            path = p;
        }
        else
        {
            path = null;
        }


        agent.enabled = false;

        return b;

    }
}
