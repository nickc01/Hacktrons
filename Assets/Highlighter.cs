using System;
using System.Collections;
using System.Collections.Generic;
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

    new private static Image renderer;
    bool Direction = true;
    float Transparency = 0f;
    float TransparencySpeed = 10f;

    private static Func<Transform> GetRect;

    private static Transform targetRect
    {
        get
        {
            if (GetRect != null)
            {
                return GetRect();
            }
            return default;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        selfRect = GetComponent<RectTransform>();
        MainCanvas = GetComponentInParent<Canvas>();
        renderer = GetComponent<Image>();
        Disable();
    }

    void Update()
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
        //Speed += Time.deltaTime * 2;
        if (GetRect != null)
        {

            var Parent = GetRect();
            if (UIObject)
            {
                transform.SetParent(Parent, true);
                selfRect.anchorMin = Vector2.zero;
                selfRect.anchorMax = Vector2.one;
                selfRect.offsetMax = Vector2.zero;
                selfRect.offsetMin = Vector2.zero;
               // selfRect.anchorMax = Vector2.Lerp(selfRect.anchorMax,Vector2.zero,Speed * Time.deltaTime);
                //selfRect.anchorMin = Vector2.Lerp(selfRect.anchorMin, Vector2.zero, Speed * Time.deltaTime);
                //selfRect.anchorMax = Vector2.Lerp(selfRect.anchorMax,Parent.anchorMax,Speed * Time.deltaTime);
                //selfRect.anchorMin = Vector2.Lerp(selfRect.anchorMin, Parent.anchorMin, Speed * Time.deltaTime);
                //selfRect.anchoredPosition = Vector2.Lerp(selfRect.anchoredPosition,Vector2.zero,Speed * Time.deltaTime);
                //selfRect.sizeDelta = Vector2.Lerp(selfRect.sizeDelta, Parent.sizeDelta, Speed * Time.deltaTime);
                //selfRect.offsetMax = Vector2.Lerp(selfRect.offsetMax, Vector2.zero, Speed * Time.deltaTime);
                //selfRect.offsetMin = Vector2.Lerp(selfRect.offsetMin, Vector2.zero, Speed * Time.deltaTime);
                //selfRect.pivot = Vector2.Lerp(selfRect.pivot, Vector2.one / 2, Speed * Time.deltaTime);
                //selfRect.pivot = Vector2.Lerp(selfRect.pivot, new Vector2(Parent.anchorMax, Parent.anchorMin), Speed * Time.deltaTime);
                /*var Rect = GetRect();

                //selfRect.rect.Set(Rect.x, Rect.y, Rect.width, Rect.height);
                Vector2 position = new Vector2(Mathf.Lerp(selfRect.rect.x, Rect.x - transform.position.x, Speed * Time.deltaTime), Mathf.Lerp(selfRect.rect.y, Rect.y - transform.position.y, Speed * Time.deltaTime));
                Vector2 size = new Vector2(Mathf.Lerp(selfRect.rect.width, Rect.width, Speed * Time.deltaTime), Mathf.Lerp(selfRect.rect.height, Rect.height, Speed * Time.deltaTime));
                //selfRect.rect = new Rect(position, size);
                //selfRect.rect.Set(position.x, position.y, size.x, size.y);
                selfRect.transform.scal*/
            }
            /*else
            {
                transform.SetParent(MainCanvas.transform);
                var ScreenPosition = MainCanvas.worldCamera.WorldToScreenPoint(Parent.transform.position - (Parent.transform.lossyScale / 2));
                Debug.Log("Scale = " + Parent.transform.lossyScale);
                Debug.Log("Screen Position = " + ScreenPosition);
                //selfRect.offsetMax = Vector2.zero;
                //selfRect.offsetMin = Vector2.zero;
                selfRect.anchorMax = Vector2.Lerp(selfRect.anchorMax, Vector2.zero, Speed * Time.deltaTime);
                selfRect.anchorMin = Vector2.Lerp(selfRect.anchorMin, Vector2.zero, Speed * Time.deltaTime);
                //selfRect.pivot = Vector2.Lerp(selfRect.pivot,Vector2.zero,Speed * Time.deltaTime);
                //selfRect.offsetMax = Vector2.Lerp(selfRect.offsetMax, Vector2.zero, Speed * Time.deltaTime);
                //selfRect.offsetMin = Vector2.Lerp(selfRect.offsetMin, Vector2.zero, Speed * Time.deltaTime);
                var ScreenSize = MainCanvas.worldCamera.WorldToScreenPoint(Parent.transform.position + (Parent.transform.lossyScale / 2)) - ScreenPosition;
                //Debug.Log("Screen Size = " + ScreenSize);
                selfRect.anchoredPosition = Vector2.Lerp(selfRect.anchoredPosition, ScreenPosition, Speed * Time.deltaTime);
                selfRect.sizeDelta = Vector2.Lerp(selfRect.sizeDelta, ScreenSize, Speed * Time.deltaTime);
                //selfRect.sizeDelta = Vector2.one;
            }*/
        }
            //position = position - (Vector2)transform.position;
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
        var rect = obj.GetComponent<RectTransform>();
        if (rect == null)
        {
            throw new System.Exception("The GameObject doesn't have a RectTransform");
        }
        HighlightUI(rect);
    }

    public static void HighlightUI(RectTransform rect)
    {
        //rect.rect
        GetRect = () =>
        {
            var newRect = rect.rect;
            var pos = rect.transform.position;
            return rect.transform;
            //return new Rect(rect.transform.position, new Vector2(newRect.width,newRect.height));
        };
        SpriteHighlighter.Disable();
        Highlighter.Enable();
        UIObject = true;
        Speed = 7f;
        var Parent = rect.transform;
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
    public static void HighlightWorldRegion(Rect region,float Z)
    {
        UIObject = false;
        SpriteHighlighter.SetTransform = null;
        SpriteHighlighter.Z = Z;
        SpriteHighlighter.Region = region;
        SpriteHighlighter.Enable();
        Highlighter.Disable();
    }
}
