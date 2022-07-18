using System;
using UnityEngine;

public class LerpManager : MonoBehaviour
{
    private static LerpManager instance;
    private static event Action<float> updateEvents;

    [SerializeField]
    private AnimationCurve smoothLerp;
    [SerializeField]
    private AnimationCurve smoothIn;
    [SerializeField]
    private AnimationCurve smoothOut;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        updateEvents?.Invoke(Time.deltaTime);
    }

    public static float SmoothLerp(float A, float B, float T)
    {
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

    public static float Lerp(float A, float B, float T, Func<float, float, float, float> lerpFunc)
    {
        return lerpFunc(A, B, T);
    }
}
