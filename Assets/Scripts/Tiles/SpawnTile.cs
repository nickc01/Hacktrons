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
    public static SpawnTile SelectedTile { get; private set; } = null;

    //public bool ActiveForTutorial { get; set; } = false;
    public Action TutorialOnClick;

    public override void OnSpawn()
    {
        base.OnSpawn();
        SpawnTiles.Add(this);
        CDebug.Log(SpawnTiles.Count);
        gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
        Game.GameStartEvent += OnGameStart;
    }

    public override int GetTileID() => 1;

    void OnGameStart()
    {
        //CDebug.Log("SPAWNTILES = " + SpawnTiles.Count);
        SpawnTiles.Remove(this);
        //CDebug.Log("TILE IS NULL = " + (this == null));
        //CDebug.Log("Transform = " + transform);
        //CDebug.Log("Position = " + transform.position);
        Vector2 Position = transform.position;
        Game.DestroyTile(new Vector2Int((int)Position.x,(int)Position.y));
        Game.SpawnTile<BasicTile>(new Vector2Int((int)Position.x,(int)Position.y));
    }

    public override void OnDestruction()
    {
        Game.GameStartEvent -= OnGameStart;
        SpawnTiles.Remove(this);
        base.OnDestruction();
    }

    public async void OnMouseDown()
    {
        if (Game.ZoomedOnGame)
        {
            if (!TutorialRoutine.TutorialActive || (TutorialRoutine.TutorialActive && TutorialOnClick != null))
            {
                if (TutorialOnClick != null)
                {
                    TutorialOnClick?.Invoke();
                    TutorialOnClick = null;
                }
                if (SelectedTile == this)
                {
                    SelectedTile = null;
                    Game.SpawnTileSelector.SetActive(false);
                    CharacterSelector.Cancel();
                    return;
                }
                if (SelectedPlayer == null)
                {
                    SelectedTile = this;
                    Game.SpawnTileSelector.transform.position = transform.position;
                    Game.SpawnTileSelector.SetActive(true);
                    var Player = await CharacterSelector.Select(new Vector2Int((int)transform.position.x, (int)transform.position.y));
                    if (Player != null)
                    {
                        SelectedPlayer = Player;
                        if (Player.Players.Count == 1)
                        {
                            //Game.StartGameButton
                            Game.StartGameButton.gameObject.SetActive(true);
                            Dragger.Drag(Game.StartGameButton.gameObject, null, new Vector2(-100, 35), 1.3f, LerpManager.SmoothOut, true);
                        }
                        Player.transform.position = new Vector3(transform.position.x, transform.position.y, Player.transform.position.z);
                        Game.SpawnTileSelector.SetActive(false);
                        SelectedTile = null;
                    }
                }
                else
                {
                    CharacterSelector.UndoSelection(SelectedPlayer);
                    if (Player.Players.Count == 0)
                    {
                        //Game.StartGameButton
                        Dragger.Drag(Game.StartGameButton.gameObject, null, new Vector2(100, 35), 1.3f, LerpManager.SmoothOut, true);
                    }
                    SelectedPlayer = null;
                    Game.SpawnTileSelector.SetActive(false);
                    SelectedTile = null;
                }
            }
        }
    }
}
