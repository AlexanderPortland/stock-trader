using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TickerManager : MonoBehaviour
{
    public GameObject tickerText;
    public List<GameObject> texts;
    private float minX;
    public float scrollSpeed;
    private float offset;
    public float textWidth;

    public void Awake(){
        tickerText = GameObject.Find("TickerText");
        if (tickerText == null) Debug.LogError("no text with name \"TickerText\" found");
        UpdateTextWidth();
        //texts.Add(tickerText)
    }

    public void Update(){
        UpdatePosition();
    }

    void UpdatePosition(){
        offset -= scrollSpeed * Time.deltaTime;
        Vector3 lastPosition = tickerText.transform.position;
        float x = (offset % (2 * textWidth));
        float newX = x - minX;
        Debug.Log(x);
        tickerText.transform.position = new Vector3(newX, lastPosition.y, lastPosition.z);
    }

    public void UpdateTextContent(string s){
        tickerText.GetComponent<TextMeshProUGUI>().text = s; 
        UpdateTextWidth();
    }

    void UpdateTextWidth(){
        textWidth = tickerText.GetComponent<TextMeshProUGUI>().preferredWidth * transform.lossyScale.x;
    }
}
