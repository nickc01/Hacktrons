using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CanvasController : MonoBehaviour {

    // Use this for initialization
    private static CanvasGroup groupController;
	void Start ()
    {
        groupController = GetComponent<CanvasGroup>();
	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    public static async Task Move(GameObject AwayFrom,GameObject Towards)
    {
        await Task.Run(() => {
            CanvasGroup awayGroup = AwayFrom.GetComponent<CanvasGroup>();
            CanvasGroup towardsGroup = Towards.GetComponent<CanvasGroup>();
            towardsGroup.alpha = 0;
            Towards.SetActive(true);
            float TowardsZ = 300;
            float AwayZ = 0;
            Towards.transform.localPosition = new Vector3(0, 0, 300);
        });
    }
}
