using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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


    void Start()
    {
        Instance = this;
    }

}
