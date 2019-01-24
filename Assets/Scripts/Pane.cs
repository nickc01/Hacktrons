using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Pane : MonoBehaviour
{
    private static bool Initialized = false;
    private static Dictionary<string, Pane> PaneMap = new Dictionary<string, Pane>();
    public static void Initialize()
    {
        //var Panes = FindObjectsOfType<Pane>();
        var Panes = Resources.FindObjectsOfTypeAll<Pane>();
        foreach (var pane in Panes)
        {
            Debug.Log("Pane = " + pane);
            PaneMap.Add(pane.name, pane);
        }
       // Debug.Log("Panes = " + Panes);
    }
    public static Pane GetPane(string Name)
    {
        return PaneMap[Name];
    }
}

