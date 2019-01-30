using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour, IHasID
{
    public virtual void OnSpawn() { }
    public virtual bool Collidable { get; } = false;
    public virtual void OnDestruction() { }

    public abstract int GetTileID();

    public static T Spawn<T>(Vector2Int Position) where T : Tile
    {
        return TileManager.SpawnTile<T>(Position);
    }
}