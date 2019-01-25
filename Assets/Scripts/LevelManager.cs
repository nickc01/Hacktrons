using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject LevelButton;

    private static LevelManager manager;

    void Start()
    {
        manager = this;
        CDebug.Log("Level Count = " + TileManager.GetLevelCount());
        for (int i = 0; i < TileManager.GetLevelCount(); i++)
        {
            var Level = i;
            CDebug.Log($"I = {i}");
            var NewButton = Instantiate(LevelButton).GetComponent<Button>();
            NewButton.GetComponentInChildren<Text>().text = $"{Level + 1}";
            NewButton.transform.SetParent(transform, false);
            NewButton.onClick.AddListener(() => {
                CDebug.Log($"Level = {Level + 1}");
                TileManager.LoadLevel(Level + 1);
                Pane.GetPane("Pre Game").gameObject.SetActive(true);
            });
        }
    }
}
