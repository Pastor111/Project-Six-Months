using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridPathFinder
{
    
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
        /// the id of the tile and the data of the tile(0 - Normal || 1 - Cant Pass)
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

                if (cell == null || cell.data == 1)
                    continue;

                if(cell.LeftNeighBour == null)
                {

                    if (i - 1 < 0)
                    {
                        cell.LeftNeighBour = null;
                    }
                    else if (i % 4 == 0)
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
                    else if ((i + 1) % 4 == 0)
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

        public Texture2D mapTexture
        {
            get
            {
                var t = new Texture2D(Width, Height);
                t.filterMode = FilterMode.Point;

                int i = 0;

                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {

                        if (cells[i].data == 0)
                            t.SetPixel(x, y, Color.white);
                        else
                            t.SetPixel(x, y, Color.black);

                        i++;

                    }
                }

                t.Apply();

                return t;
            }
        }

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
    public int Width;
    public int Height;
    public GameObject RoomObject;
    public GameObject Path;
    public Vector3 pathOffset;
    [Space]
    [Space]
    [Space]
    public Vector3 doorSize;
    public Vector3 doorSize2;

    public int Spacing;

    GameObject levelParent;

    public List<Vector3> nextPoints;

    public Texture2D map;

    public GridPathFinder.Grid grid;

    GridPathFinder.Path path;

    public static LevelGenerator generator;
    // Start is called before the first frame update
    void Start()
    {
        generator = this;
        GetLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GetLevel();
        }

        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    var canGo = grid.CanGoTo(0, (Width * Height) - 1, out path);

        //    Debug.Log($"Can Go : {canGo}");
        //}


        if (path != null && path.Usable)
        {
            for (int i = 1; i < path.points.Length; i++)
            {

                Vector3 p0 = Vector3.zero;
                Vector3 p1 = Vector3.zero;
                foreach (Transform child in levelParent.transform)
                {
                    if(child.name == path.points[i].ToString())
                    {
                        p1 = child.position;
                    }

                    if (child.name == path.points[i - 1].ToString())
                    {
                        p0= child.position;
                    }
                }

                Debug.DrawLine(p0, p1);
            }
        }

    }

    void GetLevel()
    {
        GenerateRooms();
        var canGo = grid.CanGoTo(0, (Width * Height) - 1, out path);

        if (!canGo)
        {
            GetLevel();
        }

        PlaceDoors();
    }

    void PlaceDoors()
    {
        foreach (Transform child in levelParent.transform)
        {
            if (child.name.Contains("Path"))
                return;

            LevelRoom r = child.GetComponent<LevelRoom>();

            var cell = grid.GetCell(int.Parse(child.name));

            if(r.Left != null)
            {
                var pos = QuickDraw.MidPointVec3(r.LeftWall.transform.position, r.Left.RightWall.transform.position);
                var bounds = QuickDraw.JoinBounds(new Bounds(r.LeftWall.transform.position, doorSize), new Bounds(r.Left.RightWall.transform.position, doorSize));
                var p = Instantiate(Path, pos - pathOffset, Quaternion.Euler(0, 0, 0), levelParent.transform);
                p.transform.localScale = bounds.size;

            }

            if (r.Top != null)
            {
                var pos = QuickDraw.MidPointVec3(r.TopWall.transform.position, r.Top.BottomWall.transform.position);
                var bounds = QuickDraw.JoinBounds(new Bounds(r.TopWall.transform.position, doorSize2), new Bounds(r.Top.BottomWall.transform.position, doorSize2));
                var p = Instantiate(Path, pos - pathOffset, Quaternion.Euler(0, 90, 0), levelParent.transform);
                p.transform.localScale = bounds.size;

            }
        }
    }

    void GenerateRooms()
    {
        grid = new GridPathFinder.Grid(Width, Height);

        if (levelParent != null)
            Destroy(levelParent);

        levelParent = new GameObject("Level");
        //var extra = new GameObject("Level_Extra");

        nextPoints = new List<Vector3>();
        var extra = new List<GameObject>();
        // Generate Room

        int a = 0;
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                float i = Random.Range(0.0f, 1.0f);

                if(i <= 0.8f)
                {
                    grid.SetData(a, 0);
                    var room = Instantiate(RoomObject, levelParent.transform);

                    room.gameObject.name = (a).ToString();

                    room.transform.position = new Vector3(x * Spacing, 0, y * Spacing);

                    room.transform.parent = levelParent.transform;
                }
                else
                {

                    grid.SetData(a, 1);
                }
                a++;

            }
        }

        grid.SetNeightBours();
        //levelParent.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.green;
        //levelParent.transform.GetChild(levelParent.transform.childCount - 1).GetComponent<MeshRenderer>().material.color = Color.red;

        foreach (Transform child in levelParent.transform)
        {
            LevelRoom r = child.GetComponent<LevelRoom>();

            var cell = grid.GetCell(int.Parse(child.name));

            List<LevelRoom.Exit> exits = new List<LevelRoom.Exit>();

            if(cell.TopNeighBour != null && cell.TopNeighBour.data == 0)
            {
                r.Top = GameObject.Find(cell.TopNeighBour.ID.ToString()).GetComponent<LevelRoom>();
                exits.Add(LevelRoom.Exit.Top);
            }

            if (cell.BottomNeighBour != null && cell.BottomNeighBour.data == 0)
            {
                r.Bottom = GameObject.Find(cell.BottomNeighBour.ID.ToString()).GetComponent<LevelRoom>();
                exits.Add(LevelRoom.Exit.Bottom);
            }

            if (cell.LeftNeighBour != null && cell.LeftNeighBour.data == 0)
            {
                r.Left = GameObject.Find(cell.LeftNeighBour.ID.ToString()).GetComponent<LevelRoom>();
                exits.Add(LevelRoom.Exit.Left);
            }

            if (cell.RightNeighBour != null && cell.RightNeighBour.data == 0)
            {
                r.Right = GameObject.Find(cell.RightNeighBour.ID.ToString()).GetComponent<LevelRoom>();
                exits.Add(LevelRoom.Exit.Right);
            }

            r.exits = exits.ToArray();

            r.UpdateDoors();
        }

        //for (int i = 0; i < extra.Count; i++)
        //{
        //    extra[i].transform.parent = levelParent.transform;
        //}

        map = grid.mapTexture;
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
