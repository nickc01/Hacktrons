﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Start is called before the first frame update
    bool Enabled = true;
    int ArrowType;
    void Start()
    {
        if (gameObject.name == "Up Arrow")
        {
            ArrowType = 1;
        }
        if (gameObject.name == "Down Arrow")
        {
            ArrowType = 2;
        }
        if (gameObject.name == "Left Arrow")
        {
            ArrowType = 3;
        }
        if (gameObject.name == "Right Arrow")
        {
            ArrowType = 4;
        }
    }

    public void Enable(bool enable)
    {
        Enabled = enable;
        gameObject.SetActive(Enabled);
    }
    
    void OnMouseDown()
    {
        Debug.Log("Active Player = " + Player.ActivePlayer);
        Debug.Log("Enabled = " + Enabled);
        if (Player.ActivePlayer != null && Enabled)
        {
            Vector2Int newPosition = new Vector2Int((int)Player.ActivePlayer.transform.position.x, (int)Player.ActivePlayer.transform.position.y);
            if (ArrowType == 1)
            {
                newPosition += new Vector2Int(0,1);
            }
            if (ArrowType == 2)
            {
                newPosition += new Vector2Int(0, -1);
            }
            if (ArrowType == 3)
            {
                newPosition += new Vector2Int(-1, 0);
            }
            if (ArrowType == 4)
            {
                newPosition += new Vector2Int(1, 0);
            }
            Player.ActivePlayer.ArrowMove(newPosition);
        }
    }
}
