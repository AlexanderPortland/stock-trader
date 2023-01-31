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
    public UILineRenderer lineBestFit;
    public UILineRenderer[] allLines;

    public TextMeshProUGUI valueText;
    public TextMeshProUGUI maxText;
    public TextMeshProUGUI minText;
    public TextMeshProUGUI priceText;

    public List<GameObject> pies;

    //1 is full and 2 will only render half of vertices
    public int GRAPH_RESOLUTION = 1;
    public int GRAPH_BUFFER = 10;
    public int GRAPH_DAYS_BACK = 260; //260 for year, 130 for 6 months, 20 for month, 5 for week
    public int GRAPH_SCALING_DAYS_BACK = 260;

    float NEGATIVE_SLOPE_HUE = 0f;
    float POSITIVE_SLOPE_HUE = 126f / 360f;
    float SIGMOID_SCALING_CONST = 10f;

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
        lineRenderer = GameObject.Find("MAIN").GetComponent<UILineRenderer>();
        lineBestFit = GameObject.Find("BestFit").GetComponent<UILineRenderer>();
        allLines = FindObjectsOfType<UILineRenderer>();
        holdingsText.Add(GameObject.Find("HoldingsText"));

        tickerManager.UpdateTextContent(stockDataManager.GetStockSummary());
        dateText = GameObject.Find("DateText").GetComponent<TextMeshProUGUI>();
        stockDropdown = GameObject.Find("StockChoices").GetComponent<TMP_Dropdown>();
        quantityField = GameObject.Find("QuantityField").GetComponent<TMP_InputField>();
        descriptionText = GameObject.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
        cashText = GameObject.Find("CashText").GetComponent<TextMeshProUGUI>();
        valueText = GameObject.Find("ValueText").GetComponent<TextMeshProUGUI>();
        maxText = GameObject.Find("MaxLabel").GetComponent<TextMeshProUGUI>();
        minText = GameObject.Find("MinLabel").GetComponent<TextMeshProUGUI>();
        priceText = GameObject.Find("PriceLabel").GetComponent<TextMeshProUGUI>();

        pies = new List<GameObject>();
        pies.Add(GameObject.Find("PieChart"));
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.RightArrow)){
            if(stockDropdown.value + 1 >= stockDropdown.options.Count) stockDropdown.value = 0;
            else stockDropdown.value++;
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)){
            if(stockDropdown.value - 1 < 0) stockDropdown.value = stockDropdown.options.Count - 1;
            else stockDropdown.value--;
        } else if (Input.GetKey(KeyCode.Space)){
            stockDataManager.NextDay();
        }
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
        if (price > 0) {
            priceText.text = "$" + string.Format("{0:0.00}", price);
        } else {
            priceText.text = "N/A";
        }
    }

    public void OnStockChange(){
        lineRenderer.ClearPoints();
        List<Vector2> points = new List<Vector2>();

        string symbol = stockDropdown.options[stockDropdown.value].text;
        float max = 0;
        float min = 1000000000f;
        Stock s = stockDataManager.FindStock(symbol);
        int today = stockDataManager.currentDay;
        int lastDayRender = Math.Min(s.days.Length - 1, today + GRAPH_DAYS_BACK);
        int lastDayCount = Math.Min(s.days.Length - 1, today + GRAPH_SCALING_DAYS_BACK);
        List<Vector2> pointsToFit = new List<Vector2>();
        for(int i = today; i < lastDayCount; i++){
            float close = s.days[i].close;
            if (close > max) max = close;
            if (close < min) min = close;
            if ((i - today) % GRAPH_RESOLUTION == 0 && i < lastDayRender){
                pointsToFit.Add(new Vector2(i - today, close));
                points.Add(new Vector2(i - today, close));
            }
        }
        
        foreach(UILineRenderer rend in allLines){
            float unitHeight = rend.height / (max - min);
            rend.maxY = max + (GRAPH_BUFFER / unitHeight);
            rend.minY = Math.Max(min - (GRAPH_BUFFER / unitHeight), 0);
            rend.maxX = lastDayRender - today - 1;
            rend.minX = 0;
        }
        lineRenderer.SetPoints(points);
        LinearFunction fit = Calculator.FindLineOfBestFit(pointsToFit);
        lineBestFit.SetFunction(fit);
        float StandardDeviationShiftDown = Calculator.StandardDeviationShiftDown(fit, pointsToFit);
        GameObject.Find("TopSD").GetComponent<UILineRenderer>().SetFunction(new LinearFunction(fit.m, fit.b + StandardDeviationShiftDown));
        GameObject.Find("BottomSD").GetComponent<UILineRenderer>().SetFunction(new LinearFunction(fit.m, fit.b - StandardDeviationShiftDown));

        float hue = Mathf.Lerp(POSITIVE_SLOPE_HUE, NEGATIVE_SLOPE_HUE, Sigmoid(fit.m * SIGMOID_SCALING_CONST));
        lineBestFit.color = Color.HSVToRGB(hue, 0.75f, 1f);
        
        maxText.text = assetHolder.FancifyMoneyText(max);
        minText.text = assetHolder.FancifyMoneyText(min);

        UpdateDescriptionText();
    }

    public void UpdateAssetsUI(){
        cashText.text = "Cash: " + assetHolder.GetCashString();

        FixSizeOfHoldingsText();
        UpdateHoldingsText();

        FixSizeOfPies();
        UpdatePies();
        OnStockChange();
    }

    void FixSizeOfHoldingsText(){
        int i = assetHolder.holdings.Count;
        if (i > holdingsText.Count){
            for(int j = holdingsText.Count; j < i; j++){
                //Debug.Log("adding holding");
                GameObject lead = holdingsText[0];
                Vector3 position = 
                    new Vector3(lead.transform.position.x, 
                                lead.transform.position.y - (40f * j * lead.transform.lossyScale.x),
                                lead.transform.position.z);
                holdingsText.Add(Instantiate(lead, position, lead.transform.rotation, lead.transform.parent));
            }
        } else if (i == holdingsText.Count){

        } else if (i < holdingsText.Count){
            //Debug.Log("removing holding");
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
        int i = stockDataManager.stocks.Count + 1;

        if (i > pies.Count){
            for(int j = pies.Count; j < i; j++){
                GameObject lead = pies[0];
                Vector3 position = lead.transform.position;
                GameObject g = Instantiate(lead, position, lead.transform.rotation, lead.transform.parent);
                pies.Add(g);
            }
        } else if (i == pies.Count){

        } else if (i < pies.Count){
            for(int j = Math.Max(i, 1); j < pies.Count; j++){ 
                GameObject h = pies[pies.Count - 1];
                unusedPieColors.Add(h.GetComponent<Image>().color);
                pies.RemoveAt(pies.Count - 1);
                GameObject.Destroy(h);
            }
        }
        if (pies.Count < 1) Debug.LogError("amount of pies less than 1 (one has to be saved to be copied)");
    }

    public List<Color> unusedPieColors = new List<Color> {
        new Color(255f / 255f, 56f / 255f, 56f / 255f), //bright red
        new Color(255f / 255f, 142f / 255f, 56f / 255f), //orange
        new Color(255f / 255f, 255f / 255f, 56f / 255f), //bright yellow
        //new Color(255f / 255f, 116f / 255f, 56f / 255f), //red orange
        //new Color(255f / 255f, 215f / 255f, 56f / 255f), //yellow orange
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
        float totalValue = Sum(values) + AssetHolder.cash;
        float valOfPrevious = 0;
        for(int i = values.Length; i >= 1; i--){
            if(totalValue > 0){
                if(values[i - 1] > 0){
                    Image pie = pies[i].GetComponent<Image>();
                    if (!pies[i].activeInHierarchy){
                        pies[i].SetActive(true);
                        int index = UnityEngine.Random.Range((int)0, (int)unusedPieColors.Count);
                        pie.color = unusedPieColors[index];
                        unusedPieColors.RemoveAt(index);
                    }
                    
                    float myFill = values[i - 1] / totalValue;
                    pie.fillAmount = valOfPrevious + myFill;
                    valOfPrevious += myFill;
                } else {
                    if (pies[i].activeInHierarchy){
                        pies[i].SetActive(false);
                        Image pie = pies[i].GetComponent<Image>();
                        if (pie.color != Color.white) unusedPieColors.Add(pie.color);
                        pie.color = Color.white;
                    }
                }
            } else if (totalValue == 0){
                pies[i].SetActive(false);
            } else {
                Debug.LogError("total value is less than zero");
                return;
            }
        }
        valueText.text = assetHolder.FancifyMoneyText(totalValue) + " in assets";
    }

    float Sum(float[] nums){
        float sum = 0;
        for(int i = 0; i < nums.Length; i++){
            sum += nums[i];
        }
        return sum;
    }

    public static float Sigmoid(double value) {
        float k = (float)Math.Exp(value);
        return k / (1.0f + k);
    }
}
