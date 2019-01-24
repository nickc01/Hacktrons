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

    //Returns a player on successful selection, returns null if unsuccessful
    public static async Task<Player> Select(Vector2Int SpawnPosition)
    {
        if (Selecting == true)
        {
            Waiting = false;
            await Task.Run(() => { while (Selecting) { } });
            Selecting = false;
        }
        Selecting = true;
        Waiting = true;
        //TEMPORARY
        var player = TileManager.SpawnCharacter<Knife>(SpawnPosition);
        /*var player = GameObject.Instantiate(TileManager.Tiles[3]).GetComponent<Player>();
        player.OnSpawn();*/


        Selecting = false;
        Waiting = false;
        return player;
    }
    public static void UndoSelection(Player player)
    {
        TileManager.DestroyCharacter(player);
    }
}
