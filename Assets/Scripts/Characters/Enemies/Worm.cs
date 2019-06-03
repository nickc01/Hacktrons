using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Worm : Enemy
{
    public override string Info => "Is able to store lots of health";

    public override int GetTileID() => 7;

    public override int AttackDamage => 3;

    public override int AttackRange => 2;

    public override int MaxTrailLength => 10;

    public override int MovesMax => 3;
}
