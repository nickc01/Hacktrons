using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnDisplay : MonoBehaviour
{
    public static TurnDisplay Instance { get; private set; }
    public TextMeshProUGUI TurnText;
    private new RawImage renderer;

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
