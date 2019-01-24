using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct Arrows
{
    public Arrow Up;
    public Arrow Down;
    public Arrow Left;
    public Arrow Right;
    public Arrow[] ArrowList;
    public Player Host;
    public Arrows(GameObject up,GameObject down,GameObject left,GameObject right)
    {
        Up = up.GetComponent<Arrow>();
        Down = down.GetComponent<Arrow>();
        Left = left.GetComponent<Arrow>();
        Right = right.GetComponent<Arrow>();
        ArrowList = new Arrow[] {Up,Down,Left,Right };
        Host = null;
    }

    public void Enable(bool enabled)
    {
        Up.Enable(enabled);
        Down.Enable(enabled);
        Left.Enable(enabled);
        Right.Enable(enabled);
        if (enabled == true)
        {
            Check(new Vector2Int((int)Up.transform.position.x,(int)Up.transform.position.y - 1));
        }
    }

    public void Check(Vector2Int position)
    {
       /* if (CheckArrow(position.x,position.y + 1))
        {
            Up.SetActive(false);
        }
        else
        {
            Up.SetActive(true);
        }


        if (CheckArrow(position.x, position.y - 1))
        {
            Down.SetActive(false);
        }
        else
        {
            Down.SetActive(true);
        }


        if (CheckArrow(position.x - 1, position.y))
        {
            Left.SetActive(false);
        }
        else
        {
            Left.SetActive(true);
        }*/

        Up.gameObject.SetActive(CheckArrow(position.x, position.y + 1));
        Down.gameObject.SetActive(CheckArrow(position.x, position.y - 1));
        Left.gameObject.SetActive(CheckArrow(position.x - 1, position.y));
        Right.gameObject.SetActive(CheckArrow(position.x + 1, position.y));
        //TileManager.Print();
        /*if (CheckArrow(position.x + 1, position.y))
        {
            Right.SetActive(false);
        }
        else
        {
            Right.SetActive(true);
        }*/
    }
    private bool CheckArrow(int x, int y)
    {
        if (TileManager.WithinBounds(x,y) && TileManager.GetTile(x,y) != null)
        {
            var gameTile = TileManager.GetGameTile(x, y);
            if (gameTile == null)
            {
                Debug.Log("NULL");
                return true;
            }
            Debug.Log("NOT NULL");
        }
        return false;
    }
}
