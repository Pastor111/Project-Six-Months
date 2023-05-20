using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInfo
{
    public int LeftMag;
    public int LeftTotal;
}

public class Gun : MonoBehaviour
{
    public Weapon currentWeapon;
    public Camera MyCamera;
    public PlayerMovement player;
    [Space]
    [Space]
    [Space]
    [Header("Visuals")]
    public Animator anim;
    public GameObject[] gunModels;
    public GameObject Muzzle;
    public Recoil recoil;
    public Crosshair crosshair;
    public float SpreadSpeed;
    [Space]
    [Header("UI")]
    public TMPro.TextMeshProUGUI BulletText;
    public Crosshair HitMarker;
    [Space]
    [Space]
    [Space]
    [Header("Other Game Feel Stuff")]
    public AudioSource source;
    public AudioClip EmptyMagSound;

    #region Private

    private int CurrentMag;
    private int TotalMagLeft;
    private Transform appearPoint;
    bool CanShoot = true;
    Ray ray;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        SetWeapon(currentWeapon);
    }

    // Update is called once per frame
    void Update()
    {
        ray = MyCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        anim.SetBool("Running", player.Running);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            TryToShoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

        BulletText.text = $"{CurrentMag} | {TotalMagLeft}";

        crosshair.Spread = Mathf.Lerp(crosshair.Spread, 0.0f, SpreadSpeed * Time.deltaTime);

        recoil.recoilX = currentWeapon.NormalRecoil.x;
        recoil.recoilY = currentWeapon.NormalRecoil.y;
        recoil.recoilZ = currentWeapon.NormalRecoil.z;

        crosshair.DefaultSpread = currentWeapon.NormalSpread;
    }

    public void TryToShoot()
    {

        if(CurrentMag <= 0)
        {
            source.PlayOneShot(EmptyMagSound);
        }

        if(CanShoot && CurrentMag > 0 && !player.Running)
        {
            Shoot();
        }


    }

    public void Shoot()
    {
        var bullet = Instantiate(currentWeapon.Bullet, appearPoint.position, Quaternion.identity).GetComponent<Bullet>();
        var muzzle = Instantiate(Muzzle, appearPoint.position, appearPoint.rotation).GetComponent<Muzzle>();

        source.PlayOneShot(currentWeapon.ShootingSound);

        EZCameraShake.CameraShaker.Instance.ShakeOnce(currentWeapon.Magnitude, currentWeapon.Rougness, currentWeapon.fadeIn, currentWeapon.fadeout);

        crosshair.Spread += currentWeapon.AddSpread;
        recoil.RecoilFire();
        muzzle.Restart();
        anim.SetTrigger("Shoot");
        Physics.Raycast(ray, out RaycastHit hit);

        if(hit.collider == null)
        {
            bullet.ShootRb(ray.direction);
        }
        else
        {
            bullet.ShootTransform(hit.point);
        }

        CurrentMag--;
        CanShoot = false;
        StartCoroutine(ShootDelay(currentWeapon.FireRate));
        
    }

    public void Reload()
    {
        CanShoot = false;
        anim.SetTrigger("Reload");
        source.PlayOneShot(currentWeapon.ReloadSound);
        StartCoroutine(WaitReload(currentWeapon.ReloadTime));
    }

    public void ShowHitMarker(Color col)
    {
        HitMarker.gameObject.SetActive(true);
        HitMarker.color = col;
        StartCoroutine(HitMaker_Show(0.1f));
    }

    IEnumerator HitMaker_Show(float t)
    {
        yield return new WaitForSeconds(t);
        HitMarker.gameObject.SetActive(false);
    }

    IEnumerator WaitReload(float t)
    {
        yield return new WaitForSeconds(t);

        if (TotalMagLeft <= currentWeapon.Mag)
        {
            int mag = TotalMagLeft;
            int add = (mag -= CurrentMag);
            CurrentMag += add;
            TotalMagLeft -= TotalMagLeft;
        }
        else
        {
            int mag = currentWeapon.Mag;
            int add = (mag -= CurrentMag);
            CurrentMag += add;
            TotalMagLeft -= add;
        }

        int mag2 = currentWeapon.Mag;
        TotalMagLeft = mag2;
        CanShoot = true;
    }

    public void SetWeapon(Weapon w)
    {
        CurrentMag = w.Mag;
        TotalMagLeft = w.Mag;
        anim.runtimeAnimatorController = w.animatorController;

        for (int i = 0; i < gunModels.Length; i++)
        {
            if(gunModels[i].name == w.Name)
            {
                gunModels[i].SetActive(true);
                appearPoint = gunModels[i].transform.Find("Appear");
            }
            else
            {
                gunModels[i].SetActive(false);
            }
        }
    }

    IEnumerator ShootDelay(float t)
    {
        yield return new WaitForSeconds(t);
        CanShoot = true;
    }
}
