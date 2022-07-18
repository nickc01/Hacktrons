#define DebugButtons

using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonHandle : MonoBehaviour
{
    private void Start()
    {
        MethodInfo[] methods = typeof(Buttons).GetMethods();
        string underScoreName = gameObject.name.Replace(" ", "_");
        string spaceLessName = gameObject.name.Replace(" ", "");
        MethodInfo selection = methods.FirstOrDefault(md =>
        {
            return (md.Name == underScoreName || md.Name == spaceLessName) && md.IsStatic && md.ReturnType == typeof(void);
        });
        if (selection != null)
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                selection.Invoke(null, null);
            });
        }
    }
}

