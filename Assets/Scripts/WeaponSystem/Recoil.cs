using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Took From this video 
/// https://www.youtube.com/watch?v=geieixA4Mqc&ab_channel=Gilbert
/// </summary>
public class Recoil : MonoBehaviour
{

    private Vector3 currentRotation;
    private Vector3 targetRotation;

    //HipFire Recoil
    public float recoilX;
    public float recoilY;
    public float recoilZ;

    //Settings
    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;

    Vector3 zero;

    // Start is called before the first frame update
    void Start()
    {
        zero = transform.localRotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if(targetRotation != Vector3.zero)
        {
            targetRotation = Vector3.Lerp(targetRotation, zero, returnSpeed * Time.deltaTime);
            currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(currentRotation);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

    }

    public void RecoilFire()
    {
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }
}
