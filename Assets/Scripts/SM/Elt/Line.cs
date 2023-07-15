using UnityEngine;

public class Line : MonoBehaviour
{
    public RectTransform rectFirst, rectSecond;
    RectTransform rect;

    public void Set(GameObject first, GameObject second)
    {
        rectFirst = first.GetComponent<RectTransform>();
        rectSecond = second.GetComponent<RectTransform>();
        rect = GetComponent<RectTransform>();
        rect.position = new Vector3(rectFirst.position.x + 2.5f, 0);

        Debug.Log($"{rectSecond.localPosition.x}, {rectSecond.localPosition.y}");
    }

    void Update()
    {
        if (this.gameObject.activeSelf)
        {
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Distance());
            rect.eulerAngles = new Vector3(0, 0, Mathf.Atan((rectSecond.localPosition.x - rectFirst.localPosition.x) / 600));
        }
    }

   float Distance()
   {
        return Mathf.Sqrt((rectSecond.localPosition.y - rectFirst.localPosition.y) *
            (rectSecond.localPosition.y - rectFirst.localPosition.y) +
            (rectSecond.localPosition.x - rectFirst.localPosition.x) *
            (rectSecond.localPosition.x - rectFirst.localPosition.x));
   }
}
