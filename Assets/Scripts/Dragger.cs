using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Dragger
{
    private static Dictionary<GameObject, bool> MovingObjects = new Dictionary<GameObject, bool>();

    public static IEnumerator Drag(GameObject UIObject, Vector2? From, Vector2? To, float Speed = 1f, Func<float, float, float, float> lerpFunc = null, bool Interrupt = false, bool EndIfEqual = false)
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
            yield break;
        }
        if (MovingObjects.ContainsKey(UIObject))
        {
            if (Interrupt)
            {
                MovingObjects[UIObject] = false;
            }
            yield return new WaitUntil(() => !MovingObjects.ContainsKey(UIObject));
        }
        MovingObjects.Add(UIObject, true);
        bool Done = false;
        float T = 0;
        Action func = () =>
        {
            if (Done)
            {
                return;
            }
            T += Time.deltaTime * Speed;
            if (T > 1)
            {
                T = 1;
            }
            rect.anchoredPosition = new Vector2(lerpFunc(From.Value.x, To.Value.x, T), lerpFunc(From.Value.y, To.Value.y, T));
            if (T == 1)
            {
                Done = true;
            }
        };
        StaticUpdate.Updates += func;
        yield return new WaitUntil(() => !(!Done && MovingObjects[UIObject]));
        StaticUpdate.Updates -= func;
        MovingObjects.Remove(UIObject);
    }
}
