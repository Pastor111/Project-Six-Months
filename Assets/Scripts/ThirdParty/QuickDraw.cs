using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RenderCommand
{
    public string text;
    public Vector3[] points;
    public float LineWidth;
    public Vector3 Position;
    public Color color;
    public Vector3 scale;
    public bool Loop;
}

public class QuickDraw : MonoBehaviour
{
    public Material defaultMaterial;
    public TMPro.TMP_FontAsset font;

    static QuickDraw instance;

    static List<RenderCommand> _draws = new List<RenderCommand>();

    LineRenderer[] lines = new LineRenderer[0];
    TMPro.TextMeshPro[] texts = new TMPro.TextMeshPro[0];

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public static void RemoveAllDrawings()
    {
        instance.LateUpdate();

        for (int i = 0; i < instance.lines.Length; i++)
        {
            DestroyImmediate(instance.lines[i].gameObject);
        }

        for (int i = 0; i < instance.texts.Length; i++)
        {
            DestroyImmediate(instance.texts[i].gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < instance.lines.Length; i++)
        {
            Destroy(instance.lines[i].gameObject);
        }

        for (int i = 0; i < instance.texts.Length; i++)
        {
            Destroy(instance.texts[i].gameObject);
        }
    }

    private void LateUpdate()
    {
        //if(lines != null)
        //{


        //}

        var llist = new List<LineRenderer>();
        var tlist = new List<TMPro.TextMeshPro>();

        for (int i = 0; i < _draws.Count; i++)
        {

            if (string.IsNullOrEmpty(_draws[i].text))
            {
                GameObject l = new GameObject($"Line {i}");
                var line = l.AddComponent<LineRenderer>();
                line.positionCount = _draws[i].points.Length;
                line.SetPositions(_draws[i].points);
                line.startWidth = _draws[i].LineWidth;
                line.material = defaultMaterial;
                line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                line.endWidth = _draws[i].LineWidth;
                line.startColor = _draws[i].color;
                line.endColor = _draws[i].color;
                line.loop = _draws[i].Loop;
                l.transform.position = _draws[i].Position;
                line.useWorldSpace = false;

                llist.Add(line);
            }
            else
            {
                GameObject l = new GameObject($"Text {i}");
                var text = l.AddComponent<TMPro.TextMeshPro>();
                text.text = _draws[i].text;
                text.font = font;
                text.color = _draws[i].color;
                text.enableAutoSizing = true;
                text.alignment = TMPro.TextAlignmentOptions.Center;
                text.transform.localScale = _draws[i].scale;
                text.transform.position = _draws[i].Position;
                tlist.Add(text);
            }


        }

        lines = llist.ToArray();
        texts = tlist.ToArray();

        _draws.Clear();
    }

    #region Draw

    #region Line

    public static void DrawLine(Vector3 start, Vector3 end, Vector3 WorldPosition)
    {
        DrawLine(new Vector3[] { start, end }, 0.5f, Color.white, WorldPosition);
    }

    public static void DrawLine(Vector3 start, Vector3 end, Color color, Vector3 WorldPosition)
    {
        DrawLine(new Vector3[] { start, end }, 0.5f, color, WorldPosition);
    }

    public static void DrawLine(Vector3 start, Vector3 end, float width, Color color, Vector3 WorldPosition)
    {
        DrawLine(new Vector3[] { start, end }, width, color, WorldPosition);
    }

    public static void DrawLine(Vector3[] points, float LineWidth, Color color, Vector3 worldPosition)
    {
        RenderCommand cmd = new RenderCommand()
        {
            points = points,
            color = color,
            LineWidth = LineWidth,
            Position = worldPosition,
            Loop = false,
        };

        _draws.Add(cmd);
    }

    #endregion

    #region Triangle

    public static void DrawTriangle(Vector3 worldPosition, Color col, float LineWidth)
    {
        Vector3[] positions = new Vector3[3] { new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 1, 0) };

        RenderCommand cmd = new RenderCommand()
        {
            points = positions,
            color = col,
            LineWidth = LineWidth,
            Position = worldPosition,
            Loop = true,
        };

        _draws.Add(cmd);
    }

    #endregion

    #region Square

    public static void DrawSquare(Vector3 worldPosition, Color col, float LineWidth)
    {
        Vector3[] positions = new Vector3[4] { new Vector3(0, 0, 0), new Vector3(1f, 0, 0), new Vector3(1f, 1f, 0), new Vector3(0, 1f) };

        RenderCommand cmd = new RenderCommand()
        {
            points = positions,
            color = col,
            LineWidth = LineWidth,
            Position = worldPosition - new Vector3(0.5f, 0.5f),
            Loop = true,
        };

        _draws.Add(cmd);
    }

    public static void DrawCube(Vector3 worldPosition, Color col, float LineWidth)
    {
        Vector3[] positions = new Vector3[] { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 1), new Vector3(1, 1, 1), new Vector3(0, 1, 1) };

        RenderCommand cmd = new RenderCommand()
        {
            points = positions,
            color = col,
            LineWidth = LineWidth,
            Position = worldPosition,
            Loop = true,
        };

        _draws.Add(cmd);
    }


    #endregion

    #region Circle

    public static void DrawCircle(int segments, float radius, Vector3 worldPosition, Color col, float LineWidth)
    {

        float angle = 2 * Mathf.PI / segments;


        Vector3[] positions = new Vector3[segments];


        for (int i = 0; i < segments; i++)
        {
            Matrix4x4 rotationMatrix = new Matrix4x4(new Vector4(Mathf.Cos(angle * i), Mathf.Sin(angle * i), 0, 0),
                                                     new Vector4(-1 * Mathf.Sin(angle * i), Mathf.Cos(angle * i), 0, 0),
                                       new Vector4(0, 0, 1, 0),
                                       new Vector4(0, 0, 0, 1));
            Vector3 initialRelativePosition = new Vector3(0, radius, 0);
            positions[i] = Vector3.zero + rotationMatrix.MultiplyPoint(initialRelativePosition);

        }

        RenderCommand cmd = new RenderCommand()
        {
            points = positions,
            color = col,
            LineWidth = LineWidth,
            Position = worldPosition,
            Loop = true,
        };

        _draws.Add(cmd);
    }


    #endregion

    public static void DrawBounds(Bounds b)
    {

        var startMousePos = b.min;
        var finalPosition = b.max;
        finalPosition.z = b.min.z;
        var p0 = new Vector3(startMousePos.x, finalPosition.y, finalPosition.z);
        var p1 = new Vector3(finalPosition.x, startMousePos.y, finalPosition.z);

        QuickDraw.DrawLine(startMousePos, p0, 0.1f, Color.yellow, Vector3.zero);
        QuickDraw.DrawLine(p0, finalPosition, 0.1f, Color.yellow, Vector3.zero);
        QuickDraw.DrawLine(finalPosition, p1, 0.1f, Color.yellow, Vector3.zero);
        QuickDraw.DrawLine(p1, startMousePos, 0.1f, Color.yellow, Vector3.zero);


    }

    public static Bounds JoinBounds(Bounds left, Bounds right)
    {
        Bounds b = new Bounds();
        b.center = MidPointVec3(left.center, right.center);
        b.SetMinMax(left.min, right.max);
        b.Encapsulate(left.min);
        b.Encapsulate(new Vector3(right.max.x, left.min.y, left.min.z));
        b.Encapsulate(new Vector3(left.min.x, right.max.y, left.min.z));
        b.Encapsulate(right.max);

        return b;

    }

    public static Vector2 MidPoint(Vector2 a, Vector2 b)
    {
        float x = (a.x + b.x) / 2;
        float y = (a.y + b.y) / 2;

        return new Vector2(x, y);
    }

    public static Vector3 MidPointVec3(Vector3 a, Vector3 b)
    {
        float x = (a.x + b.x) / 2;
        float y = (a.y + b.y) / 2;
        float z = (a.z + b.z) / 2;

        return new Vector3(x, y, z);
    }

    public static void DrawBounds(Bounds b, Color col)
    {

        var startMousePos = b.min;
        var finalPosition = b.max;
        finalPosition.z = b.min.z;
        var p0 = new Vector3(startMousePos.x, finalPosition.y, finalPosition.z);
        var p1 = new Vector3(finalPosition.x, startMousePos.y, finalPosition.z);

        QuickDraw.DrawLine(startMousePos, p0, 0.1f, col, Vector3.zero);
        QuickDraw.DrawLine(p0, finalPosition, 0.1f, col, Vector3.zero);
        QuickDraw.DrawLine(finalPosition, p1, 0.1f, col, Vector3.zero);
        QuickDraw.DrawLine(p1, startMousePos, 0.1f, col, Vector3.zero);


    }

    public static void DrawText(string text, Vector3 position, Color col, float scale)
    {
        RenderCommand cmd = new RenderCommand()
        {
            text = text,
            Position = position,
            color = col,
            scale = new Vector3(scale, scale, scale),
        };

        _draws.Add(cmd);
    }

    #endregion


}
