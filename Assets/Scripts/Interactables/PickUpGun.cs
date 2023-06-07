using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpGun : MonoBehaviour
{
    public Weapon weapon;

    public int MagAmmo;
    public int LeftAmmo;

    // Start is called before the first frame update
    void Start()
    {
        MagAmmo = (int)Random.Range(weapon.MinMaxPickUpGunMag.x, weapon.MinMaxPickUpGunMag.y);
        LeftAmmo = (int)Random.Range(weapon.MinMaxPickUpGunFullMag.x, weapon.MinMaxPickUpGunFullMag.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        Player.instance.gun.PickUpWeapon(this);
        Player.instance.ShowNotification(3f, $"You Picked Up a {weapon.Name}");
        Destroy(gameObject);
    }
}
