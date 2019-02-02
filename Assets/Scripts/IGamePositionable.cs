using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IGamePositionable
{
    Vector2Int GamePosition { get; set; }
}

