using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public InteractableButton button;
    public float Speed;
    public Vector3 UpPos;
    public Vector3 DownPos;
    bool Moving = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Moving)
        {
            button.Active = false;
        }
        else
        {
            button.Active = true;
        }
    }

    public void Use()
    {
        if(!Moving)
        {
            Moving = true;
            if(transform.localPosition == DownPos)
            {
                StartCoroutine(Move(UpPos));
            }
            else
            {
                StartCoroutine(Move(DownPos));
            }
        }
    }

    IEnumerator Move(Vector3 goPos)
    {
        while(transform.localPosition != goPos)
        {
            Player.instance.transform.parent = transform;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, goPos, Speed * Time.deltaTime);
            Moving = true;
            yield return null;
        }

        Player.instance.transform.parent = null;

        Moving = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(UpPos, Vector3.one);
        Gizmos.DrawWireCube(DownPos, Vector3.one);
    }
}
