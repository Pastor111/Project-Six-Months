using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterEnemy : EnemyBehaviour
{
    public Weapon gun;

    public WeaponInfo Magazine;

    public GameObject[] gunModels;
    public GameObject Muzzle;

    public float DistanceAwayFromPlayer;
    public float MinDistanceAwayFromPlayer;
    public Vector2 ShootDelay;
    public float VisionRadius;

    public float Damping = 2;

    public Color DamageColor;
    public GameObject DeathParticles;


    public float Innacuracy;

    Transform appearPoint;

    Vector3 targetPos;

    bool CanShoot = false;
    bool Reloading = false;

    // Start is called before the first frame update
    void Start()
    {
        Magazine = new WeaponInfo() { LeftMag = gun.Mag, LeftTotal = gun.DefaultMaxMag };
        SetWeapon(gun);
        target = FindObjectOfType<Player>().transform;
        startColor = GetComponent<Renderer>().material;
        StartCoroutine(WaitShoot(2));

        targetPos = GetNewTargetPos(20f);

    }

    // Update is called once per frame
    void Update()
    {

        //float dis = Vector3.Distance(transform.position, target.position);

        //if (dis >= MinDistanceAwayFromPlayer)
        //{
            //agent.isStopped = false;
            agent.SetDestination(target.position);
        //}
        //else
        //    agent.isStopped = true;

        var lookPos = target.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * Damping);

        Shoot();

    }

    public Vector3 GetNewTargetPos(float radius)
    {
        var p = GetPointInRadius(target.position, radius);

        p.y = target.position.y;

        return p;
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

    public override void GetDamage(int damage)
    {
        base.GetDamage(damage);


        GetComponent<Renderer>().material.color = DamageColor;
        StartCoroutine(ShowDamage(0.2f, GetComponent<Renderer>()));


    }

    public void Reload()
    {
        if (Reloading)
            return;


        targetPos = GetNewTargetPos(20f);


        Reloading = true;
        CanShoot = false;
        //anim.SetTrigger("Reload");
        AudioManager.PlaySound3D(gun.ReloadSound, false, 1, transform.position, 0, 50f, 1, SoundEffectsMixer);
        StartCoroutine(Reload_Wait(gun.ReloadTime));
    }

    IEnumerator Reload_Wait(float t)
    {
            yield return new WaitForSeconds(t);

            var add = gun.Mag - Magazine.LeftMag;

            if (add >= 1000)
                add = 1000;

            Magazine.LeftTotal -= add;
            Magazine.LeftMag += add;

        Reloading = false;
        StartCoroutine(WaitShoot(1));
            //int mag2 = currentWeapon.Mag;
            //TotalMagLeft = mag2;
        
    }

    public override IEnumerator ShowDamage(float t, Renderer renderer)
    {
        return base.ShowDamage(t, renderer);
    }

    public override void Die()
    {
        base.Die();

        Destroy(gameObject);
        Instantiate(DeathParticles, transform.position, Quaternion.identity);
    }

    public void Shoot()
    {
        if (CanShoot)
        {

            if(Magazine.LeftMag > 0)
            {

                bool PlayerInRange = Physics.SphereCast(appearPoint.position, VisionRadius, transform.forward, out RaycastHit hit, 200f, Player.instance.transform.gameObject.layer);


                if (PlayerInRange)
                {
                    var bullet = Instantiate(gun.Enemy_Bullet, appearPoint.position, Quaternion.identity).GetComponent<Bullet>();
                    var muzzle = Instantiate(Muzzle, appearPoint.position, appearPoint.rotation).GetComponent<Muzzle>();

                    AudioManager.PlaySound3D(gun.ShootingSound, false, 1, transform.position, 2, 50f, Random.Range(0.9f, 1.1f), SoundEffectsMixer);

                    var p = GetPointInRadius(target.position, Innacuracy);
                    muzzle.Restart();

                    Magazine.LeftMag--;
                    bullet.ShootRb(p - transform.position);
                    bullet.Owner = gameObject;
                    CanShoot = false;
                    StartCoroutine(WaitShoot(gun.FireRate + Random.Range(ShootDelay.x, ShootDelay.y)));
                }

            }
            else
            {
                Reload();
            }

            //Else Dont shoot i guess
        }
    }

    IEnumerator WaitShoot(float t)
    {
        yield return new WaitForSeconds(t);
        CanShoot = true;
    }

    Vector3 GetPointInRadius(Vector3 centerOfRadius, float radius)
    {
        //Vector3 centerOfRadius = target.position;
        //float radius = Innacuracy;
        Vector3 t = centerOfRadius + (Vector3)(radius * UnityEngine.Random.insideUnitCircle);

        return t;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward * 5);
        Gizmos.DrawWireSphere(transform.position + transform.forward * 5, VisionRadius);
    }


    public void SetWeapon(Weapon w)
    {

        //SetWeapon(weapons[i]);

        //CurrentGunIndex = i;

        //CurrentMag = w.Mag;
        //TotalMagLeft = w.Mag;
        //anim.runtimeAnimatorController = w.animatorController;

        for (int i = 0; i < gunModels.Length; i++)
        {
            if (gunModels[i].name == w.Name)
            {
                gunModels[i].SetActive(true);

                if (gun.type != Weapon.WeaponType.Melee)
                    appearPoint = gunModels[i].transform.Find("Appear");
                //else
                //    appearPoint = gunModels[i].transform;
            }
            else
            {
                gunModels[i].SetActive(false);
            }
        }
    }
}
