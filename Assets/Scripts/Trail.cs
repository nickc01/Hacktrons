using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Trail : MonoBehaviour
{
    public Character Host;
    Color? previousColor = null;
    float MinAlpha = 128;
    float T = 0;
    bool Direction = true;
    SpriteRenderer renderer;
    private bool flash = false;
    float Speed = 3f;
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

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }
    
    void Update()
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
            renderer.color = new Color(renderer.color.r,renderer.color.g,renderer.color.b,Mathf.Lerp(1f,0.5f,T));
        }
    }
}
