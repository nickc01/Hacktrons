public class Spy : Enemy
{
    public override string Info => "Moves slow, doesn't have much health, but has a large attack range";

    public override int GetTileID()
    {
        return 6;
    }

    public override int AttackDamage => 1;

    public override int AttackRange => 7;

    public override int MaxTrailLength => 2;

    public override int MovesMax => 1;
}
