using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class Knife : MonoBehaviour
{
    public float ReturnSpeed;
    public float Speed;
    public Vector3 rotDir;
    public float RotateSpeed;
    public Transform sliceNormal;
    public float AimAssist;
    public LayerMask maskToHit;

    public bool Return;

    [HideInInspector]
    public Vector3 hitPos;
    Rigidbody rb;
    Vector3 lastPos;
    Vector3 firstPos;
    Transform followTarget;
    float t;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position;
        firstPos = lastPos;
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Return)
        {
            transform.parent = null;
            //t = Mathf.MoveTowards(t, 1.0f, ReturnSpeed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, hitPos, ReturnSpeed * Time.deltaTime);
            //transform.position = Vector3.Slerp(transform.position, Player.instance.transform.position, t);
            //MoveInArc(transform.position, Player.instance.transform.position, 2f, ReturnSpeed);
            if (transform.position == hitPos)
            {
                //Destroy(gameObject);


            }
            else
            {
                transform.Rotate(rotDir * RotateSpeed * Time.deltaTime);
            }


        }
        else
        {


            t = 0.0f;

            if (followTarget != null)
            {
                hitPos = followTarget.position;
            }

            if (hitPos != Vector3.zero)
            {
                transform.position = Vector3.MoveTowards(transform.position, hitPos, Speed * Time.deltaTime);

                if (transform.position == hitPos)
                {
                    //Destroy(gameObject);


                }
                else
                {
                    transform.Rotate(rotDir * RotateSpeed * Time.deltaTime);
                }





            }

        }


        if (!Return)
        {

            if (Physics.Linecast(lastPos, transform.position, out RaycastHit hit, maskToHit))
            {
                Debug.Log(hit.transform.name);
                rb.velocity = Vector3.zero;
                Ray r = new Ray(transform.position, (transform.position - lastPos));

                transform.position = hit.point - (r.direction * 0.2f);
            }

        }

        var coll = Physics.OverlapSphere(transform.position, AimAssist, maskToHit);

        if (coll.Length > 0)
        {
            //transform.position = hit.point;

            if (coll[0].GetComponent<EnemyBehaviour>() != null)
            {
                EnemyBehaviour enemy = coll[0].GetComponent<Collider>().GetComponent<EnemyBehaviour>();


                EzySlice.Plane cuttingPlane = new EzySlice.Plane();

                // the plane will be set to the same coordinates as the object that this
                // script is attached to
                // NOTE -> Debug Gizmo drawing only works if we pass the transform
                cuttingPlane.Compute(sliceNormal);



                var a = enemy.gameObject.Slice(transform.position, sliceNormal.forward, enemy.GetComponent<Renderer>().material);
                var part_a = a.CreateLowerHull();
                var part_b = a.CreateUpperHull();

                part_a.GetComponent<Renderer>().material = enemy.GetComponent<Renderer>().material;
                part_b.GetComponent<Renderer>().material = enemy.GetComponent<Renderer>().material;

                part_a.GetComponent<Renderer>().material.color = Color.red;
                part_b.GetComponent<Renderer>().material.color = Color.red;

                part_a.transform.position = enemy.transform.position;
                part_b.transform.position = enemy.transform.position;
                hitPos = Vector3.zero;

                transform.SetParent(part_a.transform);

                part_a.AddComponent<MeshCollider>().sharedMesh = a.lowerHull;
                part_a.GetComponent<MeshCollider>().convex = true;
                part_a.AddComponent<Rigidbody>();


                part_b.AddComponent<MeshCollider>().sharedMesh = a.upperHull;
                part_b.GetComponent<MeshCollider>().convex = true;
                part_b.AddComponent<Rigidbody>();

                part_a.GetComponent<Rigidbody>().AddExplosionForce(10000, enemy.transform.position, 10);
                part_a.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                part_b.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                part_b.GetComponent<Rigidbody>().AddExplosionForce(10000, enemy.transform.position, 10);

                enemy.OnKnifeCollide(this, 100);



            }

            //Debug.Log(coll[0].transform.name);
            //Debug.Break();
            //Destroy(gameObject);
        }


        lastPos = transform.position;
    }


    public void ShootTransform(Vector3 hit)
    {
        lastPos = transform.position;
        firstPos = lastPos;
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        hitPos = hit;

   
    }

    public void ShootTransformTarget(Transform hit)
    {
        lastPos = transform.position;
        firstPos = lastPos;

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        followTarget = hit;
        hitPos = hit.position;
    }

    public void ShootRb(Vector3 dir)
    {

        lastPos = transform.position;
        firstPos = lastPos;
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb = GetComponent<Rigidbody>();

        rb.velocity = dir * Speed;
    }

#if UNITY_EDITOR
    /**
	 * This is for Visual debugging purposes in the editor 
	 */
    public void OnDrawGizmos()
    {
        EzySlice.Plane cuttingPlane = new EzySlice.Plane();


        Gizmos.DrawWireSphere(transform.position, AimAssist);

        // the plane will be set to the same coordinates as the object that this
        // script is attached to
        // NOTE -> Debug Gizmo drawing only works if we pass the transform
        cuttingPlane.Compute(sliceNormal);

        // draw gizmos for the plane
        // NOTE -> Debug Gizmo drawing is ONLY available in editor mode. Do NOT try
        // to run this in the final build or you'll get crashes (most likey)
        cuttingPlane.OnDebugDraw();
    }

#endif
}
