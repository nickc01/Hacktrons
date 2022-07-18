using UnityEngine;

public class Trail : MonoBehaviour
{
    public Character Host;
    private Color? previousColor = null;
    private float MinAlpha = 128;
    private float T = 0;
    private bool Direction = true;
    private new SpriteRenderer renderer;
    private bool flash = false;
    private float Speed = 3f;
    public bool Flash
    {
        get => flash;
        set
        {
            if (value != flash)
            {
                if (value == true)
                {
                    previousColor = GetComponent<SpriteRenderer>().color;
                }
                else
                {
                    GetComponent<SpriteRenderer>().color = previousColor.Value;
                    previousColor = null;
                }
                flash = value;
            }
        }
    }

    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Flash)
        {
            if (Direction == true)
            {
                T += Time.deltaTime * Speed;
                if (T > 1)
                {
                    T = 1;
                    Direction = false;
                }
            }
            else
            {
                T -= Time.deltaTime * Speed;
                if (T < 0)
                {
                    T = 0;
                    Direction = true;
                }
            }
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, Mathf.Lerp(1f, 0.5f, T));
        }
    }
}
