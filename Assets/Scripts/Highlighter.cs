using System;
using UnityEngine;
using UnityEngine.UI;

public class Highlighter : MonoBehaviour
{
    private static bool UIObject = false;
    private static Canvas MainCanvas;
    public static Highlighter Instance;
    private static RectTransform selfRect;
    private static SpriteHighlighter spriteHighlighter => SpriteHighlighter.highlighter;
    private static float MinSpeed = 7f;
    private static float Speed = 7f;

    private static new Image renderer;
    private bool Direction = true;
    private float Transparency = 0f;
    private float TransparencySpeed = 10f;

    private static Func<Transform> GetRect;

    // Start is called before the first frame update
    private void Start()
    {
        Instance = this;
        selfRect = GetComponent<RectTransform>();
        MainCanvas = GetComponentInParent<Canvas>();
        renderer = GetComponent<Image>();
        Disable();
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
        if (GetRect != null)
        {

            Transform Parent = GetRect();
            if (UIObject)
            {
                transform.SetParent(Parent, true);
                selfRect.anchorMin = Vector2.zero;
                selfRect.anchorMax = Vector2.one;
                selfRect.offsetMax = Vector2.zero;
                selfRect.offsetMin = Vector2.zero;
            }
        }
    }

    public static void Enable()
    {
        Instance.gameObject.SetActive(true);
    }

    public static void Disable()
    {
        Instance.gameObject.SetActive(false);
    }

    public static void EnableAll()
    {
        Enable();
        SpriteHighlighter.Enable();
    }
    public static void DisableAll()
    {
        Disable();
        SpriteHighlighter.Disable();
    }

    public static void HighlightUI(GameObject obj)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        if (rect == null)
        {
            throw new System.Exception("The GameObject doesn't have a RectTransform");
        }
        HighlightUI(rect);
    }

    public static void HighlightUI(RectTransform rect)
    {
        GetRect = () =>
        {
            Rect newRect = rect.rect;
            Vector3 pos = rect.transform.position;
            return rect.transform;
        };
        SpriteHighlighter.Disable();
        Highlighter.Enable();
        UIObject = true;
        Speed = 7f;
        Transform Parent = rect.transform;
        Instance.transform.SetParent(Parent, true);
        selfRect.anchorMin = Vector2.zero;
        selfRect.anchorMax = Vector2.one;
        selfRect.offsetMax = Vector2.zero;
        selfRect.offsetMin = Vector2.zero;
    }
    public static void HighlightSprite(GameObject gm)
    {
        UIObject = false;
        SpriteHighlighter.SetTransform = gm.transform;
        SpriteHighlighter.Z = gm.transform.position.z;
        SpriteHighlighter.Region = default;
        SpriteHighlighter.Enable();
        Highlighter.Disable();

        Speed = 7f;
    }
    public static void HighlightWorldRegion(Rect region, float Z)
    {
        UIObject = false;
        SpriteHighlighter.SetTransform = null;
        SpriteHighlighter.Z = Z;
        SpriteHighlighter.Region = region;
        SpriteHighlighter.Enable();
        Highlighter.Disable();
    }
}
