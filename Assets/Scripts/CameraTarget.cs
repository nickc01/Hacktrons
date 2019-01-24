using System.Collections;
using System.Collections.Generic;
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
        //Debug.Log("Start Mover");
    }

    void Update()
    {
        if (Active)
        {
            //Debug.Log($"Camera = {mainCamera}");
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

    public static bool Active { get; set; } = false;
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
}
