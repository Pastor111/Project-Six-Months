using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed;
    public GameObject explosion;
    [HideInInspector]
    public GameObject Owner;
    Vector3 hitPos;
    Rigidbody rb;
    Vector3 lastPos;
    Vector3 firstPos;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position;
        firstPos = lastPos;
        if(rb == null)
            rb = GetComponent<Rigidbody>();
        //firstPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (hitPos != Vector3.zero)
        {
            transform.position = Vector3.MoveTowards(transform.position, hitPos, Speed * Time.deltaTime);

            if (transform.position == hitPos)
            {
                Destroy(gameObject);

            
            }





        }

        if (Physics.Linecast(lastPos, transform.position, out RaycastHit hit))
        {
            transform.position = hit.point;

            if (hit.collider.GetComponent<EnemyBehaviour>())
            {
                EnemyBehaviour enemy = hit.collider.GetComponent<EnemyBehaviour>();
                enemy.OnBulletCollide(this);
            }

            //Debug.Log(hit.transform.name);
            Destroy(gameObject);
        }

        lastPos = transform.position;
    }

    private void OnDestroy()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Debug.DrawLine(firstPos, transform.position, Color.black, 5f);
    }


    public void ShootTransform(Vector3 hit)
    {
        hitPos = hit;
    }

    public void ShootRb(Vector3 dir)
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = dir * Speed;
    }
}
