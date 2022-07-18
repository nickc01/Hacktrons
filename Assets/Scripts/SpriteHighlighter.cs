using UnityEngine;

public class SpriteHighlighter : MonoBehaviour
{
    public static Transform SetTransform;
    public static Rect Region = default;
    public static float Z = 0;
    public static SpriteHighlighter highlighter { get; private set; }
    private static new SpriteRenderer renderer;
    private bool Direction = true;
    private float Transparency = 0f;
    private float TransparencySpeed = 10f;

    private void Start()
    {
        highlighter = this;
        renderer = GetComponent<SpriteRenderer>();
        Disable();
    }

    public static void Enable()
    {
        highlighter.gameObject.SetActive(true);
    }

    public static void Disable()
    {
        highlighter.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Direction)
        {
            Transparency += Time.deltaTime * TransparencySpeed;
            if (Transparency > 1)
            {
                Transparency = 1f;
                Direction = false;
            }
        }
        else
        {
            Transparency -= Time.deltaTime * TransparencySpeed;
            if (Transparency < 0)
            {
                Transparency = 0f;
                Direction = true;
            }
        }
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, Transparency);
        if (SetTransform != null)
        {
            transform.position = SetTransform.position;
            renderer.size = SetTransform.lossyScale;
        }
        else
        {
            transform.position = new Vector3(Region.xMin - 0.5f + Region.width / 2, Region.yMin - 0.5f + Region.height / 2, Z);
            renderer.size = new Vector2(Region.width, Region.height);
        }
    }
}
