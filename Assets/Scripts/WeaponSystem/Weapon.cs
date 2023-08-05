using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "New Weapon")]
public class Weapon : ScriptableObject
{
    public string Name;
    public enum WeaponType {Melee, SemiAuto, FullAuto}
    public WeaponType type;
    public bool InfiniteAmmo;

    public float ShootDELAY;
    public float KnifeAimAssist = 2;
    public GameObject Bullet;
    public GameObject Enemy_Bullet;
    public int Mag;
    public int DefaultMaxMag;

    public int Damage = 1;


    public float FireRate;
    public float ReloadTime;
    public AudioClip ShootingSound;
    public AudioClip ReloadSound;

    [Space]
    [Space]
    [Space]
    [Header("Pick Up")]
    public Vector2 MinMaxPickUpGunMag;
    public Vector2 MinMaxPickUpGunFullMag;

    [Space]
    [Space]
    [Space]
    [Header("Spread")]
    public float NormalSpread;
    //public float AimSpread;
    public float AddSpread;

    [Space]
    [Space]
    [Space]
    [Header("Recoil")]
    public Vector3 NormalRecoil;
    public Vector3 AimRecoil;

    [Space]
    [Space]
    [Space]
    [Header("Shake")]
    public float Magnitude;
    public float Rougness;
    public float fadeIn;
    public float fadeout;
    [Space]
    [Space]
    [Space]
    [Header("Melee")]
    public int AttackTimesUpdate;
    public float WindUpTime;
    public float AttackRadius;
    public float AttackTime;
    public float SlowMoTime;

    public RuntimeAnimatorController animatorController;
}
