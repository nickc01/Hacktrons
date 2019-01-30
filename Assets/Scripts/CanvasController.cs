using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CanvasController : MonoBehaviour {

    // Use this for initialization
    private static CanvasGroup groupController;
    private static List<Func<float, bool>> actions = new List<Func<float, bool>>();
	void Start ()
    {
        groupController = GetComponent<CanvasGroup>();
        Pane.Initialize();
	}
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = actions.Count - 1; i > 0; i--)
        {
            if (actions[i](Time.deltaTime) == true)
            {
                actions.Remove(actions[i]);
            }
           // actions[i](Time.deltaTime);
        }
	}

    public static async Task MovePanes(string From, string To)
    {
        await MovePanes(Pane.GetPane(From),Pane.GetPane(To));
    }
    public static async Task MovePanes(Pane From, Pane To)
    {
        //var FromObject = GameObject.Find(From);
        //var ToObject = GameObject.Find(To);
        /*if (FromObject == null)
        {
            CDebug.Log("Cant find from object of : " + From);
        }
        if (ToObject == null)
        {
            CDebug.Log("Cant find to object of : " + To);
        }*/
        //Temporary
        From.gameObject.SetActive(false);
        To.gameObject.SetActive(true);
        /*try
        {
            Task fadein = From.Move(true);
            Task fadeOut = To.Move(false);
            await fadein;
            await fadeOut;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }*/
    }

}
