using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text.RegularExpressions;

public class UIManager : MonoBehaviour
{
    public StockDataManager stockDataManager;    
    public TickerManager tickerManager;
    public AssetHolder assetHolder;

    public TextMeshProUGUI dateText;
    public TMP_Dropdown stockDropdown;
    public TMP_InputField quantityField;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI cashText;
    public List<GameObject> holdingsText;
    public UIGridRenderer gridRenderer;
    public UILineRenderer lineRenderer;

    public List<GameObject> pies;

    //1 is full and 2 will only render half of vertices
    public int GRAPH_RESOLUTION = 1;
    public int GRAPH_BUFFER = 10;

    //holdings colors
    Color GREEN = new Color(0.344f, 1f, 0.491f);
    Color RED = new Color(1f, 0.345f, 0.443f);
    Color BLUE = new Color(0.345f, 0.911f, 1f);
    Color WHITE = new Color(1f, 1f, 1f);

    // Start is called before the first frame update
    public void Initialize() {
        stockDataManager = FindObjectOfType<StockDataManager>();
        tickerManager = FindObjectOfType<TickerManager>();
        assetHolder = FindObjectOfType<AssetHolder>();
        gridRenderer = FindObjectOfType<UIGridRenderer>();
        lineRenderer = FindObjectOfType<UILineRenderer>();
        holdingsText.Add(GameObject.Find("HoldingsText"));

        tickerManager.UpdateTextContent(stockDataManager.GetStockSummary());
        dateText = GameObject.Find("DateText").GetComponent<TextMeshProUGUI>();
        stockDropdown = GameObject.Find("StockChoices").GetComponent<TMP_Dropdown>();
        quantityField = GameObject.Find("QuantityField").GetComponent<TMP_InputField>();
        descriptionText = GameObject.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
        cashText = GameObject.Find("CashText").GetComponent<TextMeshProUGUI>();

        pies = new List<GameObject>();
        pies.Add(GameObject.Find("PieChart"));
    }

    public void UpdateDayUI(int newDay){
        Stock s = stockDataManager.stocks[0];
        //Debug.Log(s.name);
        Day d = s.days[newDay];
        Date date = d.date;
        dateText.text = d.DateToString();
        tickerManager.Start();
        tickerManager.UpdateTextContent(stockDataManager.GetStockSummary());
        OnStockChange();
    }

    public void InitializeUI(){
        Initialize();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        for(int i = 0; i < stockDataManager.stocks.Count; i++){
            Stock s = stockDataManager.stocks[i];
            options.Add(new TMP_Dropdown.OptionData(s.symbol));
        }
        stockDropdown.options = options;
        ValidateQuantityField();
        UpdateAssetsUI();
    }

    public void ValidateQuantityField(){
        quantityField.text = Regex.Replace(quantityField.text, "[^0-9]", "");
        UpdateDescriptionText();
    }

    public void UpdateDescriptionText(){
        Color ERROR_COLOR = RED;

        string symbol = stockDropdown.options[stockDropdown.value].text;
        float price = stockDataManager.FindStock(symbol).TryGetCloseOnDay(stockDataManager.currentDay);
        string text = "";
        if (quantityField.text != "") {
            int quantity = Int32.Parse(quantityField.text);
            float total = quantity * price;
            text = quantity + " shares of " + symbol + " at $" + price + " each = $" + total + " total";
            descriptionText.color = WHITE;
        }

        if (price <= 0) {
            text = "sorry, there is no price data for " + symbol + " on " + stockDataManager.stocks[0].days[stockDataManager.currentDay].DateToString();
            descriptionText.color = ERROR_COLOR;
        }

        descriptionText.text = text;
    }

    public void OnStockChange(){
        lineRenderer.ClearPoints();
        List<Vector2> points = new List<Vector2>();

        string symbol = stockDropdown.options[stockDropdown.value].text;
        float max = 0;
        float min = 1000000000f;
        Stock s = stockDataManager.FindStock(symbol);
        int today = stockDataManager.currentDay;
        for(int i = today; i < s.days.Length - 1; i+=GRAPH_RESOLUTION){
            Debug.Log("adding day" + i);
            float close = s.days[i].close;
            if (close > max) max = close;
            if (close < min) min = close;
            points.Add(new Vector2(i - today, close));
        }
        lineRenderer.SetPoints(points);
        float unitHeight = lineRenderer.height / (max - min);
        lineRenderer.maxY = max + (GRAPH_BUFFER / unitHeight);
        lineRenderer.minY = Math.Max(min - (GRAPH_BUFFER / unitHeight), 0);
        lineRenderer.maxX = s.days.Length - today;

        UpdateDescriptionText();
    }

    public void UpdateAssetsUI(){
        cashText.text = "Cash: " + assetHolder.GetCashString();

        FixSizeOfHoldingsText();
        UpdateHoldingsText();

        FixSizeOfPies();
        UpdatePies();
    }

    void FixSizeOfHoldingsText(){
        int i = assetHolder.holdings.Count;
        if (i > holdingsText.Count){
            for(int j = holdingsText.Count; j < i; j++){
                Debug.Log("adding holding");
                GameObject lead = holdingsText[0];
                Vector3 position = 
                    new Vector3(lead.transform.position.x, 
                                lead.transform.position.y - (40f * j * lead.transform.lossyScale.x),
                                lead.transform.position.z);
                holdingsText.Add(Instantiate(lead, position, lead.transform.rotation, lead.transform.parent));
            }
        } else if (i == holdingsText.Count){

        } else if (i < holdingsText.Count){
            Debug.Log("removing holding");
            for(int j = Math.Max(i, 1); j < holdingsText.Count; j++){ 
                GameObject h = holdingsText[holdingsText.Count - 1];
                holdingsText.RemoveAt(holdingsText.Count - 1);
                GameObject.Destroy(h);
            }
        }
        if (holdingsText.Count < 1) Debug.LogError("holdingsText less than 1");
    }

    void UpdateHoldingsText(){
        assetHolder.holdings = SortHoldings(assetHolder.holdings);
        List<Holding> loh = assetHolder.holdings;
        for (int i = 0; i < holdingsText.Count; i++){
            if (i < assetHolder.holdings.Count){
                holdingsText[i].GetComponent<TextMeshProUGUI>().text 
                    = stockDataManager.HoldingString(loh[i]);
                //set color too
                holdingsText[i].GetComponent<TextMeshProUGUI>().color = GetColor2(loh[i].GetCurrentPrice() - loh[i].buyPrice);
            } else {
                holdingsText[i].GetComponent<TextMeshProUGUI>().text 
                    = "";
            }
        }
    }

    Color GetColor(float percent){
        float H = percent * 0.4f;
        float S = 0.9f;
        float V = 0.9f;
        return Color.HSVToRGB(H, S, V);
    }

    Color GetColor2(float difference){
        if (Math.Abs(difference) < 3f){
            return BLUE;
        } else if (difference > 0) {
            return GREEN;
        } else {
            return RED;
        }
    }

    List<Holding> SortHoldings(List<Holding> loh){
        return loh;
    }

    public void RequestBuy(){
        if (quantityField.text == "") return;
        int quantity = Int32.Parse(quantityField.text);
        string symbol = stockDropdown.options[stockDropdown.value].text;
        float price = stockDataManager.FindStock(symbol).TryGetCloseOnDay(stockDataManager.currentDay);
        if (price > 0) stockDataManager.Buy(symbol, quantity);
    }

    public void RequestSell(){
        if (quantityField.text == "") return;
        int quantity = Int32.Parse(quantityField.text);
        string symbol = stockDropdown.options[stockDropdown.value].text;
        float price = stockDataManager.FindStock(symbol).TryGetCloseOnDay(stockDataManager.currentDay);
        if (price > 0) stockDataManager.Sell(symbol, quantity);
    }

    public void FixSizeOfPies(){
        int i = stockDataManager.stocks.Count;

        if (i > pies.Count){
            for(int j = pies.Count; j < i; j++){
                Debug.Log("adding pie");
                GameObject lead = pies[0];
                Debug.Log(lead);
                Vector3 position = lead.transform.position;
                GameObject g = Instantiate(lead, position, lead.transform.rotation, lead.transform.parent);
                pies.Add(g);
            }
        } else if (i == pies.Count){

        } else if (i < pies.Count){
            Debug.Log("removing pie");
            for(int j = Math.Max(i, 1); j < pies.Count; j++){ 
                GameObject h = pies[pies.Count - 1];
                pies.RemoveAt(pies.Count - 1);
                GameObject.Destroy(h);
            }
        }
        if (pies.Count < 1) Debug.LogError("amount of pies less than 1 (one has to be saved to be copied)");
    }

    Color[] PIE_COLORS = new Color[] {
        new Color(255f / 255f, 56f / 255f, 56f / 255f), //bright red
        new Color(255f / 255f, 142f / 255f, 56f / 255f), //orange
        new Color(255f / 255f, 255f / 255f, 56f / 255f), //bright yellow
        new Color(255f / 255f, 116f / 255f, 56f / 255f), //red orange
        new Color(255f / 255f, 215f / 255f, 56f / 255f), //yellow orange
        new Color(146f / 255f, 255f / 255f, 56f / 255f), //lime
        new Color(56f / 255f, 255f / 255f, 86f / 255f), //bright green
        new Color(56f / 255f, 255f / 255f, 192f / 255f), //green blue
        new Color(56f / 255f, 255f / 255f, 248f / 255f), //bright cyan
        new Color(56f / 255f, 209f / 255f, 255f / 255f), //sky blue
        new Color(56f / 255f, 139f / 255f, 255f / 255f), //medium blue
        new Color(56f / 255f, 59f / 255f, 255f / 255f), //purpley blue
        new Color(122f / 255f, 56f / 255f, 255f / 255f), //purple
        new Color(192f / 255f, 56f / 255f, 255f / 255f), //pink purple
        new Color(255f / 255f, 56f / 255f, 255f / 255f), //bright pink
        new Color(255f / 255f, 56f / 255f, 179f / 255f) //pink red
        };

    void UpdatePies(){
        float[] values = assetHolder.HoldingsValueBySymbol();
        float totalValue = Sum(values);
        float valOfPrevious = 0;
        int usedColors = 0;
        for(int i = values.Length - 1; i >= 0; i--){
            if(totalValue > 0){
                if(values[i] > 0){
                    pies[i].SetActive(true);
                    Image pie = pies[i].GetComponent<Image>();
                    float myFill = values[i] / totalValue;
                    Debug.Log(i + ", myfill: " + myFill + ", previous: " + valOfPrevious);
                    pie.fillAmount = valOfPrevious + myFill;
                    valOfPrevious += myFill;
                    pie.color = PIE_COLORS[usedColors];
                    usedColors++;
                } else {
                    pies[i].SetActive(false);
                }
            } else if (totalValue == 0){
                pies[i].SetActive(false);
            } else {
                Debug.LogError("total value is less than zero");
                return;
            }
        }
    }

    float Sum(float[] nums){
        float sum = 0;
        for(int i = 0; i < nums.Length; i++){
            sum += nums[i];
        }
        return sum;
    }
}
