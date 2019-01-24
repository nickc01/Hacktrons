using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public virtual void OnSpawn() { }
    public virtual bool Collidable { get; } = false;
    public virtual void OnDestruction() { }
}