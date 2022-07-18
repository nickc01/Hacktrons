using UnityEngine;

public class BackgroundSprites : MonoBehaviour
{
    public int Amount = 2000;
    public float Min = 15f;
    public float Max = 600f;
    public Color MinColor;
    public Rect Boundaries;
    public Rect MinBoundaries;
    public GameObject BackgroundSprite;

    private void Start()
    {
        RenderSettings.fogColor = GetComponent<Camera>().backgroundColor;
        for (int i = 0; i < Amount; i++)
        {
            GameObject NewSprite = GameObject.Instantiate(BackgroundSprite);
            float Depth = Random.value;
            float Z = Mathf.Lerp(Min, Max, Depth);
            Vector3 NewPosition = new Vector3();
            while (MinBoundaries.Contains(NewPosition))
            {
                NewPosition = new Vector3(Random.Range(Boundaries.xMin, Boundaries.xMax),
          Random.Range(Boundaries.yMin, Boundaries.yMax), Z);
            }
            NewSprite.transform.position = NewPosition;
        }
    }
}
