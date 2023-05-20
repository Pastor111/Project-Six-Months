using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "New Weapon")]
public class Weapon : ScriptableObject
{
    public string Name;
    public enum WeaponType {Melee, SemiAuto, FullAuto}
    public WeaponType type;

    public GameObject Bullet;
    public int Mag;

    public float FireRate;
    public float ReloadTime;
    public AudioClip ShootingSound;
    public AudioClip ReloadSound;

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

    public RuntimeAnimatorController animatorController;
}
