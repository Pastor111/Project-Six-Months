using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LevelRoom : MonoBehaviour
{
    public enum Exit {Top, Bottom, Left, Right}

    public Exit[] exits;

    public GameObject Button;

    public GameObject LeftWall;
    public GameObject RightWall;
    public GameObject TopWall;
    public GameObject BottomWall;
    public GameObject Door;

    public GameObject[] LevelLayouts;

    public LevelRoom Top;
    public LevelRoom Bottom;
    public LevelRoom Left;
    public LevelRoom Right;

    public List<GameObject> Doors;
    public List<GameObject> Enemies;

    public Bounds bounds;

    bool wasPlayerHereLastFrame;
    bool HasBeatenRoom = false;
    bool EnemiesWereSpawned = false;

    public bool CanSpawnEnemies = false;

    bool IsPlayerInside
    {
        get
        {
            return bounds.Contains(PlayerMovement.instance.transform.position);
        }
    }


    // Start is called before the first frame update
    void Start()
    {

        Button.GetComponentInChildren<Interactable>().action.AddListener(LevelGenerator.generator.ReloadLevel);



    }

    public void GenerateBounds()
    {
        var b1 = QuickDraw.JoinBounds(LeftWall.GetComponent<Renderer>().bounds, RightWall.GetComponent<Renderer>().bounds);
        var b2 = QuickDraw.JoinBounds(TopWall.GetComponent<Renderer>().bounds, BottomWall.GetComponent<Renderer>().bounds);

        bounds = QuickDraw.JoinBounds(b1, b2);
        bounds = new Bounds(bounds.center, bounds.size * 0.9f);
    }

    // Update is called once per frame
    void Update()
    {

        if (!CanSpawnEnemies)
            return;

        if (LevelGenerator.generator.FirstRoom == gameObject)
            ForceLayout(0);

        if (LevelGenerator.generator.FirstRoom != null && transform == LevelGenerator.generator.FirstRoom.transform)
            return;

        if (IsPlayerInside && !wasPlayerHereLastFrame && !HasBeatenRoom && !EnemiesWereSpawned /*(Enemies == null || Enemies.Count <= 0)*/)
        {
            CloseDoors();
            SpawnEnemies();
            //StartCoroutine(Test());
        }

        for (int i = 0; i < Enemies.Count; i++)
        {
            if(Enemies[i] == null)
            {
                Enemies.RemoveAt(i);
            }
        }

        if (IsPlayerInside)
        {
            if(!HasBeatenRoom)
            {
                CloseDoors();
            }

        }
        if (IsPlayerInside && wasPlayerHereLastFrame && Enemies.Count <= 0 && EnemiesWereSpawned)
        {
            OpenDoors();
            HasBeatenRoom = true;

            if (LevelGenerator.generator.LastRoom != null && transform == LevelGenerator.generator.LastRoom.transform)
                Button.SetActive(true);
            else
                Button.SetActive(false);

        }


        //QuickDraw.DrawBounds(bounds);

        wasPlayerHereLastFrame = IsPlayerInside;

    }

    public void ForceLayout(int i)
    {

        LevelLayouts[i].SetActive(true);

        for (int x = 0; x < LevelLayouts.Length; x++)
        {
            if (x != i)
                LevelLayouts[x].SetActive(false);
        }
    }

    public void ChooseLevelLayout()
    {
        int i = Random.Range(0, LevelLayouts.Length - 1);

        LevelLayouts[i].SetActive(true);

        for (int x = 0; x < LevelLayouts.Length; x++)
        {
            if (x != i)
                LevelLayouts[x].SetActive(false);
        }
    }

    IEnumerator WaitSpawn(float t)
    {
        yield return new WaitForSeconds(t);
        int amount = Random.Range(1, 8);

        for (int i = 0; i < amount; i++)
        {
            var pos = RandomPointInBounds(bounds);

            pos.y = 5;

            var enemy = Instantiate(LevelGenerator.generator.AvailableEnemies[Random.Range(0, LevelGenerator.generator.AvailableEnemies.Length - 1)].gameObject, pos, Quaternion.identity);

            Enemies.Add(enemy);
        }

        EnemiesWereSpawned = true;
    }

    public void SpawnEnemies()
    {
        StartCoroutine(WaitSpawn(1));
    }

    public static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    IEnumerator Test()
    {
        yield return new WaitForSeconds(2);
        OpenDoors();
    }

    public void OpenDoors()
    {
        for (int i = 0; i < Doors.Count; i++)
        {
            Doors[i].transform.GetChild(0).transform.localPosition = new Vector3(-20.6323f, Doors[i].transform.GetChild(0).transform.localPosition.y, Doors[i].transform.GetChild(0).transform.localPosition.z);
            Doors[i].transform.GetChild(1).transform.localPosition = new Vector3(28.1962f, Doors[i].transform.GetChild(1).transform.localPosition.y, Doors[i].transform.GetChild(1).transform.localPosition.z);
        }
    }

    public void CloseDoors()
    {
        for (int i = 0; i < Doors.Count; i++)
        {
            Doors[i].transform.GetChild(0).transform.localPosition = new Vector3(-16.6323f, Doors[i].transform.GetChild(0).transform.localPosition.y, Doors[i].transform.GetChild(0).transform.localPosition.z); 
            Doors[i].transform.GetChild(1).transform.localPosition = new Vector3(24.1962f, Doors[i].transform.GetChild(1).transform.localPosition.y, Doors[i].transform.GetChild(1).transform.localPosition.z); 
        }
    }

    public void UpdateDoors()
    {
        for (int i = 0; i < exits.Length; i++)
        {
            if(exits[i] == Exit.Top)
            {
                TopWall.SetActive(false);
                var d = Instantiate(Door, TopWall.transform.position, TopWall.transform.rotation, transform);
                d.SetActive(true);
                Doors.Add(d);
            }

            if (exits[i] == Exit.Bottom)
            {
                BottomWall.SetActive(false);
                var d = Instantiate(Door, BottomWall.transform.position, BottomWall.transform.rotation, transform);
                d.SetActive(true);
                Doors.Add(d);
            }

            if (exits[i] == Exit.Left)
            {
                LeftWall.SetActive(false);
                var d = Instantiate(Door, LeftWall.transform.position, LeftWall.transform.rotation, transform);
                d.SetActive(true);
                Doors.Add(d);
            }

            if (exits[i] == Exit.Right)
            {
                RightWall.SetActive(false);
                var d = Instantiate(Door, RightWall.transform.position, RightWall.transform.rotation, transform);
                d.SetActive(true);
                Doors.Add(d);
            }
        }
    }
}
