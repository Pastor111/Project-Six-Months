using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muzzle : MonoBehaviour
{
    public float MinSize;
    public float MaxSize;

    public enum Dimension {TwoD, ThreeD };
    public Dimension dimension;

    public Color MinColor;
    public Color MaxColor;

    public float ShrinkSpeed;

    // Start is called before the first frame update
    void Start()
    {
        Restart();

    }

    public void Restart()
    {
        if (dimension == Dimension.TwoD)
            GetComponent<SpriteRenderer>().color = Color.Lerp(MinColor, MaxColor, Random.Range(0f, 1f));

        var size = Random.Range(MinSize, MaxSize);
        var rot = Random.Range(0, 360);

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + rot);

        if (dimension == Dimension.TwoD)
            transform.localScale = new Vector3(size, size, 1);
        else
            transform.localScale = new Vector3(size, size, size);

        StartCoroutine(AwakeTime());
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localScale.x >= 0)
        {
            var plus = ShrinkSpeed * Time.deltaTime;
            transform.localScale -= Vector3.one * plus;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator AwakeTime()
    {
        yield return new WaitForSeconds(.1f);
        gameObject.SetActive(false);
    }
}
