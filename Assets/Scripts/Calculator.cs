using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq.Expressions;

public class Calculator : MonoBehaviour
{
    
    public static LinearFunction FindLineOfBestFit(List<Vector2> data){
        float x = 0;
        float y = 0;
        float xy = 0;
        float xSqr = 0;
        for(int i = 0; i < data.Count; i++){
            x += i;
            y += data[i].y;
            xy += i * data[i].y;
            xSqr += i * i;
        }
        //plot x, y, xy, and x squared
        //sum the columns for each value
        //slope = m or b1
        //y-intercept = b or b0
        int n = data.Count;
        float m = (n * xy - (x * y)) / (n * xSqr - Mathf.Pow(x, 2));
        float b = (y - m * x) / n;
        LinearFunction f = new  LinearFunction(m, b);
        Debug.Log(f);
        return f;
    }
}

public struct LinearFunction {
    public float m;
    public float b;

    public LinearFunction(float m, float b){
        this.m = m;
        this.b = b;
    }

    public float f(float x){
        return m * x + b;
    }

    public override string ToString()
    {
        if (b < 0){
            return "y = " + m + "x - " + Mathf.Abs(b);
        }
        return "y = " + m + "x + " + b;
    }   
}
