using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject LevelButton;

    private static LevelManager manager;

    private void Start()
    {
        manager = this;
        for (int i = 0; i < Game.GetLevelCount(); i++)
        {
            int Level = i;
            Button NewButton = Instantiate(LevelButton).GetComponent<Button>();
            NewButton.GetComponentInChildren<Text>().text = $"{Level + 1}";
            if (Level > 0)
            {
                NewButton.gameObject.SetActive(false);
            }
            NewButton.transform.SetParent(transform, false);
            NewButton.onClick.AddListener(() =>
            {
                GlobalRoutine.Start(Game.LoadLevel(Level + 1));
            });
        }
    }

    public static bool UnlockLevel(int LevelNumber)
    {
        if (LevelNumber > manager.transform.childCount)
        {
            return false;
        }
        Transform Child = manager.transform.GetChild(LevelNumber - 1);
        if (Child == null)
        {
            return false;
        }
        else
        {
            Child.gameObject.SetActive(true);
            return true;
        }
    }
}
