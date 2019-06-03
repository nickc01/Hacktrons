using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Bug : Enemy
{
    public override string Info => "Moves faster, has more health, but does less damage than normal";

    public override int GetTileID() => 5;

    public override int AttackDamage => 2;

    public override int AttackRange => 2;

    public override int MaxTrailLength => 5;

    public override int MovesMax => 6;
}
