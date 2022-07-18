using System;
using UnityEngine;
using UnityEngine.UI;

public class ToolSelector : MonoBehaviour
{
    // Start is called before the first frame update
    private Button button;
    private RawImage image;
    private Text amountText;
    public Player SetPlayer { get; set; }
    private int currentAmount = 0;
    private bool Initialized = false;
    //public static bool ActiveForTutorial = false;
    public static Action TutorialOnClick;
    public int CurrentAmount
    {
        get => currentAmount;
        set
        {
            amountText.text = value.ToString();
            currentAmount = value;
        }
    }

    public Texture Image
    {
        get => image.texture;
        set => image.texture = value;
    }

    public void Start()
    {
        if (Initialized == false)
        {
            Initialized = true;
            button = GetComponent<Button>();
            image = GetComponent<RawImage>();
            amountText = GetComponentInChildren<Text>();
            button.onClick.AddListener(OnClick);
        }
    }

    public void Undo()
    {
        CurrentAmount++;
    }

    private void OnClick()
    {
        if (!TutorialRoutine.TutorialActive || (TutorialRoutine.TutorialActive && TutorialOnClick != null))
        {
            if (TutorialOnClick != null)
            {
                TutorialOnClick?.Invoke();
                TutorialOnClick = null;
            }
            if (CurrentAmount > 0 && SpawnTile.SelectedTile != null)
            {
                CurrentAmount--;
                CharacterSelector.Selection = Game.SpawnCharacter(SetPlayer.GetType(), CharacterSelector.SpawnPosition) as Player;
                CharacterSelector.Cancel();
            }
        }
    }
}
