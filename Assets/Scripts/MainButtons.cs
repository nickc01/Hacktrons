using UnityEngine;

public class MainButtons : MonoBehaviour
{
    private static MainButtons Instance;
    // Start is called before the first frame update
    public GameObject finishTurn;
    public GameObject attack;
    public GameObject cancelAttack;

    public static GameObject FinishTurn => Instance.finishTurn;
    public static GameObject Attack => Instance.attack;
    public static GameObject CancelAttack => Instance.cancelAttack;

    private void Start()
    {
        Instance = this;
    }

}
