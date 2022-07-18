using System;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public static Action TutorialEvent;

    // Start is called before the first frame update
    private bool Enabled = true;
    private int ArrowType;

    private void Start()
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

    public void Enable(bool enable, bool activation = true)
    {
        Enabled = enable;
        if (activation)
            gameObject.SetActive(Enabled);
    }

    private void OnMouseDown()
    {
        if (Player.ActivePlayer != null && Enabled)
        {
            if (TutorialEvent != null)
            {
                TutorialEvent?.Invoke();
                TutorialEvent = null;
            }
            Vector2Int newPosition = new Vector2Int((int)Player.ActivePlayer.transform.position.x, (int)Player.ActivePlayer.transform.position.y);
            if (ArrowType == 1)
            {
                newPosition += new Vector2Int(0, 1);
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
