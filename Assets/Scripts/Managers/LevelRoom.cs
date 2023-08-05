using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LevelRoom : MonoBehaviour
{

    public enum RoomType {Normal, Store, Chest, Boss, Gambling, Elevator}

    public enum Exit {Top, Bottom, Left, Right}

    public RoomType roomType;
    [Space]
    [Space]
    [Space]

    public Exit[] exits;

    public GameObject Button;

    public int ActiveLayout;

    public GameObject LeftWall;
    public GameObject RightWall;
    public GameObject TopWall;
    public GameObject BottomWall;
    public GameObject Door;

    public GameObject Ceiling;
    public GameObject Floor;
    public GameObject OpenFloor;



    public GameObject[] LevelLayouts;
    public GameObject[] LevelLayouts_Friendly;
    public GameObject Elevator;

    public LevelRoom UpperLevel;
    public LevelRoom SubLevel;
    public LevelRoom Top;
    public LevelRoom Bottom;
    public LevelRoom Left;
    public LevelRoom Right;

    public List<PickUpItem> Coins;


    public List<GameObject> _Paths;

    public List<GameObject> Doors;
    public List<GameObject> Enemies;

    public List<GameObject> EnemySpawns; 

    public Bounds bounds;

    bool wasPlayerHereLastFrame;
    bool HasBeatenRoom = false;
    bool EnemiesWereSpawned = false;
    bool HasGivenReward = false;

    public bool CanSpawnEnemies = false;

    bool IsPlayerInside
    {
        get
        {
            return bounds.Contains(Player.instance.transform.position);
        }
    }

    bool isSpawning = false;


    // Start is called before the first frame update
    void Start()
    {

        Button.GetComponentInChildren<Interactable>().action.AddListener(LevelGenerator.generator.ReloadLevel);



    }

    public void GenerateBounds()
    {

        var b1 = QuickDraw.JoinBounds(GetBounds(LeftWall), GetBounds(RightWall));
        var b2 = QuickDraw.JoinBounds(GetBounds(TopWall), GetBounds(BottomWall));

        bounds = QuickDraw.JoinBounds(b1, b2);
        bounds = new Bounds(bounds.center, bounds.size * 0.9f);
    }

    public static Bounds GetBounds(GameObject obj)
    {
        if (obj.GetComponent<Renderer>() != null)
            return obj.GetComponent<Renderer>().bounds;

        Bounds b = new Bounds();

        for (int i = 0; i < obj.transform.childCount; i++)
        {
            if(obj.transform.GetChild(i).GetComponent<Renderer>() != null)
            {
                b = QuickDraw.JoinBounds(b, obj.transform.GetChild(i).GetComponent<Renderer>().bounds);
            }
        }

        return b;
    }

    // Update is called once per frame
    void Update()
    {

        if (LevelGenerator.generator.FirstRoom == null)
        {

        }
        else
        {


            if (IsPlayerInside)
            {
                LevelGenerator.generator.UpdatePlayerFieldOfView(int.Parse(gameObject.name));
            }


            if (transform == LevelGenerator.generator.FirstRoom.transform || roomType == RoomType.Elevator || roomType == RoomType.Store)
            {

                ForceLayout(0, roomType == RoomType.Elevator);
                return;
            }
            else
            {


                if (!CanSpawnEnemies)
                    return;

                for (int i = 0; i < Coins.Count; i++)
                {
                    if (Coins[i] == null)
                        Coins.RemoveAt(i);
                }

                if (IsPlayerInside && !wasPlayerHereLastFrame && !HasBeatenRoom && !EnemiesWereSpawned && !isSpawning /*(Enemies == null || Enemies.Count <= 0)*/)
                {

                    CloseDoors();
                    SpawnEnemies();
                    DynamicAudioManager.instance.state = DynamicAudioManager.State.Fighting;
                    //StartCoroutine(Test());
                }



                for (int i = 0; i < Enemies.Count; i++)
                {
                    if (Enemies[i] == null)
                    {
                        Enemies.RemoveAt(i);
                    }
                }

                if (IsPlayerInside)
                {
                    if (!HasBeatenRoom)
                    {
                        CloseDoors();
                    }

                }
                if (IsPlayerInside && wasPlayerHereLastFrame && Enemies.Count <= 0 && EnemiesWereSpawned)
                {
                    OpenDoors();

                    float CoinSpeed = 30f;
                    for (int i = 0; i < Coins.Count; i++)
                    {
                        Coins[i].transform.position = Vector3.MoveTowards(Coins[i].transform.position, Player.instance.transform.position, CoinSpeed * Time.deltaTime);
                    }
                    DynamicAudioManager.instance.state = DynamicAudioManager.State.Calm;
        
                    if (!HasGivenReward)
                    {
                        BakeRefelections();

                        HasGivenReward = true;

                        //Player.instance.SetGold(Player.instance.GetGold() + Random.Range(5, 20));

                        int r = Random.Range(0, 100);

                        if (r >= 40)
                        {

                            HasGivenReward = true;
                            var layout = LevelLayouts[ActiveLayout];

                            var chests = layout.transform.GetComponentsInChildren<Chest>(true);

                            int i = Random.Range(0, 100);
                            int x = Random.Range(0, chests.Length);

                            GameObject chest = null;

                            foreach (ItemProbability item in LevelGenerator.generator.AvailableObjectsToReward)
                            {
                                if (i >= item.Probability.x && i <= item.Probability.y)
                                {
                                    chest = Instantiate(item.obj, chests[x].transform.position, chests[x].transform.rotation, transform);
                                }
                            }



                            var icon = MiniMapManager.GetMiniMap().PlaceItemInMiniMap(int.Parse(transform.name), MiniMapManager.GetMiniMap().ChestIcon, Color.white, new Vector3(0.2f, 0.2f));

                            if (chest.GetComponent<Chest>() != null)
                            {
                                chest.GetComponent<Chest>().Icon = icon;
                            }
                        }

                        HasGivenReward = true;
                    }

                    HasBeatenRoom = true;

                    if (LevelGenerator.generator.LastRoom != null && transform == LevelGenerator.generator.LastRoom.transform)
                        Button.SetActive(true);
                    else
                        Button.SetActive(false);

                }


                //QuickDraw.DrawBounds(bounds);

                wasPlayerHereLastFrame = IsPlayerInside;
            }

        }

    }

    public Vector3 GetFreePosition()
    {
        var layout = LevelLayouts[ActiveLayout];

        var chests = layout.transform.GetComponentsInChildren<Chest>(true);

        //int i = Random.Range(0, 100);
        int x = Random.Range(0, chests.Length);

        return chests[x].transform.position;
    }

    public void ForceLayout(int i, bool isElevator = false)
    {
        LevelLayouts[i].SetActive(true);

        ActiveLayout = i;

       
        if (isElevator)
        {
            Elevator.SetActive(true);
            UpperLevel.ForceLayout(0, false);
        }

        //Floor = LevelLayouts[ActiveLayout].transform.Find("Floor").gameObject;

        foreach (Transform child in LevelLayouts[i].transform)
        {
            if (child.name.Contains("Spawn"))
            {
                EnemySpawns.Add(child.gameObject);
                child.gameObject.SetActive(false);
            }
        }

        for (int x = 0; x < LevelLayouts.Length; x++)
        {
            if (x != i)
                LevelLayouts[x].SetActive(false);
        }
    }

    public void ChooseLevelLayout()
    {
        int i = 0;

        if (UpperLevel != null || SubLevel != null)
        {

            i = Random.Range(0, LevelLayouts_Friendly.Length);

            LevelLayouts_Friendly[i].SetActive(true);

            ActiveLayout = i;

            //if (UpperLevel != null && (i != 1 && i != 2))
            //{
            //    UpperLevel.ForceLayout(0);
            //    Elevator.SetActive(true);
            //}

            foreach (Transform child in LevelLayouts_Friendly[i].transform)
            {
                if (child.name.Contains("Spawn"))
                {
                    EnemySpawns.Add(child.gameObject);
                    child.gameObject.SetActive(false);
                }
            }

            for (int x = 0; x < LevelLayouts.Length; x++)
            {
                if (LevelLayouts_Friendly[i] != LevelLayouts[x])
                    LevelLayouts[x].SetActive(false);
            }

            BakeRefelections();

            //Floor = LevelLayouts[ActiveLayout].transform.Find("Floor").gameObject;

            return;
        }
 

        i = Random.Range(0, LevelLayouts.Length);

        LevelLayouts[i].SetActive(true);

        ActiveLayout = i;

        //Floor = LevelLayouts[ActiveLayout].transform.Find("Floor").gameObject;
        //if (UpperLevel != null && (i != 1 && i != 2))
        //{
        //    UpperLevel.ForceLayout(0);
        //    Elevator.SetActive(true);
        //}

        foreach (Transform child in LevelLayouts[i].transform)
        {
            if (child.name.Contains("Spawn"))
            {
                EnemySpawns.Add(child.gameObject);
                child.gameObject.SetActive(false);
            }
        }

        for (int x = 0; x < LevelLayouts.Length; x++)
        {
            if (x != i)
                LevelLayouts[x].SetActive(false);
        }

        BakeRefelections();
    }

    void BakeRefelections()
    {
        if(transform.Find("Reflection") == null)
        {

            GameObject Reflection = new GameObject("Reflection");
            Reflection.transform.position = bounds.center;
            Reflection.transform.parent = transform;
            var probe = Reflection.AddComponent<ReflectionProbe>();
            probe.mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
            probe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
            probe.renderDynamicObjects = true;
            probe.size = bounds.size * 2.5f;
            probe.resolution = 256;
            probe.boxProjection = true;
            probe.RenderProbe();
        }
        else
        {
            var a = transform.Find("Reflection").GetComponent<ReflectionProbe>();
            a.RenderProbe();
        }

    }

    IEnumerator WaitSpawn(float t, float t2)
    {
        yield return new WaitForSeconds(t);
        int amount = Random.Range((int)LevelGenerator.generator.MinMaxEnemyNumber.x, (int)LevelGenerator.generator.MinMaxEnemyNumber.y);

        Transform[] pos = new Transform[amount];

        for (int i = 0; i < amount; i++)
        {

            int x = Random.Range(0, EnemySpawns.Count);

            pos[i] = EnemySpawns[x].transform;

            EnemySpawns[x].SetActive(true);
            EnemySpawns[x].GetComponent<ParticleSystem>().Stop();
            EnemySpawns[x].GetComponent<ParticleSystem>().Play();

            //EnemySpawns.Remove(EnemySpawns[x]);
        }

        yield return new WaitForSeconds(t2);

        for (int i = 0; i < amount; i++)
        {

            Destroy(pos[i].gameObject);

            var enemy = Instantiate(LevelGenerator.generator.AvailableEnemies[Random.Range(0, LevelGenerator.generator.AvailableEnemies.Length)].gameObject, pos[i].position, Quaternion.identity);

            Enemies.Add(enemy);
        }

        EnemiesWereSpawned = true;
        isSpawning = false;
    }

    public void SpawnEnemies()
    {
        isSpawning = true;
        StartCoroutine(WaitSpawn(1, 2));
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
            Doors[i].transform.GetComponentInChildren<Animator>().SetBool("Open", true);
        }
    }

    public void CloseDoors()
    {
        for (int i = 0; i < Doors.Count; i++)
        {
            Doors[i].transform.GetComponentInChildren<Animator>().SetBool("Open", false);
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

        if(UpperLevel != null)
        {
            Ceiling.SetActive(false);
            var d = Instantiate(OpenFloor, Ceiling.transform.position, Ceiling.transform.rotation, transform);
            d.SetActive(true);
        }

        if (SubLevel != null)
        {
            //Floor = LevelLayouts[ActiveLayout].transform.Find("Floor").gameObject;
            Floor.SetActive(false);
            var d = Instantiate(OpenFloor, Floor.transform.position, Floor.transform.rotation, transform);
            d.SetActive(true);
        }
    }
}
