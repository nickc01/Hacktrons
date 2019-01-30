using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    private static CameraTarget maintarget;
    private static Camera[] mainCameras;
    [SerializeField]
    private float CameraSpeed = 0.7f;
    [SerializeField]
    private float MovementSpeed = 0.7f;
    // Start is called before the first frame update
    void Start()
    {
        maintarget = this;
        mainCameras = FindObjectsOfType<Camera>();
    }

    void Update()
    {
        if (Active)
        {
            //CDebug.Log($"Camera = {mainCamera}");
            foreach (var mainCamera in mainCameras)
            {
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, Position, Time.deltaTime * maintarget.CameraSpeed);
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
                }
            }
        }
    }

    public static bool Active { get; set; } = true;
    public static bool Movable { get; set; } = false;
    public static Vector3 Position
    {
        get => maintarget.transform.position;
        set => maintarget.transform.position = value;
    }
    //Warps the camera to a specified position
    public static void WarpCamera(Vector3 position)
    {
        Position = new Vector3(position.x,position.y,-10);
        foreach (var mainCamera in mainCameras)
        {
            mainCamera.transform.position = new Vector3(position.x, position.y, -10);
        }
    }

    public static async Task MoveForward(float? Distance = null, float Speed = 1f,bool Instant = true)
    {
        if (Distance == null)
        {
            Distance = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>().planeDistance;
        }
        await Move(Position, Position + Vector3.forward * Distance.Value,Instant: Instant);
    }

    public static async Task MoveBackward(float? Distance = null, float Speed = 1f,bool Instant = true)
    {
        if (Distance == null)
        {
            Distance = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>().planeDistance;
        }
        await Move(Position, Position + Vector3.back * Distance.Value,Instant: Instant);
    }


    public static async Task Move(Vector3 From, Vector3 To,float Speed = 1f,bool Instant = true, Func<float,float,float,float> lerpFunc = null)
    {
        if (lerpFunc == null)
        {
            lerpFunc = LerpManager.SmoothLerp;
        }

        bool Done = false;
        float T = 0;
        float CamSpeed = maintarget.CameraSpeed;
        if (Instant)
        {
            maintarget.CameraSpeed = 99999f;
        }

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
            Position = new Vector3(lerpFunc(From.x,To.x,T),lerpFunc(From.y,To.y,T),lerpFunc(From.z,To.z,T));
            if (T == 1)
            {
                Done = true;
            }
        };
        StaticUpdate.Updates += func;
        await Task.Run(() => { while (!Done) { } });
        StaticUpdate.Updates -= func;
        if (Instant)
        {
            maintarget.CameraSpeed = CamSpeed;
        }
        Done = false;
    }
}
