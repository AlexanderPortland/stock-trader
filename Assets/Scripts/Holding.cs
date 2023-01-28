using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holding : MonoBehaviour
{
    public string symbol;
    public float buyPrice;
    public float buyQuantity;

    public Holding(string _symbol, float _quantity, float _price){
        symbol = _symbol;
        buyPrice = _price;
        buyQuantity = _quantity;
    }
}
