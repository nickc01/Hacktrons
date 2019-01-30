using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSprites : MonoBehaviour
{
    public int Amount = 2000;
    public float Min = 15f;
    public float Max = 600f;
    public Color MinColor;
    //public Color MaxColor;
    public Rect Boundaries;
    public Rect MinBoundaries;
    public GameObject BackgroundSprite;
    void Start()
    {
        RenderSettings.fogColor = GetComponent<Camera>().backgroundColor;
        //RenderSettings.fogEndDistance = Max;
        //RenderSettings.fogStartDistance = Min;
        for (int i = 0; i < Amount; i++)
        {
            var NewSprite = GameObject.Instantiate(BackgroundSprite);
            var Depth = Random.value;
            var Z = Mathf.Lerp(Min, Max, Depth);
            //var NewPosition = new Vector3();
            /*float X = 0f;
            float Y = 0f;
            //var Renderer = NewSprite.GetComponent<SpriteRenderer>();
            var Depth = Random.value;
            var Z = Mathf.Lerp(Min,Max,Depth);
            var LowerBoundX = Random.Range(0, 1);
            var LowerBoundY = Random.Range(0, 1);
            //var HigherBoundX = Random.Range(0, 1);
            //var HigherBoundY = Random.Range(0, 1);
            if (LowerBoundX == 0)
            {
                X = Random.Range(Boundaries.xMin,MinBoundaries.xMin);
            }
            else
            {
                X = Random.Range(MinBoundaries.xMax, Boundaries.xMax);
            }

            if (LowerBoundY == 0)
            {
                Y = Random.Range(Boundaries.yMin, MinBoundaries.yMin);
            }
            else
            {
                Y = Random.Range(MinBoundaries.yMax, Boundaries.yMax);
            }
            var NewPosition = new Vector3(Random.Range(Boundaries.xMin,Boundaries.xMax),
              Random.Range(Boundaries.yMin, Boundaries.yMax), Z);
            var NewPosition = new Vector3(X,Y,Z);*/
            /*var NewPosition = new Vector3(Random.Range(2 * MinBoundaries.xMin - Boundaries.xMin, 2 * MinBoundaries.xMax - Boundaries.xMax),
                Random.Range(2 * MinBoundaries.yMin - Boundaries.yMin, 2 * MinBoundaries.yMax - Boundaries.yMax),Z);*/
            var NewPosition = new Vector3();
            while (MinBoundaries.Contains(NewPosition))
            {
                NewPosition = new Vector3(Random.Range(Boundaries.xMin, Boundaries.xMax),
          Random.Range(Boundaries.yMin, Boundaries.yMax), Z);
            }
            NewSprite.transform.position = NewPosition;
            //Renderer.color = new Color(Mathf.Lerp(MinColor.r,MaxColor.r,Depth), Mathf.Lerp(MinColor.g, MaxColor.g, Depth), Mathf.Lerp(MinColor.b, MaxColor.b, Depth), Mathf.Lerp(MinColor.a, MaxColor.a, Depth));
;        }
    }
}
