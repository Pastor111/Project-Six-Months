using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemProbability
{
    public GameObject obj;
    public Vector2 Probability;
}

public class Chest : MonoBehaviour
{
    public bool IsOpenned = false;
    public UnityEngine.Audio.AudioMixer SoundEffectMixer;

    public Interactable interactable;
    public enum ChestType {Bad, Normal, Special}
    public ChestType type;
    public GameObject ClosedObj;
    public GameObject OpennedObj;
    public Transform AppearPoint;
    public ItemProbability[] ObjectsToSpawn;
    public GameObject Icon;
    public AudioClip OpenChest;
    //public Vector3 PushForce;

    public Vector2 MinMaxGold;

    Renderer[] renderers;

    // Start is called before the first frame update
    void Start()
    {
        renderers = transform.GetComponentsInChildren<Renderer>();

        if (type == ChestType.Bad)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].material.color.a == 1)
                    renderers[i].material.color = Color.green;
            }
        }
        else if (type == ChestType.Normal)
        {
            for (int i = 0; i < renderers.Length; i++)
            {

                if(renderers[i].material.color.a == 1)
                    renderers[i].material.color = Color.yellow;
            }
        }
        else
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i].material.color.a == 1)
                    renderers[i].material.color = Color.red;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        OpennedObj.SetActive(IsOpenned);
        ClosedObj.SetActive(!IsOpenned);


    }

    public void Open()
    {

        if (type == ChestType.Special && Player.instance.Keys <= 0)
        {
            Player.instance.ShowNotification(2.5f, "You Need a Key to open this chest");
            return;
        }
        else if(type == ChestType.Special && Player.instance.Keys > 0)
        {
            Player.instance.Keys--;
        }

        AudioManager.PlaySound2D(OpenChest, false, 1.0f, 0, 1, SoundEffectMixer);
        IsOpenned = true;
        Destroy(interactable);

        gameObject.layer = LayerMask.NameToLayer("Default");
        Destroy(gameObject);
        Destroy(Icon.gameObject);
        GrantReward();

    }

    public void GrantReward()
    {
        int i = Random.Range(0, 4);

        if(i == 0)
        {
            int plus = (int)Random.Range(MinMaxGold.x, MinMaxGold.y);
            Player.instance.SetGold(Player.instance.GetGold() + plus);
            Player.instance.ShowNotification(2f, $"Gold +{plus}");
        }
        else if(i == 1)
        {
            Player.instance.Keys++;
            Player.instance.ShowNotification(2f, $"Key +{1}");
        }
        else
        {
            //Player.instance.;
            //Player.instance.ShowNotification(2f, $"Nothing Better luck next time");
            int x = Random.Range(0, 100);

            foreach (ItemProbability item in ObjectsToSpawn)
            {
                if(x >= item.Probability.x && x <= item.Probability.y)
                {
                    Instantiate(item.obj, transform.position, item.obj.transform.localRotation);
                }
            }

            //var obj = Instantiate(ObjectsToSpawn[x], AppearPoint.position, Quaternion.identity);

            //if(obj.GetComponent<Rigidbody>() != null)
            //{
            //    obj.GetComponent<Rigidbody>().AddForce(PushForce);
            //}
        }

    }
}
