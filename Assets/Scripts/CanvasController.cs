using System;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{

    // Use this for initialization
    private static CanvasGroup groupController;
    private static List<Func<float, bool>> actions = new List<Func<float, bool>>();

    private void Start()
    {
        groupController = GetComponent<CanvasGroup>();
        Pane.Initialize();
    }

    // Update is called once per frame
    private void Update()
    {
        for (int i = actions.Count - 1; i > 0; i--)
        {
            if (actions[i](Time.deltaTime) == true)
            {
                actions.Remove(actions[i]);
            }
        }
    }

    public static void MovePanes(string From, string To)
    {
        MovePanes(Pane.GetPane(From), Pane.GetPane(To));
    }
    public static void MovePanes(Pane From, Pane To)
    {
        From.gameObject.SetActive(false);
        To.gameObject.SetActive(true);
    }

}
