using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TickerManager : MonoBehaviour
{
    public GameObject tickerText;
    private float minX;
    public float scrollSpeed;
    private float offset;
    public float textWidth;

    public void Awake(){
        tickerText = GameObject.Find("TickerText");
        if (tickerText == null) Debug.LogError("no text with name \"TickerText\" found");
        UpdateTextWidth();
    }

    public void Update(){
        UpdatePosition();
    }

    void UpdatePosition(){
        offset -= scrollSpeed * Time.deltaTime;
        Vector3 lastPosition = tickerText.transform.position;
        float newX = (offset % textWidth) - minX;
        tickerText.transform.position = new Vector3(newX, lastPosition.y, lastPosition.z);
    }

    public void UpdateTextContent(string s){
        tickerText.GetComponent<TextMeshProUGUI>().text = s; 
        UpdateTextWidth();
    }

    void UpdateTextWidth(){
        Bounds bounds = tickerText.GetComponent<TextMeshProUGUI>().bounds;
        Debug.Log(bounds);
        textWidth = bounds.max.x - bounds.min.x;
    }
}
