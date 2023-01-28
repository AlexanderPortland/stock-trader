using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TickerManager : MonoBehaviour
{
    public GameObject tickerText;
    public GameObject tickerText1;
    public List<GameObject> texts;
    private float minX = 25f;
    public float scrollSpeed;
    private float offset;
    public float textWidth;

    public void Awake(){
        tickerText = GameObject.Find("TickerText");
        tickerText1 = GameObject.Find("TickerText1");
        if (tickerText == null) Debug.LogError("no text with name \"TickerText\" found");
        if (tickerText1 == null) Debug.LogError("no text with name \"TickerText1\" found");
        UpdateTextWidth();
    }

    public void Update(){
        UpdatePosition();
    }

    void UpdatePosition(){
        offset += scrollSpeed * Time.deltaTime;
        Vector3 lastPosition = tickerText.transform.position;
        float x = (offset % (2 * textWidth));
        float newX = x - minX;
        tickerText.transform.position = new Vector3(-1 * newX, lastPosition.y, lastPosition.z);

        Vector3 lastPosition1 = tickerText1.transform.position;
        float x1 = ((offset + textWidth) % (2 * textWidth));
        float newX1 = x1 - minX;
        tickerText1.transform.position = new Vector3(-1 * newX1, lastPosition.y, lastPosition.z);
    }

    public void UpdateTextContent(string s){
        tickerText.GetComponent<TextMeshProUGUI>().text = s; 
        tickerText1.GetComponent<TextMeshProUGUI>().text = s; 
        UpdateTextWidth();
    }

    void UpdateTextWidth(){
        textWidth = tickerText.GetComponent<TextMeshProUGUI>().preferredWidth * transform.lossyScale.x;
    }
}
