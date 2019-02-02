using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    public static CharacterStats Instance;

    public TextMeshProUGUI MovesLeft;
    public TextMeshProUGUI AttackRange;
    public TextMeshProUGUI AttackDamage;
    public TextMeshProUGUI TrailLength;
    public TextMeshProUGUI MaxMoves;
    public TextMeshProUGUI CharacterName;
    public TextMeshProUGUI Info;
    public RawImage CharacterTexture;
    public GameObject Blocker;

    public static Character MainCharacter;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        Clear();
    }

    public static void Clear()
    {
        MainCharacter = null;
        Instance.MovesLeft.text = "";
        Instance.AttackDamage.text = "";
        Instance.AttackRange.text = "";
        Instance.TrailLength.text = "";
        Instance.CharacterName.text = "";
        Instance.Info.text = "";
        Instance.MaxMoves.text = "";
        Instance.CharacterTexture.texture = null;
        Instance.Blocker.SetActive(true);
    }

    public static void SetCharacter(Character character)
    {
        MainCharacter = character;
        if (MainCharacter == null)
        {
            Clear();
            return;
        }
        Instance.MovesLeft.text = (character.MovesMax - character.MovesDone).ToString();
        Instance.AttackDamage.text = character.AttackDamage.ToString();
        Instance.AttackRange.text = character.AttackRange.ToString();
        Instance.TrailLength.text = character.MaxTrailLength.ToString();
        Instance.CharacterName.text = "program." + character.Name + ".exe";
        Instance.Info.text = character.Info;
        Instance.MaxMoves.text = character.MovesMax.ToString();
        Instance.CharacterTexture.texture = character.GetComponent<SpriteRenderer>().sprite.texture;
        Instance.Blocker.SetActive(false);
    }

    public static void Refresh()
    {
        SetCharacter(MainCharacter);
    }
}
