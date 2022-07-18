public class Knife : Player
{
    public override string Info => "A well-rounded program";

    public override string Name => "Hack";

    public override int GetStartingAmount()
    {
        return 4;
    }

    public override int AttackDamage => 3;

    public override int AttackRange => 4;

    public override int MaxTrailLength => 4;

    public override int MovesMax => 4;

    public override int GetTileID()
    {
        return 3;
    }
}