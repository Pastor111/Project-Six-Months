using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LevelRoom : MonoBehaviour
{
    public enum Exit {Top, Bottom, Left, Right}

    public Exit[] exits;

    public GameObject LeftWall;
    public GameObject RightWall;
    public GameObject TopWall;
    public GameObject BottomWall;
    public GameObject Door;

    public LevelRoom Top;
    public LevelRoom Bottom;
    public LevelRoom Left;
    public LevelRoom Right;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if(Top != null)
        //{
        //    var mid = QuickDraw.MidPointVec3(TopWall.transform.position, Top.BottomWall.transform.position);

        //    QuickDraw.DrawBounds(new Bounds(mid, Vector3.one));


        //    var b = QuickDraw.JoinBounds(new Bounds(TopWall.transform.position, LevelGenerator.generator.doorSize), new Bounds(Top.BottomWall.transform.position, LevelGenerator.generator.doorSize));

        //    QuickDraw.DrawBounds(b);
        //    //Debug.DrawLine(TopWall.transform.position, Top.BottomWall.transform.position);
        //}

        //if (Bottom != null)
        //{

        //    var mid = QuickDraw.MidPointVec3(BottomWall.transform.position, Bottom.TopWall.transform.position);

        //    QuickDraw.DrawBounds(new Bounds(mid, Vector3.one));

        //    var b = QuickDraw.JoinBounds(new Bounds(BottomWall.transform.position, LevelGenerator.generator.doorSize), new Bounds(Bottom.TopWall.transform.position, LevelGenerator.generator.doorSize));

        //    QuickDraw.DrawBounds(b);
        //    //Debug.DrawLine(BottomWall.transform.position, Bottom.TopWall.transform.position);
        //}

        //if (Left != null)
        //{
        //    var mid = QuickDraw.MidPointVec3(LeftWall.transform.position, Left.RightWall.transform.position);

        //    QuickDraw.DrawBounds(new Bounds(mid, Vector3.one));


        //    var b = QuickDraw.JoinBounds(new Bounds(LeftWall.transform.position, LevelGenerator.generator.doorSize), new Bounds(Left.RightWall.transform.position, LevelGenerator.generator.doorSize));

        //    QuickDraw.DrawBounds(b);

        //    //Debug.DrawLine(LeftWall.transform.position, Left.RightWall.transform.position);
        //}

        //if (Right != null)
        //{

        //    var mid = QuickDraw.MidPointVec3(RightWall.transform.position, Right.LeftWall.transform.position);

        //    QuickDraw.DrawBounds(new Bounds(mid, Vector3.one));

        //    var b = QuickDraw.JoinBounds(new Bounds(RightWall.transform.position, LevelGenerator.generator.doorSize), new Bounds(Right.LeftWall.transform.position, LevelGenerator.generator.doorSize));

        //    QuickDraw.DrawBounds(b);

        //    //Debug.DrawLine(RightWall.transform.position, Right.LeftWall.transform.position);
        //}


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
            }

            if (exits[i] == Exit.Bottom)
            {
                BottomWall.SetActive(false);
                var d = Instantiate(Door, BottomWall.transform.position, BottomWall.transform.rotation, transform);
                d.SetActive(true);
            }

            if (exits[i] == Exit.Left)
            {
                LeftWall.SetActive(false);
                var d = Instantiate(Door, LeftWall.transform.position, LeftWall.transform.rotation, transform);
                d.SetActive(true);
            }

            if (exits[i] == Exit.Right)
            {
                RightWall.SetActive(false);
                var d = Instantiate(Door, RightWall.transform.position, RightWall.transform.rotation, transform);
                d.SetActive(true);
            }
        }
    }
}
