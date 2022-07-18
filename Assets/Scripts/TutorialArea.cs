using TMPro;
using UnityEngine;

public class TutorialArea : MonoBehaviour
{
    // Start is called before the first frame update
    public static TutorialArea Instance { get; private set; }
    public static GameObject ButtonArea { get; private set; }

    private bool Initialized = false;
    private TextMeshProUGUI textArea;

    [SerializeField]
    private AudioClip tutorialSound;

    public static AudioClip TutorialSound => Instance.tutorialSound;

    public static string TextArea
    {
        get => Instance.textArea.text;
        set => Instance.textArea.text = value;
    }

    public void Start()
    {
        if (!Initialized)
        {
            Initialized = true;
            Instance = this;
            textArea = GetComponentInChildren<TextMeshProUGUI>();
            ButtonArea = GameObject.FindGameObjectWithTag("ButtonArea");
        }
    }
}
