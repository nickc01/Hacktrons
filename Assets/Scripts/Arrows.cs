using UnityEngine;

public struct Arrows
{
    private static Arrows Instance;
    public Arrow Up;
    public Arrow Down;
    public Arrow Left;
    public Arrow Right;
    public Arrow[] ArrowList;
    public Player Host;
    public Arrows(GameObject up, GameObject down, GameObject left, GameObject right)
    {
        Up = up.GetComponent<Arrow>();
        Down = down.GetComponent<Arrow>();
        Left = left.GetComponent<Arrow>();
        Right = right.GetComponent<Arrow>();
        ArrowList = new Arrow[] { Up, Down, Left, Right };
        Host = null;
        Instance = this;
    }

    public static void EnableArrows(bool enabled, bool activation = true)
    {
        Instance.Enable(enabled, activation);
    }

    public void Enable(bool enabled, bool activation = true)
    {
        Up.Enable(enabled, activation);
        Down.Enable(enabled, activation);
        Left.Enable(enabled, activation);
        Right.Enable(enabled, activation);
        if (enabled == true)
        {
            Check(new Vector2Int((int)Up.transform.position.x, (int)Up.transform.position.y - 1));
        }
    }

    public void Check(Vector2Int position)
    {
        Up.gameObject.SetActive(CheckArrow(position.x, position.y + 1));
        Down.gameObject.SetActive(CheckArrow(position.x, position.y - 1));
        Left.gameObject.SetActive(CheckArrow(position.x - 1, position.y));
        Right.gameObject.SetActive(CheckArrow(position.x + 1, position.y));
    }
    private bool CheckArrow(int x, int y)
    {
        if (Game.WithinBounds(x, y) && Game.GetTile(x, y) != null)
        {
            Component gameTile = Game.GetGameTile(x, y);
            if (gameTile == null)
            {
                return true;
            }
        }
        return false;
    }
}
