using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

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
            CDebug.Log("Pane = " + pane);
            PaneMap.Add(pane.name, pane);
        }
       // CDebug.Log("Panes = " + Panes);
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

    public static async Task SwitchTo(Pane From,Pane To)
    {
        Task fromTask = From.Move(Fade.Out, Direction.Towards,Interrupt: true);
        Task toTask = To.Move(Fade.In, Direction.Towards, Interrupt: true);
        Task camera = CameraTarget.MoveForward();
        await fromTask;
        await toTask;
        await camera;
    }

    public static async Task SwitchBackTo(Pane From, Pane To)
    {
        Task fromTask = From.Move(Fade.Out, Direction.Away, Interrupt: true);
        Task toTask = To.Move(Fade.In, Direction.Away, Interrupt: true);
        Task camera = CameraTarget.MoveBackward();
        await fromTask;
        await toTask;
        await camera;
    }

    public async Task Move(Fade fade, Direction direction,float Speed = 1f,float? Distance = null, Func<float,float,float,float> lerpFunc = null,bool Interrupt = false)
    {
        if (Distance == null)
        {
            Distance = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>().planeDistance;
        }
        if (Moving)
        {
            if (Interrupt)
            {
                Done = true;
            }
            await Task.Run(() => { while (Moving) { } });
            Done = false;
        }
        if (lerpFunc == null)
        {
            lerpFunc = LerpManager.SmoothLerp;
        }
        Moving = true;
        var buttons = GetComponentsInChildren<Button>();
        var canvas = GetComponentInParent<Canvas>();
        var ScreenFactor = canvas.worldCamera.ScreenToWorldPoint(Vector3.one) - canvas.worldCamera.ScreenToWorldPoint(Vector3.zero);
        //Debug.Log("Screen Factor = " + ScreenFactor);
        //Distance = ScreenFactor.z * Distance;
        //Distance = canvas.worldCamera.ScreenToWorldPoint(new Vector3(0, 0, -Distance)).z;
        Distance /= canvas.GetComponent<RectTransform>().localScale.z;
        //float MaxSize = 4f;
        //float MinSize = 0.25f;
        var group = GetComponent<CanvasGroup>();
        var rect = GetComponent<RectTransform>();
        if (fade == Fade.Out)
        {
            group.alpha = 1;
            group.blocksRaycasts = false;
            //rect.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            foreach (var button in buttons)
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
                //rect.localScale = new Vector3(MinSize, MinSize, 1f);
                //transform.localPosition = Vector3.zero;
                transform.localPosition = new Vector3(0, 0, Distance.Value);
            }
            else
            {
                //rect.localScale = new Vector3(MaxSize, MaxSize, 1f);
                transform.localPosition = new Vector3(0,0,-Distance.Value);
            }
            foreach (var button in buttons)
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
                group.alpha = lerpFunc(1f,0f,T);
                if (direction == Direction.Towards)
                {
                    //rect.localScale = new Vector3(lerpFunc(1f, MaxSize, T), lerpFunc(1f, MaxSize, T), 1f);
                    transform.localPosition = new Vector3(0,0,lerpFunc(0,-Distance.Value,T));
                }
                else
                {
                    //rect.localScale = new Vector3(lerpFunc(1f, MinSize, T), lerpFunc(1f, MinSize, T), 1f);
                    transform.localPosition = new Vector3(0, 0, lerpFunc(0, Distance.Value, T));
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
                //rect.localScale = new Vector3(lerpFunc(MinSize, 1f, T), lerpFunc(MinSize, 1f, T), 1f);
                if (direction == Direction.Towards)
                {
                    //rect.localScale = new Vector3(lerpFunc(MinSize, 1f, T), lerpFunc(MinSize, 1f, T), 1f);
                    transform.localPosition = new Vector3(0, 0, lerpFunc(Distance.Value, 0, T));
                }
                else
                {
                    //Debug.Log("Away");
                    //rect.localScale = new Vector3(lerpFunc(MaxSize, 1f, T), lerpFunc(MaxSize, 1f, T), 1f);
                    transform.localPosition = new Vector3(0, 0, lerpFunc(-Distance.Value, 0, T));
                }
                if (T == 1)
                {
                    Done = true;
                }
            }
        };
        StaticUpdate.Updates += func;
        await Task.Run(() => { while (!Done) { } });
        StaticUpdate.Updates -= func;
        Done = false;
        Moving = false;

    }
}

