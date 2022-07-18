using System.Collections;
using UnityEngine;

public static class CharacterSelector
{
    public static Player SelectedPlayer { get; private set; }

    private static bool Selecting = false;
    private static bool Waiting = false;
    public static Player Selection { get; set; }
    public static Vector2Int SpawnPosition { get; private set; }

    private static void Enable()
    {
        Game.SidePanel.SetActive(true);
        GlobalRoutine.Start(Dragger.Drag(Game.SidePanel, null, new Vector2(-55, 50), 3f, Interrupt: true));
    }

    private static void Disable()
    {
        GlobalRoutine.Start(Dragger.Drag(Game.SidePanel, null, new Vector2(105, 50), 3f, Interrupt: true));
    }

    public static void Cancel()
    {
        Waiting = false;
    }

    //Returns a player on successful selection, returns null if unsuccessful
    public static IEnumerator Select(Vector2Int spawnPosition)
    {
        if (Selecting == true)
        {
            Waiting = false;
            yield return new WaitUntil(() => !Selecting);
            Selecting = false;
        }
        Selecting = true;
        Waiting = true;
        Selection = null;
        SpawnPosition = spawnPosition;
        Enable();
        yield return new WaitUntil(() => !(Selection == null && Waiting == true));
        Selecting = false;
        Waiting = false;
        if (Selection != null)
        {
            Disable();
            SelectedPlayer = Selection;
        }
        else
        {
            Disable();
            SelectedPlayer = null;
        }



    }
    public static void UndoSelection(Player player)
    {
        Game.DestroyCharacter(player);
        Game.ToolsPerType[player.GetType()].Undo();
    }
}
