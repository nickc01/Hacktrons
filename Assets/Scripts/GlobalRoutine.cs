using System.Collections;
using UnityEngine;

public class GlobalRoutine : MonoBehaviour
{
    private static GlobalRoutine _instance;

    public static GlobalRoutine Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("GLOBAL_ROUTINE")
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                _instance = obj.AddComponent<GlobalRoutine>();
            }
            return _instance;
        }
    }

    public static Coroutine Start(IEnumerator routine)
    {
        return Instance.StartCoroutine(routine);
    }

    public static void Stop(Coroutine routine)
    {
        Instance.StopCoroutine(routine);
    }
}

