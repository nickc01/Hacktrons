using UnityEngine;

public class Selection : MonoBehaviour
{
    public static Selection Instance { get; private set; }
    public static Character SelectedCharacter;

    private void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        if (SelectedCharacter != null)
        {
            transform.position = SelectedCharacter.transform.position;
        }
    }
}
