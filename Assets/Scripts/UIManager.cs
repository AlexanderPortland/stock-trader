using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public GameObject holdingsTextPrefab;
    public TextMeshProUGUI[] holdingsTexts;

    // Start is called before the first frame update
    void Start() {
        stockDataManager = GetComponent<StockDataManager>();
        tickerManager = FindObjectOfType<TickerManager>();
        assetHolder = FindObjectOfType<AssetHolder>();

        tickerManager.UpdateTextContent(stockDataManager.GetStockSummary());
        dateText = GameObject.Find("DateText").GetComponent<TextMeshProUGUI>();
        stockDropdown = GameObject.Find("StockChoices").GetComponent<TMP_Dropdown>();
        quantityField = GameObject.Find("QuantityField").GetComponent<TMP_InputField>();
        descriptionText = GameObject.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
        cashText = GameObject.Find("CashText").GetComponent<TextMeshProUGUI>();
    }

    public void UpdateDayUI(int newDay){
        Stock s = stockDataManager.stocks[0];
        //Debug.Log(s.name);
        Day d = s.days[newDay];
        Date date = d.date;
        dateText.text = d.DateToString();
        tickerManager.UpdateTextContent(stockDataManager.GetStockSummary());
    }

    public void InitializeUI(){
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
        Color ERROR_COLOR = new Color(1f, 0.345f, 0.443f);
        if (quantityField.text == "") {
            descriptionText.text = "";
            return;
        }
        int quantity = Int32.Parse(quantityField.text);
        string symbol = stockDropdown.options[stockDropdown.value].text;

        float price = stockDataManager.FindStock(symbol).TryGetCloseOnDay(stockDataManager.currentDay);
        float total = quantity * price;
        string text = quantity + " shares of " + symbol + " at $" + price + " each = $" + total + " total";
        descriptionText.color = new Color(1f, 1f, 1f);
        if (price <= 0) {
            text = "sorry, there is no price data for " + symbol + " on " + stockDataManager.stocks[0].days[stockDataManager.currentDay].DateToString();
            descriptionText.color = ERROR_COLOR;
        }
        descriptionText.text = text;
    }

    public void UpdateAssetsUI(){
        cashText.text = "Cash: " + assetHolder.GetCashString();

    }

    public void RequestBuy(){
        int quantity = Int32.Parse(quantityField.text);
        string symbol = stockDropdown.options[stockDropdown.value].text;
        float price = stockDataManager.FindStock(symbol).TryGetCloseOnDay(stockDataManager.currentDay);
        if (price > 0) stockDataManager.Buy(symbol, quantity);
    }

    public void RequestSell(){
        int quantity = Int32.Parse(quantityField.text);
        string symbol = stockDropdown.options[stockDropdown.value].text;
        float price = stockDataManager.FindStock(symbol).TryGetCloseOnDay(stockDataManager.currentDay);
        if (price > 0) stockDataManager.Sell(symbol, quantity);
    }
}
