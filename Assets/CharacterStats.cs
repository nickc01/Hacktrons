using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterStats : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static CharacterStats Instance;

    private CanvasGroup group;

    private float baseAlpha;
    private float invisibleAlpha = 0.15f;

    private float TargetAlpha;

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
        group = GetComponent<CanvasGroup>();
        baseAlpha = group.alpha;
        TargetAlpha = baseAlpha;
        Clear();
    }

    void Update()
    {
        group.alpha = Mathf.Lerp(group.alpha,TargetAlpha,5f * Time.deltaTime);
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
        //Instance.Blocker.SetActive(true);
        Instance.gameObject.SetActive(false);
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
        Instance.gameObject.SetActive(true);
    }

    public static void Refresh()
    {
        SetCharacter(MainCharacter);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TargetAlpha = invisibleAlpha;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TargetAlpha = baseAlpha;
    }
}
