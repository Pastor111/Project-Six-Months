using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace GridPathFinder
{

    /// A unility class with functions to scale Texture2D Data.
    ///
    /// Scale is performed on the GPU using RTT, so it's blazing fast.
    /// Setting up and Getting back the texture data is the bottleneck.
    /// But Scaling itself costs only 1 draw call and 1 RTT State setup!
    /// WARNING: This script override the RTT Setup! (It sets a RTT!)  
    ///
    /// Note: This scaler does NOT support aspect ratio based scaling. You will have to do it yourself!
    /// It supports Alpha, but you will have to divide by alpha in your shaders,
    /// because of premultiplied alpha effect. Or you should use blend modes.
    public class GPUTextureScaler
    {
        /// <summary>
        ///     Returns a scaled copy of given texture.
        /// </summary>
        /// <param name="tex">Source texure to scale</param>
        /// <param name="width">Destination texture width</param>
        /// <param name="height">Destination texture height</param>
        /// <param name="mode">Filtering mode</param>
        public static Texture2D Scaled(Texture2D src, int width, int height, FilterMode mode = FilterMode.Trilinear)
        {
            Rect texR = new(0, 0, width, height);
            _gpu_scale(src, width, height, mode);

            //Get rendered data back to a new texture
            Texture2D result = new(width, height, TextureFormat.ARGB32, true);
            result.Reinitialize(width, height);
            result.ReadPixels(texR, 0, 0, true);
            return result;
        }

        /// <summary>
        ///     Scales the texture data of the given texture.
        /// </summary>
        /// <param name="tex">Texure to scale</param>
        /// <param name="width">New width</param>
        /// <param name="height">New height</param>
        /// <param name="mode">Filtering mode</param>
        public static void Scale(Texture2D tex, int width, int height, FilterMode mode = FilterMode.Trilinear)
        {
            Rect texR = new(0, 0, width, height);
            _gpu_scale(tex, width, height, mode);

            // Update new texture
            tex.Reinitialize(width, height);
            tex.ReadPixels(texR, 0, 0, true);
            tex.Apply(true); //Remove this if you hate us applying textures for you :)
        }

        // Internal unility that renders the source texture into the RTT - the scaling method itself.
        private static void _gpu_scale(Texture2D src, int width, int height, FilterMode fmode)
        {
            //We need the source texture in VRAM because we render with it
            src.filterMode = fmode;
            src.Apply(true);

            //Using RTT for best quality and performance. Thanks, Unity 5
            RenderTexture rtt = new(width, height, 32);

            //Set the RTT in order to render to it
            Graphics.SetRenderTarget(rtt);

            //Setup 2D matrix in range 0..1, so nobody needs to care about sized
            GL.LoadPixelMatrix(0, 1, 1, 0);

            //Then clear & draw the texture to fill the entire RTT.
            GL.Clear(true, true, new Color(0, 0, 0, 0));
            Graphics.DrawTexture(new Rect(0, 0, 1, 1), src);
        }
    }

        public class Path
        {
        /// <summary>
        /// All The cells IDs that you need to pass to get to your point;
        /// </summary>
        public int[] points;

        public int current;

        public bool Usable;

        public int next
        {
            get
            {
                if (current + 1 >= points.Length - 1)
                    return points.Length - 1;

                return current + 1;
            }
        }

        public void NextPoint() { current++; }
    }

    [System.Serializable]
    public class Cell
    {
        public int ID;

        public int data;

        public Cell TopNeighBour;
        public Cell BottomNeighBour;
        public Cell LeftNeighBour;
        public Cell RightNeighBour;

        public Cell(int id)
        {
            ID = id;
        }
    }

    [System.Serializable]
    public class Grid
    {
        public int Width;
        public int Height;

        public Cell[] cells;

        public Grid(int w, int h)
        {
            Width = w;
            Height = h;

            cells = new Cell[w * h];

            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = new Cell(i);
                //SetData(i, 0);
            }
        }

        public Cell GetCell(int id)
        {

            if (id < 0 || id >= cells.Length)
                return null;

            return cells[id];
        }

        /// <summary>
        /// the id of the tile and the data of the tile(0 - Normal Room || 1 - Void || 2 - Shop Room)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public void SetData(int id, int data)
        {
            //for (int i = 0; i < this.cells.Length; i++)
            //{
                cells[id].data = data;

            //}
        }

        public void SetNeightBours()
        {
            //var t = mapTexture;

            //int i = 0;

            //var data = t.GetPixels();

            //Debug.Log(data.Length);

            for (int i = 0; i < cells.Length; i++)
            {
                var cell = GetCell(i);

                if (cell == null)
                    continue;

                if (cell.data == 1)
                    continue;

                if(cell.LeftNeighBour == null)
                {

                    if (i - 1 < 0)
                    {
                        cell.LeftNeighBour = null;
                    }
                    else if (i % Width == 0)
                    {
                        cell.LeftNeighBour = null;
                    }
                    else
                    {
                        var left = GetCell(i - 1);
                        if (left != null)
                        {
                            cell.LeftNeighBour = left;
                            left.RightNeighBour = cell;
                        }
                        else
                            cell.LeftNeighBour = null;
                    }

                }

                if (cell.RightNeighBour == null)
                {

                    if (i + 1 >= cells.Length)
                    {
                        cell.RightNeighBour = null;
                    }
                    else if ((i + 1) % Width == 0)
                    {
                        cell.RightNeighBour = null;
                    }
                    else
                    {
                        var left = GetCell(i + 1);
                        if (left != null)
                        {
                            cell.RightNeighBour = left;
                            left.LeftNeighBour = cell;
                        }
                        else
                            cell.RightNeighBour = null;
                    }

                }

                if (cell.TopNeighBour == null)
                {
                    if (i + Height >= cells.Length)
                    {
                        cell.TopNeighBour = null;
                    }
                    else
                    {
                        var left = GetCell(i + Height);
                        if (left != null)
                        {
                            cell.TopNeighBour = left;
                            left.BottomNeighBour = cell;
                        }
                        else
                            cell.TopNeighBour = null;
                    }

                }

                if (cell.BottomNeighBour == null)
                {

                    if (i - Height < 0)
                    {
                        cell.BottomNeighBour = null;
                    }
                    else
                    {
                        var left = GetCell(i - Height);
                        if (left != null)
                        {
                            cell.BottomNeighBour = left;
                            left.TopNeighBour = cell;
                        }
                        else
                            cell.BottomNeighBour = null;
                    }
                }
            }

            //for (int x = 0; x < Width; x++)
            //{
            //    for (int y = 0; y < Height; y++)
            //    {
            //        var cell = GetCell(i);

            //        if (cell.data == 1)
            //            return;

            //        if (y + 1 >= Height)
            //        {
            //            cell.TopNeighBour = null;
            //        }
            //        else
            //        {
            //            var c = t.GetPixel(x, y + 1);

            //            if (c == Color.black)
            //                cell.TopNeighBour = null;
            //            else
            //                cell.TopNeighBour = GetCell(i + 4);
            //        }

            //        if (y == 0)
            //        {
            //            cell.BottomNeighBour = null;
            //        }
            //        else
            //        {
            //            var c = t.GetPixel(x, y - 1);

            //            if (c == Color.black)
            //                cell.BottomNeighBour = null;
            //            else
            //                cell.BottomNeighBour = GetCell(i - 4);
            //        }

            //        if (x + 1 >= Width)
            //        {
            //            cell.RightNeighBour = null;
            //        }
            //        else
            //        {
            //            var c = t.GetPixel(x + 1, y);

            //            if (c == Color.black)
            //                cell.RightNeighBour = null;
            //            else
            //                cell.RightNeighBour = GetCell(i + 1);
            //        }

            //        if (x == 0)
            //        {
            //            cell.LeftNeighBour = null;
            //        }
            //        else
            //        {
            //            var c = t.GetPixel(x - 1, y);

            //            if (c == Color.black)
            //                cell.LeftNeighBour = null;
            //            else
            //                cell.LeftNeighBour = GetCell(i - 1);
            //        }
            //        //if (cells[i].data == 0)
            //        //    t.SetPixel(x, y, Color.white);
            //        //else
            //        //    t.SetPixel(x, y, Color.black);

            //        i++;

            //    }
            //}


        }

        //public void FillTexture(Texture2D t, Color c)
        //{
        //    for (int x = 0; x < t.width; x++)
        //    {
        //        for (int y = 0; y < t.height; y++)
        //        {
        //            t.SetPixel(x, y, c);
        //        }
        //    }
        //}

        //public Texture2D mapTexture
        //{
        //    get
        //    {
        //        var t = new Texture2D(Width * 4 + 1, Height * 4 + 1);
        //        t.filterMode = FilterMode.Point;

        //        int i = 0;

        //        FillTexture(t, Color.black);

        //        for (int x = 2; x < t.width; x += 4)
        //        {
        //            for (int y = 2; y < t.height; y += 4)
        //            {

        //                if (cells[i].data == 0)
        //                {
        //                    t.SetPixel(x + 1, y, Color.white);
        //                    t.SetPixel(x - 1, y, Color.white);
        //                    t.SetPixel(x, y + 1, Color.white);
        //                    t.SetPixel(x, y - 1, Color.white);
        //                    t.SetPixel(x, y, Color.white);
        //                    t.SetPixel(x + 1, y + 1, Color.white);
        //                    t.SetPixel(x - 1, y - 1, Color.white);
        //                    t.SetPixel(x + 1, y - 1, Color.white);
        //                    t.SetPixel(x - 1, y + 1, Color.white);
        //                }
        //                else
        //                {
        //                    t.SetPixel(x + 1, y, Color.black);
        //                    t.SetPixel(x - 1, y, Color.black);
        //                    t.SetPixel(x, y + 1, Color.black);
        //                    t.SetPixel(x, y - 1, Color.black);
        //                    t.SetPixel(x, y, Color.black);
        //                    t.SetPixel(x + 1, y + 1, Color.black);
        //                    t.SetPixel(x - 1, y - 1, Color.black);
        //                    t.SetPixel(x + 1, y - 1, Color.black);
        //                    t.SetPixel(x - 1, y + 1, Color.black);
        //                }


        //                i++;

        //            }
        //        }

        //        for (int x = 0; x < t.width; x += 4)
        //        {
        //            for (int y = 2; y < t.height; y += 4)
        //            {
        //                if (x == 0 || y == 0)
        //                    continue;

        //                if((t.GetPixel(x + 1, y) == Color.white || t.GetPixel(x, y + 1) == Color.white) && (t.GetPixel(x - 1, y) == Color.white || t.GetPixel(x, y - 1) == Color.white))
        //                {
        //                    t.SetPixel(x, y, Color.gray);
        //                }
        //            }
        //        }

        //        for (int x = 2; x < t.width; x += 4)
        //        {
        //            for (int y = 0; y < t.height; y += 4)
        //            {
        //                if (x == 0 || y == 0)
        //                    continue;

        //                if ((t.GetPixel(x + 1, y) == Color.white || t.GetPixel(x, y + 1) == Color.white) && (t.GetPixel(x - 1, y) == Color.white || t.GetPixel(x, y - 1) == Color.white))
        //                {
        //                    t.SetPixel(x, y, Color.gray);
        //                }
        //            }
        //        }

        //        t.Apply();

        //        System.IO.File.WriteAllBytes(Application.dataPath + "/Level.png", t.EncodeToPNG());

        //        return t;
        //    }
        //}

        //public Texture2D GetHighQualityImage(int width, int height, int outline)
        //{
        //    var t = mapTexture;
        //    t = GPUTextureScaler.Scaled(t, width, height, FilterMode.Point);

        //    int ratioX = (int)((float)mapTexture.width / (float)width);
        //    int ratioY = (int)((float)mapTexture.height / (float)height);

        //    for (int x = 0; x < width; x++)
        //    {
        //        for (int y = 0; y < height; y++)
        //        {
        //            int ogX = x / ratioX;
        //            int ogY = y / ratioY;

        //            if(ogX >= 1)
        //            {

        //                if (mapTexture.GetPixel(ogX - 1, ogY) == Color.black && mapTexture.GetPixel(ogX, ogY) == Color.white)
        //                {
        //                    //t.SetPixel(x, y, Color.gray);
        //                }
        //            }

        //        }
        //    }

        //    t.Apply();
        //    return t;
        //}

        public bool CanGoTo(int start, int end, out Path path)
        {
            GoToStartDirection(start, end, 0, out Path p0);
            GoToStartDirection(start, end, 1, out Path p1);
            GoToStartDirection(start, end, 2, out Path p2);
            GoToStartDirection(start, end, 3, out Path p3);

            List<Path> p = new List<Path>();

            if (p0.Usable)
                p.Add(p0);
            if (p1.Usable)
                p.Add(p1);
            if (p2.Usable)
                p.Add(p2);
            if (p3.Usable)
                p.Add(p3);


            int x = 0;

            for (int i = 0; i < p.Count; i++)
            {
                if (p[i].points != null && p[x].points != null && (p[i].points.Length < p[x].points.Length) && p[i].Usable)
                {
                    x = i;
                }
            }

            if (p.Count > 0 && p[x].Usable)
            {
                path = p[x];
                return true;
            }

            path = new Path() { Usable = false };
            return false;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="direction">0 - Up || 1 - Down || 2 - Left || 3 - Right</param>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool GoToStartDirection(int start, int end, int direction, out Path path, int[] tilesToSkip = null)
        {
            List<int> points = new List<int>();
            path = new Path();
            path.Usable = false;

            int i = start;

            if(direction == 0)
            {
                var cell = GetCell(i);

                var top = cell.TopNeighBour;

                if (top == null || top.data == 1)
                    return false;

                i = top.ID;
            }

            if (direction == 1)
            {
                var cell = GetCell(i);

                var top = cell.BottomNeighBour;

                if (top == null || top.data == 1)
                    return false;

                i = top.ID;
            }

            if (direction == 2)
            {
                var cell = GetCell(i);

                var top = cell.LeftNeighBour;

                if (top == null || top.data == 1)
                    return false;

                i = top.ID;
            }

            if (direction == 3)
            {
                var cell = GetCell(i);

                var top = cell.RightNeighBour;

                if (top == null || top.data == 1)
                    return false;

                i = top.ID;
            }

            while (i != end)
            {
                var cell = GetCell(i);

                var top = cell.TopNeighBour;
                var bottom = cell.BottomNeighBour;
                var left = cell.LeftNeighBour;
                var right = cell.RightNeighBour;

                int Tdif;
                int Bdif;
                int Ldif;
                int Rdif;



                //// 1 - top || 2 - bottom || 3 - right || 4 - left
                int nextDir = 0;

                if (top == null || top.data == 1)
                    Tdif = 10000;
                else
                    Tdif = Mathf.Abs(top.ID - end);

                if (bottom == null || bottom.data == 1)
                    Bdif = 10000;
                else
                    Bdif = Mathf.Abs(bottom.ID - end);

                if (left == null || left.data == 1)
                    Ldif = 10000;
                else
                    Ldif = Mathf.Abs(left.ID - end);

                if (right == null || right.data == 1)
                    Rdif = 10000;
                else
                    Rdif = Mathf.Abs(right.ID - end);


                int[] differences = new int[4] { Tdif, Bdif, Rdif, Ldif };

                for (int x = 0; x < differences.Length; x++)
                {
                    if (differences[x] < differences[nextDir])
                    {
                        nextDir = x;
                    }
                }

                if (nextDir == 0)
                {
                    if (top == null)
                        return false;

                    if (points.Contains(top.ID))
                    {
                        return false;
                    }

                    points.Add(top.ID);
                    i = top.ID;
                }
                else if (nextDir == 1)
                {

                    if (bottom == null)
                        return false;


                    if (points.Contains(bottom.ID))
                        return false;


                    points.Add(bottom.ID);
                    i = bottom.ID;
                }
                else if (nextDir == 2)
                {

                    if (right == null)
                        return false;



                    if (points.Contains(right.ID))
                        return false;


                    points.Add(right.ID);
                    i = right.ID;
                }
                else
                {


                    if (left == null)
                        return false;



                    if (points.Contains(left.ID))
                        return false;


                    points.Add(left.ID);
                    i = left.ID;


                }


                path.points = points.ToArray();

            }

            path.Usable = true;
            return true;
        }

        
    }
}
public class LevelGenerator : MonoBehaviour
{
    [Header("Settings")]
    public int NumberOfFloors;
    public int Width;
    public int Height;
    public Vector2 MinMaxEnemyNumber;
    public GameObject RoomObject;
    public GameObject Path;
    public Vector3 pathOffset;
    [Space]
    [Space]
    [Space]
    public Vector3 doorSize;
    public Vector3 doorSize2;
    public Vector3 ElevatorSize;
    public NavMeshSurface navMesh;
    public EnemyBehaviour[] AvailableEnemies;
    public ItemProbability[] AvailableObjectsToReward;
    public UnityEngine.UI.Image MiniMap;
    public UnityEngine.UI.Image Player;
    public Vector2 UIoffset;
    public float UIScaling;
    public float SpacingMultiplier;
    public int Spacing;

    GameObject levelParent;

    public List<Vector3> nextPoints;

    public Texture2D map;

    public GameObject Shop;

    //public GridPathFinder.Grid grid_Floor1;
    //public GridPathFinder.Grid grid_Floor2;
    public GridPathFinder.Grid[] Floors_Grid;

    GridPathFinder.Path path;

    Texture2D myGridTexture;

    //[HideInInspector]
    public GameObject FirstRoom;
    //[HideInInspector]
    public GameObject LastRoom;

    public static LevelGenerator generator;
    // Start is called before the first frame update
    void Start()
    {
        generator = this;
        AudioManager.Init();
        GetLevel();

        //myGridTexture = grid.mapTexture;
        //myGridTexture.SetPixel((int)(FirstRoom.transform.position.x / Spacing), (int)(FirstRoom.transform.position.z / Spacing), Color.green);
        //myGridTexture.SetPixel((int)(LastRoom.transform.position.x / Spacing), (int)(LastRoom.transform.position.z / Spacing), Color.red);
        ////myGridTexture.SetPixel((int)(levelParent.transform.transform.position.x / Spacing), (int)(levelParent.transform.GetChild(0).transform.position.z / Spacing), Color.green);
        //myGridTexture.Apply();
        //MiniMap.sprite = Sprite.Create(myGridTexture, new Rect(0, 0, grid.mapTexture.width, grid.mapTexture.height), new Vector2(0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    GetLevel();
        //}



        float xPos = PlayerMovement.instance.transform.position.x / (Spacing * SpacingMultiplier);
        float yPos = PlayerMovement.instance.transform.position.z / (Spacing * SpacingMultiplier);

        xPos -= UIoffset.x;
        yPos -= UIoffset.y;
        RectTransform rt = Player.rectTransform;
        rt.anchoredPosition = new Vector3(xPos * UIScaling, yPos * UIScaling);
        //Debug.Log($"{xPos} | {yPos} || {xPos * UIScaling} |{yPos * UIScaling}");

        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    var canGo = grid.CanGoTo(0, (Width * Height) - 1, out path);

        //    Debug.Log($"Can Go : {canGo}");
        //}


        //if (path != null && path.Usable)
        //{
        //    for (int i = 1; i < path.points.Length; i++)
        //    {

        //        Vector3 p0 = Vector3.zero;
        //        Vector3 p1 = Vector3.zero;
        //        foreach (Transform child in levelParent.transform)
        //        {
        //            if(child.name == path.points[i].ToString())
        //            {
        //                p1 = child.position;
        //            }

        //            if (child.name == path.points[i - 1].ToString())
        //            {
        //                p0= child.position;
        //            }
        //        }

        //        Debug.DrawLine(p0, p1);
        //    }
        //}


        var game = FindObjectOfType<GameSettings>();

        if (game.difficulty == GameSettings.GameDifficulty.Easy)
        {
            MinMaxEnemyNumber = new Vector2(1, 3);
        }


        if (game.difficulty == GameSettings.GameDifficulty.Medium)
        {
            MinMaxEnemyNumber = new Vector2(1, 5);
        }


        if (game.difficulty == GameSettings.GameDifficulty.Hard)
        {
            MinMaxEnemyNumber = new Vector2(2, 7);
        }

#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Break();
        }
#endif


    }

    public void ReloadLevel()
    {
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex));
    }

    IEnumerator LoadScene(int i)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(i);
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / .9f);
            Debug.Log(op.progress * 100);
            yield return null;
        }
    }
    void GetLevel()
    {
        Floors_Grid = new GridPathFinder.Grid[NumberOfFloors];
        MiniMapManager.GetMiniMap().maps = new Texture2D[NumberOfFloors];

        for (int i = 0; i < NumberOfFloors; i++)
        {
            MakeGrid(out Floors_Grid[i]);
        }

        if (levelParent != null)
            Destroy(levelParent);

        levelParent = new GameObject("Level");

        int[] starts = new int[NumberOfFloors];
        int[] ends = new int[NumberOfFloors];

        for (int i = 0; i < NumberOfFloors; i++)
        {
            for (int x = 0; x < Floors_Grid[i].cells.Length; x++)
            {
                if (Floors_Grid[i].cells[x].data == 0)
                {
                    if (starts[i] == -1)
                        starts[i] = x;

                    ends[i] = x;
                }

            }

        }

        bool[] Positive = new bool[NumberOfFloors];

        for (int i = 0; i < NumberOfFloors; i++)
        {
            Positive[i] = Floors_Grid[i].CanGoTo(starts[i], ends[i], out GridPathFinder.Path p);
        }

        bool CanGo = true;

        for (int i = 0; i < Positive.Length; i++)
        {
            if (Positive[i] != true)
            {
                CanGo = false;
                break;
            }

        }

        if (!CanGo)
        {
            MiniMapManager.GetMiniMap().DeleteAllIcons();
            GetLevel();
        }
        else
        {
            for (int i = 0; i < NumberOfFloors; i++)
            {
                for (int x = 0; x < Floors_Grid[i].cells.Length; x++)
                {
                    if(Floors_Grid[i].CanGoTo(starts[i], x, out GridPathFinder.Path p))
                    {

                    }
                    else
                    {
                        Floors_Grid[i].cells[x].data = 1;
                        Floors_Grid[i].SetNeightBours();
                    }
                }

            }


            for (int i = 0; i < NumberOfFloors; i++)
            {
                GenerateRooms(Floors_Grid[i], 30 * i, $"Floor {i}", i);
            }


            FirstRoom = levelParent.transform.GetChild(0).transform.GetChild(0).gameObject;
            LastRoom = levelParent.transform.GetChild(NumberOfFloors - 1).transform.GetChild(levelParent.transform.GetChild(NumberOfFloors - 1).transform.childCount - 1).gameObject;

  
            //StartCoroutine(PlacePlayer());

            PlayerMovement.instance.SetPositionInstant(FirstRoom.GetComponent<LevelRoom>().bounds.center);



            PlaceDoors();




            if (NumberOfFloors > 1)
            {

                for (int i = 0; i < NumberOfFloors - 1; i++)
                {
                    for (int x = 0; x < Floors_Grid[i].cells.Length; x++)
                    {
                        if (Floors_Grid[i].GetCell(x).data == 0 && Floors_Grid[i + 1].GetCell(x).data == 0)
                        {
                            LevelRoom r = levelParent.transform.GetChild(i).transform.Find(x.ToString()).GetComponent<LevelRoom>();
                            LevelRoom r2 = levelParent.transform.GetChild(i + 1).transform.Find((x).ToString()).GetComponent<LevelRoom>();

                            r.UpperLevel = r2;
                            r2.SubLevel = r;

                            r.UpdateDoors();
                            r2.UpdateDoors();
                        }
                    }
                }
            }

            PlaceElevators();

            PlaceSpecialRoom();




            navMesh.BuildNavMesh();

            for (int i = 0; i < levelParent.transform.childCount; i++)
            {
                foreach (Transform child in levelParent.transform.GetChild(i).transform)
                {
                    if (child.name.Contains("Path"))
                        continue;

                    LevelRoom r = child.GetComponent<LevelRoom>();

                    if (r == null)
                        continue;

                    r.CanSpawnEnemies = true;
                }

            }




        }

        //UnityEditor.AI.NavMeshBuilder.ClearAllNavMeshes();
        //UnityEditor.AI.NavMeshBuilder.BuildNavMesh();
    }

    public GameObject GetRoomInFloor(int floor, int roomID)
    {

        return levelParent.transform.GetChild(floor).Find(roomID.ToString()).gameObject;
    }

    //IEnumerator PlacePlayer()
    //{


    //}

    void PlaceSpecialRoom()
    {
        #region Store
        int floor = Random.Range(0, NumberOfFloors);

        bool hasStoreRoom = false;
        while(hasStoreRoom == false)
        {
            int i = Random.Range(0, Floors_Grid[floor].cells.Length);

            if(Floors_Grid[floor].GetCell(i).data != 1)
            {
                var r = GetRoomInFloor(floor, i).GetComponent<LevelRoom>();
                r.roomType = LevelRoom.RoomType.Store;

                Instantiate(Shop, r.GetFreePosition(), Shop.transform.rotation, r.transform);

                Debug.Log($"Store is in floor {floor} in the room number {i}");

                hasStoreRoom = true;
            }
        }
        #endregion

        #region Elevator
        //floor = Random.Range(0, NumberOfFloors);

        floor = 0;

        bool hasElevatorRoom = false;
        while (hasElevatorRoom == false)
        {
            int i = Random.Range(0, Floors_Grid[floor].cells.Length);

            if (Floors_Grid[floor].GetCell(i).data != 1)
            {

                var r = GetRoomInFloor(floor, i).GetComponent<LevelRoom>();
                if(r.roomType == LevelRoom.RoomType.Normal && r.UpperLevel != null)
                {
                    r.roomType = LevelRoom.RoomType.Elevator;

                    Debug.Log($"Store is in Elevator {floor} in the room number {i}");

                    hasElevatorRoom = true;
                }

            }
        }
        #endregion

    }

    void PlaceDoors()
    {

        for (int i = 0; i < levelParent.transform.childCount; i++)
        {

            foreach (Transform child in levelParent.transform.GetChild(i))
            {
                if (child.name.Contains("Path"))
                    return;

                LevelRoom r = child.GetComponent<LevelRoom>();

                if (r == null)
                    continue;

                if (FirstRoom == null)
                    Debug.Log("NULL");


                if (r.transform == FirstRoom.transform)
                {
                    r.ForceLayout(0);
                }


                r.ChooseLevelLayout();

                //var cell = grid.GetCell(int.Parse(child.name));

                if (r.Left != null)
                {
                    var pos = QuickDraw.MidPointVec3(r.LeftWall.transform.position, r.Left.RightWall.transform.position);
                    var bounds = QuickDraw.JoinBounds(new Bounds(r.LeftWall.transform.position, doorSize), new Bounds(r.Left.RightWall.transform.position, doorSize));
                    var p = Instantiate(Path, pos - pathOffset, Quaternion.Euler(0, 0, 0), levelParent.transform);
                    p.transform.localScale = doorSize;

                }

                if (r.Top != null)
                {
                    var pos = QuickDraw.MidPointVec3(r.TopWall.transform.position, r.Top.BottomWall.transform.position);
                    var bounds = QuickDraw.JoinBounds(new Bounds(r.TopWall.transform.position, doorSize2), new Bounds(r.Top.BottomWall.transform.position, doorSize2));
                    var p = Instantiate(Path, pos - pathOffset, Quaternion.Euler(0, 90, 0), levelParent.transform);
                    p.transform.localScale = doorSize2;

                }

                if (r.UpperLevel != null)
                {
                    var pos = QuickDraw.MidPointVec3(r.Ceiling.transform.position, r.UpperLevel.Floor.transform.position);
                    var bounds = QuickDraw.JoinBounds(new Bounds(r.Ceiling.transform.position, doorSize), new Bounds(r.UpperLevel.Floor.transform.position, doorSize));
                    var p = Instantiate(Path, pos - pathOffset, Quaternion.Euler(0, 0, 0), levelParent.transform);
                    p.transform.localScale = bounds.size;

                }

            }
        }

    }

    void PlaceElevators()
    {

        for (int i = 0; i < levelParent.transform.childCount; i++)
        {

            foreach (Transform child in levelParent.transform.GetChild(i))
            {
                if (child.name.Contains("Path"))
                    return;

                LevelRoom r = child.GetComponent<LevelRoom>();

                if (r == null)
                    continue;

                if (FirstRoom == null)
                    return;


                if (r.transform == FirstRoom.transform)
                {
                    r.ForceLayout(0);
                }


                //r.ChooseLevelLayout();

                //var cell = grid.GetCell(int.Parse(child.name));


                if (r.UpperLevel != null)
                {
                    var pos = QuickDraw.MidPointVec3(r.Ceiling.transform.position, r.UpperLevel.Floor.transform.position);
                    //var bounds = QuickDraw.JoinBounds(new Bounds(r.Ceiling.transform.position, doorSize), new Bounds(r.UpperLevel.Floor.transform.position, doorSize));
                    var p = Instantiate(Path, pos - pathOffset, Quaternion.Euler(90, 0, 0), levelParent.transform);
                    p.transform.localScale = ElevatorSize;

                }

            }
        }

    }

    public void MakeGrid(out GridPathFinder.Grid grid)
    {
        grid = new GridPathFinder.Grid(Width, Height);

        int a = 0;
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                float i = Random.Range(0.0f, 1.0f);

                if (i <= 0.8f)
                {
                    grid.SetData(a, 0);
                }
                else
                {

                    grid.SetData(a, 1);
                }
                a++;

            }
        }

        grid.SetNeightBours();
    }

    void GenerateRooms(GridPathFinder.Grid grid, float level, string ParentName, int mapNumber)
    {
      

 
        var parent = new GameObject(ParentName);

        parent.transform.parent = levelParent.transform;

        nextPoints = new List<Vector3>();
        //var extra = new List<GameObject>();

        int a = 0;
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {

                if(grid.cells[a].data == 0)
                {

                    var room = Instantiate(RoomObject, parent.transform);

                    room.gameObject.name = (a).ToString();

                    room.transform.position = new Vector3(x * Spacing, level, y * Spacing);

                    room.transform.parent = parent.transform;
                    if (FirstRoom == null)
                        FirstRoom = room;

                    LastRoom = room;

                }
                else
                {

                    //grid.SetData(a, 1);
                }
                a++;

            }
        }


        //bool storeSet = false;

        //while (!storeSet)
        //{
        //    int x = Random.Range(0, Width * Height);

        //    if (x != 0 && grid.GetCell(x).data == 0)
        //    {
        //        //grid.SetData(x, 2);
        //        LevelRoom r = parent.transform.Find(x.ToString()).GetComponent<LevelRoom>();
        //        Instantiate(Shop, r.GetFreePosition(), Shop.transform.rotation, r.transform);
        //        storeSet = true;
        //        Debug.Log($"Store is in room {x}");
        //        Debug.Log($"Store is in room {x}");
        //    }

        //}

        grid.SetNeightBours();
        //levelParent.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.green;
        //levelParent.transform.GetChild(levelParent.transform.childCount - 1).GetComponent<MeshRenderer>().material.color = Color.red;

        foreach (Transform child in parent.transform)
        {
            LevelRoom r = child.GetComponent<LevelRoom>();
            r.roomType = LevelRoom.RoomType.Normal;
            r.GenerateBounds();

            var cell = grid.GetCell(int.Parse(child.name));

            List<LevelRoom.Exit> exits = new List<LevelRoom.Exit>();

            if(cell.TopNeighBour != null && cell.TopNeighBour.data == 0)
            {
                r.Top = parent.transform.Find(cell.TopNeighBour.ID.ToString()).GetComponent<LevelRoom>();
                exits.Add(LevelRoom.Exit.Top);
            }

            if (cell.BottomNeighBour != null && cell.BottomNeighBour.data == 0)
            {
                r.Bottom = parent.transform.Find(cell.BottomNeighBour.ID.ToString()).GetComponent<LevelRoom>();
                exits.Add(LevelRoom.Exit.Bottom);
            }

            if (cell.LeftNeighBour != null && cell.LeftNeighBour.data == 0)
            {
                r.Left = parent.transform.Find(cell.LeftNeighBour.ID.ToString()).GetComponent<LevelRoom>();
                exits.Add(LevelRoom.Exit.Left);
            }

            if (cell.RightNeighBour != null && cell.RightNeighBour.data == 0)
            {
                r.Right = parent.transform.Find(cell.RightNeighBour.ID.ToString()).GetComponent<LevelRoom>();
                exits.Add(LevelRoom.Exit.Right);
            }

            r.exits = exits.ToArray();

            r.UpdateDoors();
        }

        //for (int i = 0; i < extra.Count; i++)
        //{
        //    extra[i].transform.parent = levelParent.transform;
        //}

        //map = grid.mapTexture;
        GetComponent<MiniMapManager>().GenerateMiniMap(grid, mapNumber);
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < nextPoints.Count; i++)
        {
            Gizmos.DrawWireSphere(nextPoints[i], 1.0f);
        }
    }

    public bool IsBlockInPosition(Vector3 pos, float radius = .1f)
    {
        return Physics.OverlapSphere(pos, radius).Length >= 1;
    }

    public GameObject GetBlockAtPosition(Vector3 pos, float radius = .1f)
    {
        var a = Physics.OverlapSphere(pos, radius);
        
        if (a.Length <= 0)
            return null;

        return a[0].gameObject;
    }


}
