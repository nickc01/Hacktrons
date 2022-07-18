using System;
using UnityEngine;

public class Target : MonoBehaviour
{
    public static bool TargetEnabled = true;
    public static Action TutorialTargetEvent;
    public event Action TargetSelectEvent;

    private void OnMouseDown()
    {
        if (TargetEnabled)
        {
            if (TutorialTargetEvent != null)
            {
                TutorialTargetEvent?.Invoke();
                TutorialTargetEvent = null;
            }
            TargetSelectEvent?.Invoke();
        }
    }
}
