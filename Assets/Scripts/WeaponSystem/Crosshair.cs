using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Cross
{
    public Image img;
    public Vector2 dir;
}

public class Crosshair : MonoBehaviour
{
    public Weapon correspondingWeapon;
    public Cross[] imgs;
    public Color color;
    public float Spread;
    public float DefaultSpread;

    // Start is called before the first frame update
    void Start()
    {
        SetCrossHairs(Spread);
    }

    // Update is called once per frame
    void Update()
    {
        SetCrossHairs(Spread);
        SetCrossHairsColor(color);
    }

    void SetCrossHairs(float s)
    {
        for (int i = 0; i < imgs.Length; i++)
        {
            var img = imgs[i];

            img.img.transform.localPosition = img.dir * (s + DefaultSpread);

        }
    }

    void SetCrossHairsColor(Color col)
    {
        for (int i = 0; i < imgs.Length; i++)
        {
            var img = imgs[i];

            img.img.color = col;

        }
    }

    private void OnValidate()
    {
        SetCrossHairs(Spread);
        SetCrossHairsColor(color);
    }
}
