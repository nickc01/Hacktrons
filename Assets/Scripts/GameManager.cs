using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class GameManager
{
    public static event Action GameStartEvent;
    public static bool GameStarted { get; private set; } = false;

    public static async void StartGame()
    {
        //Debug.Log("Game Started = " + GameStarted);
        if (!GameStarted)
        {
           // Debug.Log("Starting Game");
            Pane.GetPane("Pre Game").gameObject.SetActive(false);
            Pane.GetPane("Game").gameObject.SetActive(true);
            GameStarted = true;
            GameStartEvent?.Invoke();
            while (true)
            {
                //Player.Players.Count > 0 && Enemy.Enemies.Count > 0
                await Player.BeginTurns();
                if (Enemy.Enemies.Count == 0)
                {
                    Debug.Log("YOU WIN");
                    Win();
                    break;
                }
                //Debug.Log("BEGINNING ENEMY TURNS");
                await Enemy.BeginTurns();
                if (Player.Players.Count == 0)
                {
                    Debug.Log("YOU LOSE");
                    Lose();
                    break;
                }
            }
        }
    }
    public static void StopGame()
    {
        GameStarted = false;
        GameStartEvent = null;
        CameraTarget.Movable = false;
        Pane.GetPane("Game").gameObject.SetActive(false);
    }

    public static void ResetToSelectionScreen()
    {
        CameraTarget.Active = true;
        TileManager.DeleteMap();
        Pane.GetPane("Results Screen").gameObject.SetActive(false);
        Pane.GetPane("Select Level").gameObject.SetActive(true);
    }

    private static void Win()
    {
        StopGame();
        Pane.GetPane("Results Screen").gameObject.SetActive(true);
    }
    private static void Lose()
    {
        StopGame();
        Pane.GetPane("Results Screen").gameObject.SetActive(true);
    }
}
