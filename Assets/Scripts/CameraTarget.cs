using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    private static CameraTarget instance;
    private static Camera[] mainCameras;
    [SerializeField]
    private float CameraSpeed = 0.7f;
    [SerializeField]
    private float MovementSpeed = 0.7f;

    public static bool Zooming { get; set; } = false;
    public static bool Active { get; set; } = true;
    public static bool Movable { get; set; } = false;
    public static Vector3 Position
    {
        get => instance.transform.position;
        set => instance.transform.position = value;
    }
    public static float MinZ { get; set; } = 0;
    public static float MaxZ { get; set; } = 0;
    public static float ZoomSpeed { get; set; } = 3f;
    public static Rect? Boundaries { get; set; } = null;
    public static Vector3 BasePosition { get; private set; } = Vector3.zero;
    private static Canvas primaryCanvas = null;

    private static bool Moving = false;
    private static bool Done = true;

    public static Canvas PrimaryCanvas
    {
        get
        {
            if (primaryCanvas == null)
            {
                primaryCanvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
            }
            return primaryCanvas;
        }
    }
    public static float PlaneDistance => PrimaryCanvas.planeDistance;
    public static Vector2 PlaneDimensions => new Vector2(PrimaryCanvas.pixelRect.width, PrimaryCanvas.pixelRect.height);
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        mainCameras = FindObjectsOfType<Camera>();
        BasePosition = mainCameras[0].transform.position;
    }

    void Update()
    {
        if (Active)
        {
            //CDebug.Log($"Camera = {mainCamera}");
            foreach (var mainCamera in mainCameras)
            {
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, Position, Time.deltaTime * instance.CameraSpeed);
                if (Movable)
                {
                    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                    {
                        Position = new Vector3(Position.x - (MovementSpeed * Time.deltaTime), Position.y, Position.z);
                    }
                    if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                    {
                        Position = new Vector3(Position.x + (MovementSpeed * Time.deltaTime), Position.y, Position.z);
                    }
                    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                    {
                        Position = new Vector3(Position.x, Position.y + (MovementSpeed * Time.deltaTime), Position.z);
                    }
                    if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                    {
                        Position = new Vector3(Position.x, Position.y - (MovementSpeed * Time.deltaTime), Position.z);
                    }
                    instance.Check();
                }
                if (Zooming)
                {
                    //Debug.Log("Zooming");
                    //if (Input.GetKeyDown(KeyCode.Mouse2))
                    //{
                    var Z = Position.z + Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
                    if (Input.GetKeyDown(KeyCode.PageDown))
                    {
                        Z += ZoomSpeed;
                    }
                    if (Input.GetKeyDown(KeyCode.PageUp))
                    {
                        Z -= ZoomSpeed;
                    }
                    if (Z > MaxZ)
                    {
                        Z = MaxZ;
                    }
                    if (Z < MinZ)
                    {
                        Z = MinZ;
                    }
                    Position = new Vector3(Position.x, Position.y, Z);

                    
                    //}
                }
            }
        }
    }

    public static void SetZoom(float value)
    {
        if (value > MaxZ)
        {
            value = MaxZ;
        }
        if (value < MinZ)
        {
            value = MinZ;
        }
        Position = new Vector3(Position.x, Position.y, value);
    }

    public static float GetZoom()
    {
        return Position.z;
    }

    private void Check()
    {
        if (Boundaries != null && !Boundaries.Value.Contains(transform.position))
        {
            var rect = Boundaries.Value;
            //Debug.Log("Rect = " + rect);
            var pos = transform.position;
            if (pos.x > rect.xMax)
            {
                pos = new Vector3(rect.xMax, pos.y, pos.z);
            }
            if (pos.y > rect.yMax)
            {
                pos = new Vector3(pos.x, rect.yMax, pos.z);
            }
            if (pos.x < rect.xMin)
            {
                pos = new Vector3(rect.xMin, pos.y, pos.z);
            }
            if (pos.y < rect.yMin)
            {
                pos = new Vector3(pos.x, rect.yMin, pos.z);
            }
            transform.position = pos;
        }
    }

    //Warps the camera to a specified position
    public static void WarpCamera(Vector3 position)
    {
        Position = new Vector3(position.x,position.y,-10);
        instance.Check();
        foreach (var mainCamera in mainCameras)
        {
            mainCamera.transform.position = new Vector3(position.x, position.y, -10);
        }
    }

    public static async Task MoveForward(float Distance = 1f, float Speed = 1f,bool Instant = true)
    {
        /*if (Distance == null)
        {
            Distance = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>().planeDistance;
        }*/
        Distance *= PlaneDistance;
        await Move(Position, Position + Vector3.forward * Distance,Speed,Instant);
    }

    public static async Task MoveBackward(float Distance = 1f, float Speed = 1f,bool Instant = true)
    {
        Distance *= PlaneDistance;
        await Move(Position, Position + Vector3.back * Distance,Speed,Instant);
    }

    public static async Task Move(Vector3 From, Vector3 To,float Speed = 1f,bool Instant = true, Func<float,float,float,float> lerpFunc = null)
    {
        try
        {
           // Debug.Log("ATTEMPTING MOVE");
            if (lerpFunc == null)
            {
                lerpFunc = LerpManager.SmoothLerp;
            }
            if (Moving)
            {
                Moving = false;
                await Tasker.Run(() => { while (!Done) { } });
            }
            //Debug.Log("Moving");
            //bool Done = false;
            Done = false;
            bool Finished = false;
            Moving = true;
            float T = 0;
            float CamSpeed = instance.CameraSpeed;
            if (Instant)
            {
                instance.CameraSpeed = 99999f;
            }

            Action func = () =>
            {
                if (Finished)
                {
                    return;
                }
                T += Time.deltaTime * Speed;
                if (T > 1)
                {
                    T = 1;
                }
                Position = new Vector3(lerpFunc(From.x, To.x, T), lerpFunc(From.y, To.y, T), lerpFunc(From.z, To.z, T));
                if (T == 1)
                {
                    Finished = true;
                }
            };
            StaticUpdate.Updates += func;
            await Tasker.Run(() => { while (!Finished && Moving) { } });
            StaticUpdate.Updates -= func;
            if (Instant)
            {
                instance.CameraSpeed = CamSpeed;
            }
            Debug.Log("Move Done");
            Moving = false;
            Done = true;
            //Done = false;
            //Moving = false;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
