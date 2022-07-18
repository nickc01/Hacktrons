using UnityEngine;

public class Droid : Enemy
{
    public override int GetTileID()
    {
        return 4;
    }

    public override int AttackDamage => 3;
    public override int MaxTrailLength => 4;
    public override int AttackRange => 4;
    public override int MovesMax => 3;

    private static bool loaded = false;
    private static Sprite ASprite;
    private static Sprite BSprite;

    private bool SpriteMode = false;
    private float T = 0;

    public override string Info => "A basic enemy";
    public override string Name => "Bot";

    private void Start()
    {
        if (!loaded)
        {
            loaded = true;
            ASprite = Resources.Load<Sprite>("Characters/Enemies/Droid/DroidA");
            BSprite = Resources.Load<Sprite>("Characters/Enemies/Droid/DroidB");
        }
    }

    private void Update()
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
