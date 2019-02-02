using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialArea : MonoBehaviour
{
    // Start is called before the first frame update
    public static TutorialArea Instance { get; private set; }
    public static GameObject ButtonArea { get; private set; }
    bool Initialized = false;

    TextMeshProUGUI textArea;

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
