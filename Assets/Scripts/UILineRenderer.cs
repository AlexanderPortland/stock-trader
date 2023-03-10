using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : Graphic
{
    private Vector2 gridSize;
    [SerializeField]
    private List<Vector2> points;

    float width;
    public float height;
    float unitWidth;
    float unitHeight;

    public float maxX;
    public float maxY;
    public float minX;
    public float minY;

    public float thickness = 10f;

    public UILineRenderer(List<Vector2> points){
        this.points = points;
    }

    public UILineRenderer(LinearFunction func){
        for(int i = 0; i < 10; i++){
            Vector2 p = new Vector2(i, func.f(i));
            points.Add(p);
            Debug.Log(p);
        }
    }

    public void SetFunction(LinearFunction func){
        points = new List<Vector2>();
        points.Add(new Vector2(0, func.f(0)));
        points.Add(new Vector2(maxX, func.f(maxX)));
        SetAllDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh){
        vh.Clear();

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        if (maxX <= minX || maxY <= minY) Debug.LogError("misconfigured maxes and mins");

        gridSize = new Vector2(maxX - minX, maxY - minY);

        unitWidth = width / (float) gridSize.x;
        unitHeight = height / (float) gridSize.y;

        if(points.Count < 2) return;

        float angle = 0;
        for (int i = 0; i < points.Count - 1; i++) {

            Vector2 point = ScalePoint(points[i]);
            Vector2 point2 = ScalePoint(points[i+1]);

            if (i < points.Count - 1) {
                angle = GetAngle(point, point2) + 90f;
            }
            DrawVerticesForPoint(point, point2, angle, vh);
        }

        for (int i = 0; i < points.Count - 1; i++) {
            int index = i * 4;
            vh.AddTriangle(index + 0, index + 1, index + 2);
            vh.AddTriangle(index + 1, index + 2, index + 3);
            if (i < points.Count - 2){
                vh.AddTriangle(index + 2, index + 3, index + 4);
                vh.AddTriangle(index + 3, index + 4, index + 5);
            }
        }
    }

    public Vector2 ScalePoint(Vector2 p){
        return new Vector2((p.x - minX) * -1, p.y - minY);
        //return new Vector2(p.x, p.y);
    }

    public float GetAngle(Vector2 me, Vector2 target) {
        me = new Vector3(unitWidth * me.x, unitHeight * me.y);
        target = new Vector3(unitWidth * target.x, unitHeight * target.y);
        //panel resolution go there in place of 9 and 16
        float yRes = height;
        float xRes = width;

        //return (float)(Mathf.Atan2(9f*(target.y - me.y), 16f*(target.x - me.x)) * (180 / Mathf.PI));
        return (float)(Mathf.Atan2(yRes*(target.y - me.y), xRes*(target.x - me.x)) * (180 / Mathf.PI));
    }   

    void DrawVerticesForPoint(Vector2 point, Vector2 point2, float angle, VertexHelper vh) {

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point2.x, unitHeight * point2.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point2.x, unitHeight * point2.y);
        vh.AddVert(vertex);
    }

    public void SetPoints(List<Vector2> points){
        this.points = points;
        SetAllDirty();
    }

    public void ClearPoints(){
        this.points = new List<Vector2>();
    }
}
