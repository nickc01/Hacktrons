public class Bug : Enemy
{
    public override string Info => "Moves faster, has more health, but does less damage than normal";

    public override int GetTileID()
    {
        return 5;
    }

    public override int AttackDamage => 2;

    public override int AttackRange => 2;

    public override int MaxTrailLength => 5;

    public override int MovesMax => 6;
}
