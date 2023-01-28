using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Stock : MonoBehaviour
{
    public string symbol;
    public Day[] days;

    public Stock(string symbol){
        this.symbol = symbol;
        string path = symbol + ".csv";
        StreamReader reader = File.OpenText(Directory.GetCurrentDirectory() + "/assets/scripts/data/" + path);
        string[] lines = reader.ReadToEnd().Split('\n');

        days = new Day[lines.Length - 1];
        for (int i = 0; i < lines.Length - 1; i ++){
            string[] commas = lines[i + 1].Split(',');
            if (commas.Length == 6) {
                days[i] = new Day(new Date(commas[0], "mdy"),
                                            DollarStringToFloat(commas[1]), //close
                                            DollarStringToFloat(commas[3]), //open
                                            DollarStringToFloat(commas[4]), //high
                                            DollarStringToFloat(commas[5]), //low
                                            Int64.Parse(commas[2]) //volume
                                            );
            } else {
                Debug.LogWarning("line \"" + lines[i + 1] + "\" found which doesnt have length 6");
                //(which is normal for the end of nasdaq downloaded csv files)
            }
        }
    }

    public float TryGetCloseOnDay(int day){
        if (day < days.Length) return days[day].close;
        else return -1f;
    }

    public float DollarStringToFloat(string s){
        return float.Parse(
            s.Trim(new Char[]{ '$' }));
    }

    public string DayToString(int index){
        if (index < days.Length) return symbol + " -- " + days[index].ToString();
        else {
            Debug.LogError("index " + index + " too big for " + symbol + " days array of only size " + days.Length);
            return "";
        }
        
    }
}

public struct Day {
    public Date date;
    public float close;
    public float open;
    public float high;
    public float low;
    double volume;

    public Day(Date date, float close, float open, float high, float low, double volume){
        this.date = date;
        this.close = close;
        this.open = open;
        this.high = high;
        this.low = low;
        this.volume = volume;
    }

    public string DateToString(){
        return date.ToString();
    }

    public override string ToString(){
        return DateToString() + " | close: " + close + " | open: " + open + " | high: " + high;
    }
}

public struct Date {
    int day;
    int month;
    int year;

    public Date(int day, int month, int year){
        this.day = day;
        this.month = month;
        this.year = year;
    }
    
    public Date(string date, string format){
        string DATE_SEPERATOR = "/";
        string[] slashes = date.Split(DATE_SEPERATOR);
        if (format == "mdy"){
            this.day = Int32.Parse(slashes[1]);
            this.month = Int32.Parse(slashes[0]); 
            this.year = Int32.Parse(slashes[2]);
        } else if (format == "dmy"){
            this.day = Int32.Parse(slashes[0]);
            this.month = Int32.Parse(slashes[1]); 
            this.year = Int32.Parse(slashes[2]);
        } else {
            Debug.LogError("no date format \"" + format + "\" found");
            this.day = Int32.Parse(slashes[1]);
            this.month = Int32.Parse(slashes[0]); 
            this.year = Int32.Parse(slashes[2]);
        }
    }

    public override string ToString(){
        string DATE_SEPERATOR = "/";
        return day + DATE_SEPERATOR + month + DATE_SEPERATOR + year;
    }
}