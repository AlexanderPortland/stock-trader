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
    public TextMeshProUGUI dateText;
    public TMP_Dropdown stockDropdown;
    public TMP_InputField quantityField;
    public TextMeshProUGUI descriptionText;

    // Start is called before the first frame update
    void Start() {
        stockDataManager = GetComponent<StockDataManager>();
        tickerManager = FindObjectOfType<TickerManager>();
        tickerManager.UpdateTextContent(stockDataManager.GetStockSummary());
        dateText = GameObject.Find("DateText").GetComponent<TextMeshProUGUI>();
        stockDropdown = GameObject.Find("StockChoices").GetComponent<TMP_Dropdown>();
        quantityField = GameObject.Find("QuantityField").GetComponent<TMP_InputField>();
        descriptionText = GameObject.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
    }

    public void UpdateDayUI(int newDay){
        dateText.text = stockDataManager.stocks[0].days[newDay].DateToString();
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
    }

    public void ValidateQuantityField(){
        quantityField.text = Regex.Replace(quantityField.text, "[^0-9]", "");
        UpdateDescriptionText();
    }

    public void UpdateDescriptionText(){
        if (quantityField.text == "") {
            descriptionText.text = "";
            return;
        }
        int quantity = Int32.Parse(quantityField.text);
        string symbol = stockDropdown.options[stockDropdown.value].text;

        float price = stockDataManager.FindStock(symbol).TryGetCloseOnDay(stockDataManager.currentDay);
        float total = quantity * price;
        string text = quantity + " shares of " + symbol + " at $" + price + " each = $" + total + " total";
        if (price <= 0) text = "sorry, there is no price data for " + symbol + " on " + stockDataManager.stocks[0].days[stockDataManager.currentDay].DateToString();
        descriptionText.text = text;
    }
}
