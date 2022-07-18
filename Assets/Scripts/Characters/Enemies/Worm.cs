public class Worm : Enemy
{
    public override string Info => "Is able to store lots of health";

    public override int GetTileID()
    {
        return 7;
    }

    public override int AttackDamage => 3;

    public override int AttackRange => 2;

    public override int MaxTrailLength => 10;

    public override int MovesMax => 3;
}
