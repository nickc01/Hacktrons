#define DebugButtons

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonHandle : MonoBehaviour
{
    Action<GameObject> gameObjectFunc;
    Action voidFunc;
    void Start()
    {
        var methods = typeof(Game.Buttons).GetMethods();
        /* foreach (var md in methods)
         {
             if ((md.Name == gameObject.name + " Button" || md.Name == gameObject.name + "Button" || md.Name == gameObject.name) && md.IsStatic && md.ReturnType == typeof(void))
             {

             }
         }*/
        //methods[0].Name.Replace(' ', '_');
        var underScoreName = gameObject.name.Replace(" ", "_");
        var spaceLessName = gameObject.name.Replace(" ", "");
        //var selectionOLD = methods.FirstOrDefault(md => (md.Name == gameObject.name + "_Button" || md.Name == gameObject.name + "Button" || md.Name == gameObject.name) && md.IsStatic && md.ReturnType == typeof(void));
        var selection = methods.FirstOrDefault(md => {
            return (md.Name == underScoreName || md.Name == spaceLessName) && md.IsStatic && md.ReturnType == typeof(void);
        });
        if (selection != null)
        {
            GetComponent<Button>().onClick.AddListener(() => {
                selection.Invoke(null, null);
            });
        }
#if DebugButtons
        else
        {
            //CDebug.LogError("The Selection is null for " + gameObject);
        }
#endif

    }
}

