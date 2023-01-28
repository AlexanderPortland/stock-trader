using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Window_Graph : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;

    private void Awake(){
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();

        CreateCircle(new Vector2(1, 1));
        List<int> valueList = new List<int> () {19, 6, 1, 3, 5, 1, 6, 2, 4, 7, 9};
    }

    private void CreateCircle(Vector2 anchoredPosition){
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(0.25f, 0.25f);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
    }

    private void ShowGraph(List<int> values){
        float graphHeight = graphContainer.sizeDelta.y;
        float xSize = 3f;
        float yMaximum = 20;
        for (int i = 0; i < values.Count; i++){
            float xPosition = i * xSize;
            float yPosition = values[i] / yMaximum * graphHeight;
            CreateCircle(new Vector2(xPosition, yPosition));
        }
    }
}
