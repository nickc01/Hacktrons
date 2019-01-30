using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LerpManager : MonoBehaviour
{
    private static LerpManager instance;
    private static event Action<float> updateEvents;

    [SerializeField]
    AnimationCurve smoothLerp;
    [SerializeField]
    AnimationCurve smoothIn;
    [SerializeField]
    AnimationCurve smoothOut;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        updateEvents?.Invoke(Time.deltaTime);
    }

    public static float SmoothLerp(float A, float B, float T)
    {
        //Debug.Log("Smooth Lerp");
        return ((B - A) * instance.smoothLerp.Evaluate(T)) + A;
    }

    public static float SmoothInLerp(float A, float B, float T)
    {
        return ((B - A) * instance.smoothIn.Evaluate(T)) + A;
    }

    public static float SmoothOut(float A, float B, float T)
    {
        return ((B - A) * instance.smoothOut.Evaluate(T)) + A;
    }

    public static float LinearLerp(float A, float B, float T)
    {
        return ((B - A) * T) + A;
    }

    public static float Lerp(float A, float B, float T, Func<float,float,float,float> lerpFunc)
    {
        return lerpFunc(A, B, T);
    }

    public static async Task DoUpdate(float Duration, Action<float> func)
    {
        var Done = false;
        float T = 0;
        Action<float> preFunc = dt =>
        {
            if (Done)
            {
                return;
            }
            T += dt;
            if (T > Duration)
            {
                T = Duration;
            }
            func(T / Duration);
            if (T == Duration)
            {
                Done = true;
            }
        };
        updateEvents += preFunc;
        await Task.Run(() => {
            while (!Done) { }
        });
        updateEvents -= preFunc;
    }
}
