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
        if (!Initialized)
        {
            Initialized = true;
            //var Panes = FindObjectsOfType<Pane>();
            var Panes = Resources.FindObjectsOfTypeAll<Pane>();
            foreach (var pane in Panes)
            {
                //CDebug.Log("Pane = " + pane);
                PaneMap.Add(pane.name, pane);
                //pane.gameObject.SetActive(false);
            }
        }
       // CDebug.Log("Panes = " + Panes);
    }
    /*public static void Disable()
    {
        foreach (var pane in PaneMap)
        {
            pane.Value.gameObject.SetActive(false);
        }
    }*/
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

    public static async Task SwitchTo(Pane From,Pane To,float Speed = 1f,float Distance = 1f,bool MoveCamera = true)
    {
        Task fromTask = From.Move(Fade.Out, Direction.Towards, Speed, Distance, Interrupt: true);
        Task toTask = To.Move(Fade.In, Direction.Towards, Speed, Distance, Interrupt: true);
        if (MoveCamera)
        {
            Task camera = CameraTarget.MoveForward(Distance, Speed);
            await camera;
        }
        await fromTask;
        await toTask;
        
    }

    public static async Task SwitchBackTo(Pane From, Pane To, float Speed = 1f,float Distance = 1,bool MoveCamera = true)
    {
        Task fromTask = From.Move(Fade.Out, Direction.Away, Speed, Distance, Interrupt: true);
        Task toTask = To.Move(Fade.In, Direction.Away,Speed,Distance, Interrupt: true);
        if (MoveCamera)
        {
            Task camera = CameraTarget.MoveBackward(Distance, Speed);
            await camera;
        }
        await fromTask;
        await toTask;
    }

    public void Enable(Vector3? ResetPosition = null)
    {
        var group = GetComponent<CanvasGroup>();
        gameObject.SetActive(true);
        group.alpha = 1;
        group.blocksRaycasts = true;
        if (ResetPosition == null)
        {
            ResetPosition = Vector3.zero;
        }
        transform.localPosition = ResetPosition.Value;
        //transform.localPosition = Vector3.zero;
        var buttons = GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
            button.enabled = true;
        }
    }

    public async Task Move(Fade fade, Direction direction,float Speed = 1f,float Distance = 1, Func<float,float,float,float> lerpFunc = null,bool Interrupt = false)
    {
        Distance *= CameraTarget.PlaneDistance;
        if (Moving)
        {
            if (Interrupt)
            {
                Done = true;
            }
            await Tasker.Run(() => { while (Moving) { } });
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
                transform.localPosition = new Vector3(0, 0, Distance);
            }
            else
            {
                //rect.localScale = new Vector3(MaxSize, MaxSize, 1f);
                transform.localPosition = new Vector3(0,0,-Distance);
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
                    transform.localPosition = new Vector3(0,0,lerpFunc(0,-Distance,T));
                }
                else
                {
                    //rect.localScale = new Vector3(lerpFunc(1f, MinSize, T), lerpFunc(1f, MinSize, T), 1f);
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
                //rect.localScale = new Vector3(lerpFunc(MinSize, 1f, T), lerpFunc(MinSize, 1f, T), 1f);
                if (direction == Direction.Towards)
                {
                    //rect.localScale = new Vector3(lerpFunc(MinSize, 1f, T), lerpFunc(MinSize, 1f, T), 1f);
                    transform.localPosition = new Vector3(0, 0, lerpFunc(Distance, 0, T));
                }
                else
                {
                    //Debug.Log("Away");
                    //rect.localScale = new Vector3(lerpFunc(MaxSize, 1f, T), lerpFunc(MaxSize, 1f, T), 1f);
                    transform.localPosition = new Vector3(0, 0, lerpFunc(-Distance, 0, T));
                }
                if (T == 1)
                {
                    Done = true;
                }
            }
        };
        StaticUpdate.Updates += func;
        await Tasker.Run(() => { while (!Done) { } });
        StaticUpdate.Updates -= func;
        Done = false;
        Moving = false;

    }
}

