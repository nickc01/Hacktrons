using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Dragger
{
    //private static List<GameObject> MovingObjects = new List<GameObject>();
    private static Dictionary<GameObject, bool> MovingObjects = new Dictionary<GameObject, bool>();

    public static async Task Drag(GameObject UIObject,Vector2? From,Vector2? To,float Speed = 1f,Func<float,float,float,float> lerpFunc = null,bool Interrupt = false,bool EndIfEqual = false)
    {
        if (lerpFunc == null)
        {
            lerpFunc = LerpManager.SmoothOut;
        }
        RectTransform rect = UIObject.GetComponent<RectTransform>();
        if (From == null)
        {
            From = rect.anchoredPosition;
        }
        if (To == null)
        {
            To = rect.anchoredPosition;
        }
        if (EndIfEqual && From.Value == To.Value)
        {
            return;
        }
        if (MovingObjects.ContainsKey(UIObject))
        {
            if (Interrupt)
            {
                MovingObjects[UIObject] = false;
            }
            await Tasker.Run(() => { while (MovingObjects.ContainsKey(UIObject)) { } });
        }
        MovingObjects.Add(UIObject,true);
        bool Done = false;
        float T = 0;
        Action func = () => {
            if (Done)
            {
                return;
            }
            T += Time.deltaTime * Speed;
            if (T > 1)
            {
                T = 1;
            }
            rect.anchoredPosition = new Vector2(lerpFunc(From.Value.x,To.Value.x,T), lerpFunc(From.Value.y, To.Value.y, T));
            if (T == 1)
            {
                Done = true;
            }
        };
        StaticUpdate.Updates += func;
        await Tasker.Run(() => {
            while (!Done && MovingObjects[UIObject]) { }
        });
        StaticUpdate.Updates -= func;
        MovingObjects.Remove(UIObject);
    }
}
