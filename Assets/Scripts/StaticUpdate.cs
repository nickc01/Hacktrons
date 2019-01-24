using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticUpdate : MonoBehaviour
{
    public static event Action Updates;
    private void Update() => Updates?.Invoke();
}
