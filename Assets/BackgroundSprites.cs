using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSprites : MonoBehaviour
{
    public int Amount = 1000;
    public float Min = 15f;
    public float Max = 200f;
    public Color MinColor;
    public Color MaxColor;
    public Rect Boundaries;
    public GameObject BackgroundSprite;
    void Start()
    {
        for (int i = 0; i < Amount; i++)
        {
            var NewSprite = GameObject.Instantiate(BackgroundSprite);
            var Renderer = NewSprite.GetComponent<SpriteRenderer>();
            var Depth = Random.value;
            var Z = Mathf.Lerp(Min,Max,Depth);
            var NewPosition = new Vector3(Random.Range(Boundaries.xMin,Boundaries.xMax),Random.Range(Boundaries.yMin,Boundaries.yMax),Z);
            NewSprite.transform.position = NewPosition;
            Renderer.color = new Color(Mathf.Lerp(MinColor.r,MaxColor.r,Depth), Mathf.Lerp(MinColor.g, MaxColor.g, Depth), Mathf.Lerp(MinColor.b, MaxColor.b, Depth), Mathf.Lerp(MinColor.a, MaxColor.a, Depth));
;        }
    }
}
