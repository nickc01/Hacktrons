using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selection : MonoBehaviour
{
    public static Selection Instance { get; private set; }
    public static Character SelectedCharacter;

    void Start()
    {
        Instance = this;
    }

    void Update()
    {
        if (SelectedCharacter != null)
        {
            transform.position = SelectedCharacter.transform.position;
        }
    }
}
