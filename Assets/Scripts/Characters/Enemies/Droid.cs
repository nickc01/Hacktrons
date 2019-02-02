using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Droid : Enemy
{
    public override int GetTileID() => 4;

    private static bool loaded = false;
    private static Sprite ASprite;
    private static Sprite BSprite;

    private bool SpriteMode = false;
    private float T = 0;

    public override string Info => "A basic enemy";
    public override string Name => "Bot";

    void Start()
    {
        if (!loaded)
        {
            loaded = true;
            ASprite = Resources.Load<Sprite>("Characters/Enemies/Droid/DroidA");
            BSprite = Resources.Load<Sprite>("Characters/Enemies/Droid/DroidB");
            //Debug.Log("ASprite = " + ASprite);
           // Debug.Log("BSprite = " + BSprite);
        }
    }

    void Update()
    {
        T += Time.deltaTime;
        if (T >= 1)
        {
            T = 0;
            SpriteMode = !SpriteMode;
            if (SpriteMode == true)
            {
                GetComponent<SpriteRenderer>().sprite = ASprite;
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = BSprite;
            }
        }
    }
}
