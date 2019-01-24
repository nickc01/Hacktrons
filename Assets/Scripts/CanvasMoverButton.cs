using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CanvasMoverButton : MonoBehaviour
{
    [SerializeField]
    GameObject From;
    [SerializeField]
    GameObject To;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    private void OnClick()
    {
        Debug.Log("CLICKED");
       // CanvasController.Move(From, To);
    }
    void OnDestroy()
    {
        GetComponent<Button>().onClick.RemoveListener(OnClick);
    }
}
