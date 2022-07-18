using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pane : MonoBehaviour
{
    private static bool Initialized = false;
    private static Dictionary<string, Pane> PaneMap = new Dictionary<string, Pane>();
    public static void Initialize()
    {
        if (!Initialized)
        {
            Initialized = true;
            Pane[] Panes = Resources.FindObjectsOfTypeAll<Pane>();
            foreach (Pane pane in Panes)
            {
                PaneMap.Add(pane.name, pane);
            }
        }
    }

    public static Pane GetPane(string Name)
    {
        return PaneMap[Name];
    }

    public enum Fade
    {
        In,
        Out
    }

    public enum Direction
    {
        Towards,
        Away
    }

    public static implicit operator Pane(string paneName)
    {
        return GetPane(paneName);
    }

    private bool Moving = false;
    private bool Done = false;

    public static IEnumerator SwitchTo(Pane From, Pane To, float Speed = 1f, float Distance = 1f, bool MoveCamera = true)
    {
        Coroutine from = GlobalRoutine.Start(From.Move(Fade.Out, Direction.Towards, Speed, Distance, Interrupt: true));
        Coroutine to = GlobalRoutine.Start(To.Move(Fade.In, Direction.Towards, Speed, Distance, Interrupt: true));
        if (MoveCamera)
        {
            yield return CameraTarget.MoveForward(Distance, Speed);
        }
        yield return from;
        yield return to;
    }

    public static IEnumerator SwitchBackTo(Pane From, Pane To, float Speed = 1f, float Distance = 1, bool MoveCamera = true)
    {
        Coroutine from = GlobalRoutine.Start(From.Move(Fade.Out, Direction.Away, Speed, Distance, Interrupt: true));
        Coroutine to = GlobalRoutine.Start(To.Move(Fade.In, Direction.Away, Speed, Distance, Interrupt: true));
        if (MoveCamera)
        {
            yield return CameraTarget.MoveBackward(Distance, Speed);
        }

        yield return from;
        yield return to;
    }

    public void Enable(Vector3? ResetPosition = null)
    {
        CanvasGroup group = GetComponent<CanvasGroup>();
        gameObject.SetActive(true);
        group.alpha = 1;
        group.blocksRaycasts = true;
        if (ResetPosition == null)
        {
            ResetPosition = Vector3.zero;
        }
        transform.localPosition = ResetPosition.Value;
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.enabled = true;
        }
    }

    public IEnumerator Move(Fade fade, Direction direction, float Speed = 1f, float Distance = 1, Func<float, float, float, float> lerpFunc = null, bool Interrupt = false)
    {
        Distance *= CameraTarget.PlaneDistance;
        if (Moving)
        {
            if (Interrupt)
            {
                Done = true;
            }
            yield return new WaitUntil(() => !Moving);
            Done = false;
        }
        if (lerpFunc == null)
        {
            lerpFunc = LerpManager.SmoothLerp;
        }
        Moving = true;
        Button[] buttons = GetComponentsInChildren<Button>();
        Canvas canvas = GetComponentInParent<Canvas>();
        Vector3 ScreenFactor = canvas.worldCamera.ScreenToWorldPoint(Vector3.one) - canvas.worldCamera.ScreenToWorldPoint(Vector3.zero);
        Distance /= canvas.GetComponent<RectTransform>().localScale.z;
        CanvasGroup group = GetComponent<CanvasGroup>();
        RectTransform rect = GetComponent<RectTransform>();
        if (fade == Fade.Out)
        {
            group.alpha = 1;
            group.blocksRaycasts = false;
            transform.localPosition = Vector3.zero;
            foreach (Button button in buttons)
            {
                button.enabled = false;
            }
        }
        else
        {
            gameObject.SetActive(true);
            group.alpha = 0;
            group.blocksRaycasts = true;
            if (direction == Direction.Towards)
            {
                transform.localPosition = new Vector3(0, 0, Distance);
            }
            else
            {
                transform.localPosition = new Vector3(0, 0, -Distance);
            }
            foreach (Button button in buttons)
            {
                button.enabled = true;
            }
        }
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
            if (fade == Fade.Out)
            {
                group.alpha = lerpFunc(1f, 0f, T);
                if (direction == Direction.Towards)
                {
                    transform.localPosition = new Vector3(0, 0, lerpFunc(0, -Distance, T));
                }
                else
                {
                    transform.localPosition = new Vector3(0, 0, lerpFunc(0, Distance, T));
                }
                if (T == 1)
                {
                    Done = true;
                    gameObject.SetActive(false);
                }
            }
            else
            {
                group.alpha = lerpFunc(0f, 1f, T);
                if (direction == Direction.Towards)
                {
                    transform.localPosition = new Vector3(0, 0, lerpFunc(Distance, 0, T));
                }
                else
                {
                    transform.localPosition = new Vector3(0, 0, lerpFunc(-Distance, 0, T));
                }
                if (T == 1)
                {
                    Done = true;
                }
            }
        };
        StaticUpdate.Updates += func;
        yield return new WaitUntil(() => Done);
        StaticUpdate.Updates -= func;
        Done = false;
        Moving = false;
    }
}

