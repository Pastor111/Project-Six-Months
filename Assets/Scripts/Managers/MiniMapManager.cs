using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapManager : MonoBehaviour
{
    public Image MiniMap;

    public Texture2D ChestIcon;
    public Texture2D InteractionIcon;
    public Texture2D KeyIcon;
    public Texture2D BossFight;

    public float Spacing;
    public float UIScaling;
    public float SpacingMultiplier;
    public Vector2 UIoffset;

    static MiniMapManager instance;

    public static MiniMapManager GetMiniMap() { if (instance != null) return instance; else return FindObjectOfType<MiniMapManager>(); }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeRoomColor(Texture2D t,int x, int y, Color color)
    {
        t.SetPixel(x + 1, y, color);
        t.SetPixel(x - 1, y, color);
        t.SetPixel(x, y + 1, color);
        t.SetPixel(x, y - 1, color);
        t.SetPixel(x, y, color);
        t.SetPixel(x + 1, y + 1, color);
        t.SetPixel(x - 1, y - 1, color);
        t.SetPixel(x + 1, y - 1, color);
        t.SetPixel(x - 1, y + 1, color);
    }

    public GameObject PlaceItemInMiniMap(int roomLevel, Texture2D t, Color col, Vector3 scale)
    {
        Vector2 zero = new Vector2(-75f, -75f);

        int x = roomLevel / 5;
        int y = (roomLevel - (x * 5)) * 40;

        zero.x += 40 * Mathf.RoundToInt(x);
        zero.y += y;

        //float xPos = pos.x / (Spacing * SpacingMultiplier);
        //float yPos = pos.z / (Spacing * SpacingMultiplier);


        //xPos -= UIoffset.x;
        //yPos -= UIoffset.y;

        GameObject icon = new GameObject("Icon UI");
        icon.transform.parent = MiniMap.transform.parent;
        icon.AddComponent<Image>().sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero);

        RectTransform rt = (RectTransform)icon.transform;
        rt.localScale = scale;
        rt.anchoredPosition = new Vector3(zero.x * UIScaling, zero.y * UIScaling);

        return icon;
    }

    public GameObject PlaceItemInMiniMap(Vector3 pos, Texture2D t, Color col, Vector3 scale)
    {
        float xPos = pos.x / (Spacing * SpacingMultiplier);
        float yPos = pos.z / (Spacing * SpacingMultiplier);


        xPos -= UIoffset.x;
        yPos -= UIoffset.y;

        GameObject icon = new GameObject("Icon UI");
        icon.transform.parent = MiniMap.transform.parent;
        icon.AddComponent<Image>().sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero);

        RectTransform rt = (RectTransform)icon.transform;
        rt.localScale = scale;
        rt.anchoredPosition = new Vector3(xPos * UIScaling, yPos * UIScaling);

        return icon;
    }

    public void DeleteAllIcons()
    {
        foreach (Transform child in MiniMap.transform)
        {
            if (child.name == "Icon UI")
                Destroy(child.gameObject);
        }
    }

    public void GenerateMiniMap(GridPathFinder.Grid grid)
    {
        var t = new Texture2D(grid.Width * 4 + 1, grid.Height * 4 + 1);
        t.filterMode = FilterMode.Point;

        int i = 0;

        FillTexture(t, Color.black);

        for (int x = 2; x < t.width; x += 4)
        {
            for (int y = 2; y < t.height; y += 4)
            {

                if (grid.cells[i].data == 0)
                {
                    ChangeRoomColor(t, x, y, Color.white);
                    //t.SetPixel(x + 1, y, Color.white);
                    //t.SetPixel(x - 1, y, Color.white);
                    //t.SetPixel(x, y + 1, Color.white);
                    //t.SetPixel(x, y - 1, Color.white);
                    //t.SetPixel(x, y, Color.white);
                    //t.SetPixel(x + 1, y + 1, Color.white);
                    //t.SetPixel(x - 1, y - 1, Color.white);
                    //t.SetPixel(x + 1, y - 1, Color.white);
                    //t.SetPixel(x - 1, y + 1, Color.white);
                }
                else if(grid.cells[i].data == 2)
                {
                    ChangeRoomColor(t, x, y, Color.blue);
                }
                else
                {
                    ChangeRoomColor(t, x, y, Color.black);
                    //t.SetPixel(x + 1, y, Color.black);
                    //t.SetPixel(x - 1, y, Color.black);
                    //t.SetPixel(x, y + 1, Color.black);
                    //t.SetPixel(x, y - 1, Color.black);
                    //t.SetPixel(x, y, Color.black);
                    //t.SetPixel(x + 1, y + 1, Color.black);
                    //t.SetPixel(x - 1, y - 1, Color.black);
                    //t.SetPixel(x + 1, y - 1, Color.black);
                    //t.SetPixel(x - 1, y + 1, Color.black);
                }


                i++;

            }
        }

        for (int x = 0; x < t.width; x += 4)
        {
            for (int y = 2; y < t.height; y += 4)
            {
                if (x == 0 || y == 0)
                    continue;

                if ((t.GetPixel(x + 1, y) != Color.black || t.GetPixel(x, y + 1) != Color.black) && (t.GetPixel(x - 1, y) != Color.black || t.GetPixel(x, y - 1) != Color.black))
                {
                    t.SetPixel(x, y, Color.gray);
                }
            }
        }

        for (int x = 2; x < t.width; x += 4)
        {
            for (int y = 0; y < t.height; y += 4)
            {
                if (x == 0 || y == 0)
                    continue;

                if ((t.GetPixel(x + 1, y) != Color.black || t.GetPixel(x, y + 1) != Color.black) && (t.GetPixel(x - 1, y) != Color.black || t.GetPixel(x, y - 1) != Color.black))
                {
                    t.SetPixel(x, y, Color.gray);
                }
            }
        }

        ChangeRoomColor(t, (int)WorldPositionToRoom(LevelGenerator.generator.FirstRoom.transform.position).x, (int)WorldPositionToRoom(LevelGenerator.generator.FirstRoom.transform.position).y, Color.green);
        ChangeRoomColor(t, (int)WorldPositionToRoom(LevelGenerator.generator.LastRoom.transform.position).x, (int)WorldPositionToRoom(LevelGenerator.generator.LastRoom.transform.position).y, Color.red);

        PlaceItemInMiniMap(int.Parse(LevelGenerator.generator.LastRoom.transform.name), ChestIcon, Color.red, new Vector3(0.2f, 0.2f));

        t.Apply();

        //System.IO.File.WriteAllBytes(Application.dataPath + "/Level.png", t.EncodeToPNG());

        //myGridTexture.SetPixel((int)(FirstRoom.transform.position.x / Spacing), (int)(FirstRoom.transform.position.z / Spacing), Color.green);
        //myGridTexture.SetPixel((int)(LastRoom.transform.position.x / Spacing), (int)(LastRoom.transform.position.z / Spacing), Color.red);



        MiniMap.sprite = Sprite.Create(t, new Rect(0, 0, t.width,t.height), new Vector2(0, 0));
    }

    public Vector2 WorldPositionToRoom(Vector3 pos)
    {
        Vector2 zero = new Vector2(2, 2);
        var normalized = pos /= LevelGenerator.generator.Spacing;

        zero.x += 4 * normalized.x;
        zero.y += 4 * normalized.z;

        return zero;
    }

    public void FillTexture(Texture2D t, Color c)
    {
        for (int x = 0; x < t.width; x++)
        {
            for (int y = 0; y < t.height; y++)
            {
                t.SetPixel(x, y, c);
            }
        }
    }
}
