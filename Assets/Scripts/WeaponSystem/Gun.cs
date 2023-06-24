using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponInfo
{
    public int LeftMag;
    public int LeftTotal;
}

public class Gun : MonoBehaviour
{
    public List<Weapon> weapons;
    public Weapon currentWeapon
    {
        get
        {
            return weapons[CurrentGunIndex];
        }
    }

    public List<WeaponInfo> mags;


    [HideInInspector]
    public int CurrentGunIndex = 0;
    public Camera MyCamera;
    public PlayerMovement player;
    public LayerMask maskToHit;
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
    RaycastHit aimAssistHit;
    Knife thrownKnife;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        SetWeapon(0);

        for (int i = 0; i < weapons.Count; i++)
        {
            mags.Add(new WeaponInfo());
            mags[i].LeftMag = weapons[i].Mag;
            mags[i].LeftTotal = weapons[i].DefaultMaxMag;
        }

    }

    // Update is called once per frame
    void Update()
    {
        ray = MyCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        aimAssistHit = ApplyAimAssist(currentWeapon.KnifeAimAssist);

        if (aimAssistHit.collider != null && currentWeapon.type == Weapon.WeaponType.Melee)
            QuickDraw.DrawCircle(20, 1, aimAssistHit.collider.transform.position, Color.yellow, 0.1f);

        if (currentWeapon.type != Weapon.WeaponType.Melee)
            anim.SetBool("Running", player.Running);
        else
        {
            if (thrownKnife != null)
                anim.SetBool("HasKnife", false);
            else
                anim.SetBool("HasKnife", true);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && currentWeapon.type == Weapon.WeaponType.SemiAuto)
        {
            TryToShoot();
        }

        if(Input.GetKeyDown(KeyCode.Mouse1) && currentWeapon.type == Weapon.WeaponType.Melee && thrownKnife != null)
        {
            GetBackKnife();
        }


        if (Input.GetKey(KeyCode.Mouse0) && currentWeapon.type == Weapon.WeaponType.FullAuto)
        {
            TryToShoot();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && currentWeapon.type == Weapon.WeaponType.Melee)
        {
            TryToShoot();
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f) // forward
        {


            ChangeWeapons(1);
            //if (CurrentGunIndex > weapons.Count)
            //    SetWeapon(0);
            //else
            //    SetWeapon(1);

            //Debug.Log("UP");
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // backwards
        {
            //if (CurrentGunIndex < 0)
            //    SetWeapon(1);
            //else
            //    SetWeapon(0);

            //Debug.Log("DOWN");
            ChangeWeapons(-1);
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

        if (currentWeapon.InfiniteAmmo)
        {
            BulletText.text = $"{mags[CurrentGunIndex].LeftMag} | {currentWeapon.Mag} (Infinite)";

        }
        else
        {
            BulletText.text = $"{mags[CurrentGunIndex].LeftMag} | {mags[CurrentGunIndex].LeftTotal}";

        }

        crosshair.Spread = Mathf.Lerp(crosshair.Spread, 0.0f, SpreadSpeed * Time.deltaTime);

        recoil.recoilX = currentWeapon.NormalRecoil.x;
        recoil.recoilY = currentWeapon.NormalRecoil.y;
        recoil.recoilZ = currentWeapon.NormalRecoil.z;

        crosshair.DefaultSpread = currentWeapon.NormalSpread;

        ShowGunGraphics();
    }

    public void ThrowWeapon(int i)
    {

    }

    public void LateUpdate()
    {
        ShowGunGraphics();
    }

    public void ChangeWeapons(int dir)
    {
        if (dir == -1)
        {
            CurrentGunIndex--;

            if (CurrentGunIndex < 0)
            {
                CurrentGunIndex = weapons.Count - 1;
            }

            SetWeapon(CurrentGunIndex);
        }
        else
        {
            CurrentGunIndex++;

            if (CurrentGunIndex > weapons.Count - 1)
            {
                CurrentGunIndex = 0;
            }

            SetWeapon(CurrentGunIndex);
        }


        //UpdateGun();
    }

    public void PickUpWeapon(PickUpGun gun)
    {
        weapons.Add(gun.weapon);
        mags.Add(new WeaponInfo() { LeftMag = gun.LeftAmmo, LeftTotal = gun.MagAmmo });
    }

    public void ShowGunGraphics()
    {

        for (int i = 0; i < gunModels.Length; i++)
        {
            if (gunModels[i].name == currentWeapon.Name)
            {
                gunModels[i].SetActive(true);

                if (currentWeapon.type != Weapon.WeaponType.Melee)
                    appearPoint = gunModels[i].transform.Find("Appear");
                else
                    appearPoint = gunModels[i].transform;
            }
            else
            {
                gunModels[i].SetActive(false);
            }
        }
    }

    public void TryToShoot()
    {

        if (currentWeapon.type == Weapon.WeaponType.Melee && currentWeapon.Bullet == null)
            return;
        else if(currentWeapon.type == Weapon.WeaponType.Melee && currentWeapon.Bullet != null && CanShoot && mags[CurrentGunIndex].LeftMag > 0)
        {
            StartCoroutine(ThrowKnife(currentWeapon.ShootDELAY));
            return;
        }

        if(CurrentMag <= 0)
        {
            source.PlayOneShot(EmptyMagSound);
        }

        if(CanShoot && mags[CurrentGunIndex].LeftMag > 0 && !player.Running)
        {
            Shoot();
        }


    }

    public RaycastHit ApplyAimAssist(float amount)
    {
        //int layerMask = 1 << LayerMask.NameToLayer("Player");
        Physics.SphereCast(ray.origin, amount, ray.direction, out RaycastHit hit, 100000f, maskToHit);
        return hit;
    }

    public void Shoot()
    {
        var bullet = Instantiate(currentWeapon.Bullet, appearPoint.position, Quaternion.identity).GetComponent<Bullet>();
        var muzzle = Instantiate(Muzzle, appearPoint.position, appearPoint.rotation).GetComponent<Muzzle>();

        bullet.Owner = gameObject;
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

        mags[CurrentGunIndex].LeftMag--;
        CanShoot = false;
        StartCoroutine(ShootDelay(currentWeapon.FireRate));
        //anim.SetBool("HasKnife", true);
    }

    public void Reload()
    {

        if (currentWeapon.type == Weapon.WeaponType.Melee)
            return;


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

    public void GetBackKnife()
    {
        StartCoroutine(WaitForKnife());
    }

    IEnumerator WaitForKnife()
    {
        while(Vector3.Distance(thrownKnife.transform.position, appearPoint.position) >= 0.5f)
        {
            thrownKnife.hitPos = appearPoint.position;
            anim.SetBool("Reclaim", true);
            thrownKnife.Return = true;
            yield return null;
        }
        Destroy(thrownKnife.gameObject);
        anim.SetBool("Reclaim", false);
        anim.SetBool("HasKnife", true);
        mags[CurrentGunIndex].LeftMag = 1;
        CanShoot = true;
    }

    IEnumerator HitMaker_Show(float t)
    {
        yield return new WaitForSeconds(t);
        HitMarker.gameObject.SetActive(false);
    }

    IEnumerator WaitReload(float t)
    {
        yield return new WaitForSeconds(t);

        if (!currentWeapon.InfiniteAmmo)
        {
            var add = currentWeapon.Mag - mags[CurrentGunIndex].LeftMag;

            if (add >= mags[CurrentGunIndex].LeftTotal)
                add = mags[CurrentGunIndex].LeftTotal;

            mags[CurrentGunIndex].LeftTotal -= add;
            mags[CurrentGunIndex].LeftMag += add;
            //int mag2 = currentWeapon.Mag;
            //TotalMagLeft = mag2;
            CanShoot = true;
        }
        else
        {
            var add = currentWeapon.Mag - mags[CurrentGunIndex].LeftMag;

            if (add >= 1000)
                add = 1000;

            mags[CurrentGunIndex].LeftTotal -= add;
            mags[CurrentGunIndex].LeftMag += add;
            //int mag2 = currentWeapon.Mag;
            //TotalMagLeft = mag2;
            CanShoot = true;
        }

    }

    public void SetWeapon(Weapon w)
    {
        CurrentMag = w.Mag;
        TotalMagLeft = w.Mag;
        anim.runtimeAnimatorController = w.animatorController;
        //currentWeapon = w;

        //thrownKnife = null;

    }

    public void SetWeapon(int i)
    {

        SetWeapon(weapons[i]);

        CurrentGunIndex = i;

        //CurrentMag = w.Mag;
        //TotalMagLeft = w.Mag;
        //anim.runtimeAnimatorController = w.animatorController;

        //for (int i = 0; i < gunModels.Length; i++)
        //{
        //    if (gunModels[i].name == w.Name)
        //    {
        //        gunModels[i].SetActive(true);

        //        if (currentWeapon.type != Weapon.WeaponType.Melee)
        //            appearPoint = gunModels[i].transform.Find("Appear");
        //        else
        //            appearPoint = gunModels[i].transform;
        //    }
        //    else
        //    {
        //        gunModels[i].SetActive(false);
        //    }
        //}
    }

    IEnumerator ShootDelay(float t)
    {
        yield return new WaitForSeconds(t);
        CanShoot = true;
    }

    IEnumerator ThrowKnife(float t)
    {
        anim.SetTrigger("Shoot");

        mags[CurrentGunIndex].LeftMag--;
        CanShoot = false;
        yield return new WaitForSeconds(t);


        //ray = MyCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        //aimAssistHit = ApplyAimAssist(currentWeapon.KnifeAimAssist);


        var bullet = Instantiate(currentWeapon.Bullet, appearPoint.position, appearPoint.rotation).GetComponent<Knife>();
        //anim.SetBool("HasKnife", false);
        //var muzzle = Instantiate(Muzzle, appearPoint.position, appearPoint.rotation).GetComponent<Muzzle>();

        //Debug.Break();
        source.PlayOneShot(currentWeapon.ShootingSound);

        thrownKnife = bullet;

        EZCameraShake.CameraShaker.Instance.ShakeOnce(currentWeapon.Magnitude, currentWeapon.Rougness, currentWeapon.fadeIn, currentWeapon.fadeout);

        crosshair.Spread += currentWeapon.AddSpread;
        recoil.RecoilFire();
        //muzzle.Restart();

        //Debug.Break();

        if (aimAssistHit.collider == null)
        {
            bullet.ShootRb(ray.direction);
            Debug.Log("No Target");
        }
        else
        {
            Debug.DrawLine(transform.position, aimAssistHit.point, Color.red, 5f);
            bullet.ShootTransformTarget(aimAssistHit.transform);
            Debug.Log($"Target : {aimAssistHit.collider.name}");
        }

        anim.SetBool("HasKnife", false);
        anim.ResetTrigger("Shoot");
        //StartCoroutine(ShootDelay(currentWeapon.FireRate));

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(ray.origin, ray.direction * 100);
    }
}
