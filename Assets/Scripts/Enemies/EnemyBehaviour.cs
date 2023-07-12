using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class EnemyBehaviour : MonoBehaviour
{
    public float Life;

    public NavMeshAgent agent;

    protected Transform target;

    public GameObject Gold;

    public Vector2 GoldGive;


    protected Color startColor;

    public AudioMixer SoundEffectsMixer;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnBulletCollide(Bullet bullet)
    {

    }

    public virtual void OnKnifeCollide(Knife knife, int damage)
    {


    }

    public virtual void GetDamage(int damage)
    {
        Life -= damage;

        if (Life <= 0)
        {
            FindObjectOfType<Gun>().ShowHitMarker(Color.red);
            Die();
        }
        else
        {
            FindObjectOfType<Gun>().ShowHitMarker(Color.black);
        }

    }

    public virtual void Die()
    {
        int coins = Random.Range((int)GoldGive.x, (int)GoldGive.y);

        for (int i = 0; i < coins; i++)
        {
            Vector3 pos = transform.position + (Random.insideUnitSphere * 2f);
            pos.y = transform.position.y;
            var c = Instantiate(Gold, pos, Quaternion.identity).GetComponent<PickUpItem>();
            var r = LevelGenerator.generator.GetRoomInFloor(0, LevelGenerator.generator.CurrentPlayerRoom).GetComponent<LevelRoom>();

            r.Coins.Add(c);
        }
    }


    public virtual IEnumerator ShowDamage(float t, Renderer renderer)
    {
        yield return new WaitForSeconds(t);
        renderer.material.color = startColor;
    }
}
