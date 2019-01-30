using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnTile : Tile
{
    public static List<SpawnTile> SpawnTiles = new List<SpawnTile>();
    public Player SelectedPlayer { get; private set; } = null;
    public override void OnSpawn()
    {
        base.OnSpawn();
        SpawnTiles.Add(this);
        CDebug.Log(SpawnTiles.Count);
        gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
        GameManager.GameStartEvent += OnGameStart;
    }

    public override int GetTileID() => 1;

    void OnGameStart()
    {
        CDebug.Log("SPAWNTILES = " + SpawnTiles.Count);
        SpawnTiles.Remove(this);
        CDebug.Log("TILE IS NULL = " + (this == null));
        CDebug.Log("Transform = " + transform);
        CDebug.Log("Position = " + transform.position);
        Vector2 Position = transform.position;
        TileManager.DestroyTile(new Vector2Int((int)Position.x,(int)Position.y));
        TileManager.SpawnTile<BasicTile>(new Vector2Int((int)Position.x,(int)Position.y));
    }

    public override void OnDestruction()
    {
        GameManager.GameStartEvent -= OnGameStart;
        SpawnTiles.Remove(this);
        base.OnDestruction();
    }

    public async void OnMouseDown()
    {
        if (SelectedPlayer == null)
        {
            var Player = await CharacterSelector.Select(new Vector2Int((int)transform.position.x, (int)transform.position.y));
            SelectedPlayer = Player;
            Player.transform.position = new Vector3(transform.position.x,transform.position.y,Player.transform.position.z);
        }
        else
        {
            CharacterSelector.UndoSelection(SelectedPlayer);
            SelectedPlayer = null;
        }
    }
}
