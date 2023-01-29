using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGridRenderer : Graphic
{
    public float thickness = 10f;
    public Vector2Int gridSize = new Vector2Int(1, 1);
    [Range(0.0001f, 0.15f)]
    public float scale = 1;

    float width;
    float height;
    float cellWidth;
    float cellHeight;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        gridSize = new Vector2Int((int)(rectTransform.rect.width * scale), (int)(rectTransform.rect.height * scale));
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        cellWidth = width/(float)gridSize.x;
        cellHeight = height/(float) gridSize.y;

        int count = 0;
        for(int y = 0; y < gridSize.y; y++){
            for(int x = 0; x < gridSize.x; x++){
                DrawCell(x, y, count, vh);
                count++;
            }
        }

        

    }

    void DrawCell(int x, int y, int index, VertexHelper vh){
        float yPos = y * cellHeight;
        float xPos = x * cellWidth;

        float sqrThickness = thickness;
        if((sqrThickness * 2) > Mathf.Min(width, height)) sqrThickness = Mathf.Min(width, height)/2;

        float xPosAlone = xPos;
        float xPosRight = xPos + cellWidth;
        float yPosAlone = yPos;
        float yPosRight = yPos + cellHeight;

        if (x == 0){
            xPosAlone -= sqrThickness;
        } else if (x == (gridSize.x - 1)) {
            xPosRight += sqrThickness;
        }

        if (y == 0){
            yPosAlone -= sqrThickness;
        } else if (y == (gridSize.y - 1)) {
            yPosRight += sqrThickness;
        }

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = new Vector3(xPosAlone, yPosAlone);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPosAlone, yPosRight);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPosRight, yPosRight);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPosRight, yPosAlone);
        vh.AddVert(vertex);

        float widthSqr = sqrThickness * sqrThickness;
        float distanceSqr = widthSqr / 2;
        float distance = Mathf.Sqrt(distanceSqr);

        vertex.position = new Vector3(xPos + distance, yPos + distance);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + distance, yPos + (cellHeight - distance));
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + (cellWidth - distance), yPos + (cellHeight - distance));
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + (cellWidth - distance), yPos + distance);
        vh.AddVert(vertex);

        int offset = index * 8;

        //left edge
        vh.AddTriangle(offset + 0, offset + 1, offset + 5);
        vh.AddTriangle(offset + 5, offset + 4, offset + 0);

        //top edge
        vh.AddTriangle(offset + 1, offset + 2, offset + 6);
        vh.AddTriangle(offset + 6, offset + 5, offset + 1);

        //right edge
        vh.AddTriangle(offset + 2, offset + 3, offset + 7);
        vh.AddTriangle(offset + 7, offset + 6, offset + 2);

        //bottom edge
        vh.AddTriangle(offset + 3, offset + 0, offset + 4);
        vh.AddTriangle(offset + 4, offset + 7, offset + 3); 
    }
}
