using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SpriteHighlighter : MonoBehaviour
{
    public static Transform SetTransform;
    public static Rect Region = default;
    public static float Z = 0;
    public static SpriteHighlighter highlighter { get; private set; }
    new private static SpriteRenderer renderer;
    bool Direction = true;
    float Transparency = 0f;
    float TransparencySpeed = 10f;
    void Start()
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
        if (SetTransform != null)
        {
            transform.position = SetTransform.position;
            //transform.localScale = SetTransform.lossyScale;
            renderer.size = SetTransform.lossyScale;
        }
        else
        {
            transform.position = new Vector3(Region.xMin - 0.5f + Region.width / 2, Region.yMin - 0.5f + Region.height / 2,Z);
            renderer.size = new Vector2(Region.width, Region.height);
        }
    }
}
