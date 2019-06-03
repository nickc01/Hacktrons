using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Knife : Player
{
    public override string Info => "A well-rounded program";

    public override string Name => "Hack";

    public override int GetStartingAmount() => 4;

    public override int AttackDamage => 3;

    public override int AttackRange => 4;

    public override int MaxTrailLength => 4;

    public override int MovesMax => 4;

    public override int GetTileID() => 3;
}