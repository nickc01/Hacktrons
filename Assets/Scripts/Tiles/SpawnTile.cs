using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTile : Tile
{
    public static List<SpawnTile> SpawnTiles = new List<SpawnTile>();
    public Player SelectedPlayer { get; private set; } = null;
    public static SpawnTile SelectedTile { get; private set; } = null;

    public Action TutorialOnClick;

    public override void OnSpawn()
    {
        base.OnSpawn();
        SpawnTiles.Add(this);
        gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
        Game.GameStartEvent += OnGameStart;
    }

    public override int GetTileID()
    {
        return 1;
    }

    private void OnGameStart()
    {
        SpawnTiles.Remove(this);
        Vector2 Position = transform.position;
        Game.DestroyTile(new Vector2Int((int)Position.x, (int)Position.y));
        Game.SpawnTile<BasicTile>(new Vector2Int((int)Position.x, (int)Position.y));
    }

    public override void OnDestruction()
    {
        Game.GameStartEvent -= OnGameStart;
        SpawnTiles.Remove(this);
        base.OnDestruction();
    }

    public void OnMouseDown()
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

                    IEnumerator SelectCharacter()
                    {
                        yield return CharacterSelector.Select(new Vector2Int((int)transform.position.x, (int)transform.position.y));
                        if (CharacterSelector.SelectedPlayer != null)
                        {
                            SelectedPlayer = CharacterSelector.SelectedPlayer;
                            if (Player.Players.Count == 1)
                            {
                                //Game.StartGameButton
                                Game.StartGameButton.gameObject.SetActive(true);
                                GlobalRoutine.Start(Dragger.Drag(Game.StartGameButton.gameObject, null, new Vector2(-100, 35), 1.3f, LerpManager.SmoothOut, true));
                            }
                            SelectedPlayer.transform.position = new Vector3(transform.position.x, transform.position.y, SelectedPlayer.transform.position.z);
                            Game.SpawnTileSelector.SetActive(false);
                            SelectedTile = null;
                        }
                    }

                    StartCoroutine(SelectCharacter());
                }
                else
                {
                    CharacterSelector.UndoSelection(SelectedPlayer);
                    if (Player.Players.Count == 0)
                    {
                        GlobalRoutine.Start(Dragger.Drag(Game.StartGameButton.gameObject, null, new Vector2(100, 35), 1.3f, LerpManager.SmoothOut, true));
                    }
                    SelectedPlayer = null;
                    Game.SpawnTileSelector.SetActive(false);
                    SelectedTile = null;
                }
            }
        }
    }
}
