using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TurnDisplay : MonoBehaviour
{
    public static TurnDisplay Instance { get; private set; }
    public TextMeshProUGUI TurnText;
    new private RawImage renderer;

    public static Color Color
    {
        get => Instance.renderer.color;
        set => Instance.renderer.color = value;
    }

    public void Start()
    {
        Instance = this;
        renderer = GetComponent<RawImage>();
    }

    public static void SetText(string text)
    {
        Instance.TurnText.text = text;
    }

    
}
