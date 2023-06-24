using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public Vector3 Rotation;
    public int AmountToFill;
    public float MovementAmplitude;
    public enum Fill {Health, Bullets, Keys, Coins}
    public Fill fill;
    public enum Quality {Low, Medium, High}
    public Quality quality;

    float startY;

    // Start is called before the first frame update
    void Start()
    {
        startY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Rotation * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, startY + (Mathf.Sin(Time.time) * MovementAmplitude), transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            if (fill == Fill.Health)
            {
                Player.instance.AddLife(AmountToFill);
                Player.instance.ShowNotification(2.5f, $"Life +{AmountToFill}");
            }
            else if(fill == Fill.Keys)
            {
                Player.instance.Keys++;
                Player.instance.ShowNotification(2.5f, $"Keys +{1}");
            }
            else if(fill == Fill.Coins)
            {
                Player.instance.SetGold(Player.instance.GetGold() + 1);
                Player.instance.ShowNotification(2.5f, $"Gold +{1}");
            }
            else
            {
                int max = (int)Player.instance.gun.currentWeapon.MinMaxPickUpGunFullMag.x;

                if (quality == Quality.Low)
                    max += (int)Player.instance.gun.currentWeapon.MinMaxPickUpGunFullMag.y / 4;
                else if(quality == Quality.Medium)
                    max += (int)Player.instance.gun.currentWeapon.MinMaxPickUpGunFullMag.y / 2;
                else
                    max = (int)Player.instance.gun.currentWeapon.MinMaxPickUpGunFullMag.y;

                int add = (int)Random.Range(Player.instance.gun.currentWeapon.MinMaxPickUpGunFullMag.x, max);
                Player.instance.gun.mags[Player.instance.gun.CurrentGunIndex].LeftTotal += add;
                Player.instance.ShowNotification(2.5f, $"Bullets +{add}");
            }
        }
    }
}
