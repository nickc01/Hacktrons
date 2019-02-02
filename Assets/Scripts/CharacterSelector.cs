using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;

public static class CharacterSelector
{
    private static bool Selecting = false;
    private static bool Waiting = false;
    public static Player Selection { get; set; }
    public static Vector2Int SpawnPosition { get; private set; }

    private static void Enable()
    {
        Game.SidePanel.SetActive(true);
        Dragger.Drag(Game.SidePanel, null, new Vector2(-55,50),3f,Interrupt: true);
    }

    private static void Disable()
    {
        Dragger.Drag(Game.SidePanel, null, new Vector2(105, 50), 3f, Interrupt: true);
    }

    public static void Cancel()
    {
        Waiting = false;
    }

    //Returns a player on successful selection, returns null if unsuccessful
    public static async Task<Player> Select(Vector2Int spawnPosition)
    {
        if (Selecting == true)
        {
            Waiting = false;
            await Tasker.Run(() => { while (Selecting) { } });
            Selecting = false;
        }
        Selecting = true;
        Waiting = true;
        Selection = null;
        SpawnPosition = spawnPosition;
        Enable();
        await Task.Run(() => { while (Selection == null && Waiting == true) { } });
        Selecting = false;
        Waiting = false;
        if (Selection != null)
        {
            Disable();
            return Selection;
        }
        else
        {
            Disable();
            return null;
            //Initiate cancelation protocol
        }
        //TEMPORARY
        //var player = Game.SpawnCharacter<Knife>(SpawnPosition);
        /*var player = GameObject.Instantiate(TileManager.Tiles[3]).GetComponent<Player>();
        player.OnSpawn();*/


        
    }
    public static void UndoSelection(Player player)
    {
        Game.DestroyCharacter(player);
        Game.ToolsPerType[player.GetType()].Undo();
    }
}
