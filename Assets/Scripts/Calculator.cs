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

    public static float StandardDeviation(LinearFunction func, List<Vector2> data){
        float sumDistSquared = 0;
        for(int i = 0; i < data.Count; i++){
            float dist = DistanceFromLine(func, data[i]);
            sumDistSquared += Mathf.Pow(dist, 2);
        }
        return Mathf.Sqrt(sumDistSquared / data.Count);
    }

    static float DistanceFromLine(LinearFunction func, Vector2 p0){
        float x1 = 0; //dont change
        float x2 = 1;

        Vector2 p1 = new Vector2(x1, func.f(x1));
        Vector2 p2 = new Vector2(x2, func.f(x2));

        return Mathf.Abs(((1) * (p1.y - p0.y)) - ((p1.x - p0.x) * (p2.y - p1.y))) 
                        / Mathf.Sqrt(2);
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
