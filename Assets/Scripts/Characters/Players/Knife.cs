using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Knife : Player
{
    public override string Info => "A well-rounded droid";

    public override string Name => "Hack";

    public override int GetStartingAmount() => 1;

    public override int GetTileID() => 3;
}