using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : EnemyBehaviour
{

    public enum State {Walking, Attacking }
    public State currentState;

    public Renderer renderer;
    public Material DamageMaterial;
    public Vector3 FlyOffset;

    public float RotationSpeed;

    public int MinBullets;
    private int mag;

    public int MaxBullets;
    public float FireRate;
    public Transform SpawnPoint;
    public GameObject bullet;
    bool Shooting;
    public AudioClip ShootingSound;

    public float MinWalkTime;
    float WalkTime;
    public float MaxWalkTime;

    //public float RotationSmoothing;


    Vector3 playerPos;

    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<Player>().transform;
        startColor = renderer.material;
        agent.transform.parent = null;
        playerPos = target.position;
        currentState = State.Walking;
    }

    // Update is called once per frame
    void Update()
    {

        if(currentState == State.Walking)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            Shooting = false;
            WalkTime -= Time.deltaTime;
        }
        else
        {
            agent.isStopped = true;
            if(Shooting == false)
            {
                
                StartCoroutine(Attack());
                Shooting = true;
            }
        }

        if(WalkTime <= 0)
        {
            currentState = State.Attacking;
        }

        transform.position = agent.transform.position + FlyOffset;

        playerPos = Vector3.MoveTowards(playerPos, target.position, RotationSpeed * Time.deltaTime);

        transform.LookAt(playerPos);
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

    IEnumerator Attack()
    {
        mag = Random.Range(MinBullets, MaxBullets);

        float shootCountDown = 2f;

        while(shootCountDown > 0)
        {
            shootCountDown -= Time.deltaTime;
        }
        
        while(mag > 0)
        {

            var bul = Instantiate(bullet, SpawnPoint.position, Quaternion.identity).GetComponent<Bullet>();

            var dir = (playerPos - transform.position).normalized;

            bul.Owner = gameObject;
            bul.ShootRb(dir);
            AudioManager.PlaySound3D(ShootingSound, false, 1.0f, transform.position, 0, 50f, Random.Range(.9f, 1.1f), SoundEffectsMixer);

            mag--;

            yield return new WaitForSeconds(FireRate);
        }

        WalkTime = Random.Range(MinWalkTime, MaxWalkTime);
        currentState = State.Walking;
    }

    public override void Die()
    {
        base.Die();
        Destroy(gameObject);
        var room = LevelGenerator.generator.GetRoomInFloor(0, LevelGenerator.generator.CurrentPlayerRoom);
        //var ragdoll = Instantiate(DeathParticles, transform.position, Quaternion.identity, room.transform);

        var dir = (transform.position - target.position).normalized;

        //var rb = ragdoll.GetComponentsInChildren<Rigidbody>();

        //for (int i = 0; i < rb.Length; i++)
        //{
        //    rb[i].AddForce(dir * DeathForce);
        //}

        //AudioManager.PlaySound3D(Die_Sound, false, 1.0f, transform.position, 2f, 50f, Random.Range(0.9f, 1.1f), SoundEffectsMixer);

    }


}
