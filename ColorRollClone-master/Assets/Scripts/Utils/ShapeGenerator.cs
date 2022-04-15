using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShapeGenerator 
{

    public void Generate(
        Vector2 bottomLeft,
        Vector2 bottomRight,
        Vector2 topLeft,
        Vector2 topRight,
        float unitSize, out Vector2[] vertices, out int [] indices)
    {
        float topY = Mathf.Max(topLeft.y, topRight.y);
        float bottomY = Mathf.Min(bottomLeft.y, bottomRight.y);
        float fullHeight = topY - bottomY;

        float btBottomY = bottomY;
        float btTopY = Mathf.Max(bottomLeft.y, bottomRight.y);
        float btHeight = btTopY - btBottomY;
        float btBottomX = (bottomLeft.y < bottomRight.y ? bottomLeft.x : bottomRight.x);
        float btTopLeftX = (bottomLeft.y < bottomRight.y? Mathf.Lerp(bottomLeft.x, topLeft.x, btHeight/fullHeight): bottomLeft.x);
        float btTopRightX = (bottomLeft.y < bottomRight.y ? bottomRight.x:Mathf.Lerp(bottomRight.x, topRight.x, btHeight/ fullHeight));

        int btUnitNum = Mathf.FloorToInt(btHeight / unitSize);
        float btRemainingUnitSize = btHeight - btUnitNum * unitSize;


        float pBottomY = btTopY;
        float pTopY = Mathf.Min(topLeft.y, topRight.y);
        float pHeight = pTopY - pBottomY;
        float pBottomLeftX = btTopLeftX;// Mathf.Lerp(bottomLeft.x, topLeft.x, btHeight / fullHeight);
        float pBottomRightX = btTopRightX;// Mathf.Lerp(bottomRight.x, topRight.x, btHeight / fullHeight);
        float pTopLeftX = Mathf.Lerp(bottomLeft.x, topLeft.x, (btHeight+pHeight) / fullHeight);
        float pTopRightX = Mathf.Lerp(bottomRight.x, topRight.x, (btHeight + pHeight) / fullHeight);

        int pUnitNum = Mathf.FloorToInt(pHeight / unitSize);
        float pRemainingUnitSize = pHeight - pUnitNum * unitSize;


        //generate the top triangle if any
        float tBottomY = pTopY;
        float tTopY = topY;
        float tHeight = tTopY - tBottomY;
        int tUnitNum = Mathf.FloorToInt(tHeight / unitSize);
        float tTopX = (topLeft.y > topRight.y ? topLeft.x : topRight.x);


        //float s = (bottomRight.x-bottomLeft.x) * 0.5f;
        vertices = new Vector2[(pUnitNum+1+ tUnitNum+1 +btUnitNum+1) * 2+2];
        indices = new int[(pUnitNum+1+ tUnitNum+1 + btUnitNum + 1) * 6];// { 0, 1, 2, 2, 3, 0 };



        vertices[0] = new Vector2(btBottomX, btBottomY);
        vertices[1] = new Vector2(btBottomX, btBottomY);

        for (int i = 0; i < btUnitNum + 1; i++)
        {
            float y = unitSize + unitSize * i;

            float x1 = Mathf.Lerp(btBottomX, btTopLeftX, y / btHeight);
            float x2 = Mathf.Lerp(btBottomX, btTopRightX, y / btHeight);

            vertices[i * 2 + 2] = new Vector2(x1, btBottomY + y);
            vertices[i * 2 + 3] = new Vector2(x2, btBottomY + y);

            indices[i * 6] = 2 * i;
            indices[i * 6 + 1] = 2 * i + 2;
            indices[i * 6 + 2] = 2 * i + 3;

            indices[i * 6 + 3] = 2 * i + 3;
            indices[i * 6 + 4] = 2 * i + 1;
            indices[i * 6 + 5] = 2 * i;
        }
        {
            float y = btRemainingUnitSize + unitSize * btUnitNum;
            float x1 = btTopLeftX;// Mathf.Lerp(btBottomX, btTopLeftX, y / btHeight);
            float x2 = btTopRightX;// Mathf.Lerp(btBottomX, btTopRightX, y / btHeight);

            vertices[btUnitNum * 2 + 2] = new Vector2(x1, btBottomY + y);
            vertices[btUnitNum * 2 + 3] = new Vector2(x2, btBottomY + y);
        }


        int pStartVertexIndex = btUnitNum * 2 + 2;
        int pStartIndexIndex = btUnitNum * 6 + 5 + 1;


        //vertices[pStartVertexIndex] = new Vector2(pBottomLeftX, pBottomY);
        //vertices[pStartVertexIndex +1] = new Vector2(pBottomRightX, pBottomY);

        for (int i = 0; i < pUnitNum + 1; i++)
        {
            float y = unitSize + unitSize * i;

            float x1 = Mathf.Lerp(pBottomLeftX, pTopLeftX , y / pHeight);
            float x2 = Mathf.Lerp(pBottomRightX, pTopRightX , y / pHeight);

            vertices[pStartVertexIndex +i * 2 + 2] = new Vector2(x1, pBottomY + y);
            vertices[pStartVertexIndex +i * 2 + 3] = new Vector2(x2, pBottomY + y);

            indices[pStartIndexIndex+i * 6] = pStartVertexIndex+2 * i;
            indices[pStartIndexIndex+i * 6 + 1] = pStartVertexIndex+2 * i + 2;
            indices[pStartIndexIndex+i * 6 + 2] = pStartVertexIndex+2 * i + 3;

            indices[pStartIndexIndex+i * 6 + 3] = pStartVertexIndex+2 * i + 3;
            indices[pStartIndexIndex+i * 6 + 4] = pStartVertexIndex+2 * i + 1;
            indices[pStartIndexIndex+i * 6 + 5] = pStartVertexIndex+2 * i;
        }
        {
            float y = pRemainingUnitSize + unitSize * pUnitNum;
            float x1 = Mathf.Lerp(pBottomLeftX, pTopLeftX, y / pHeight);
            float x2 = Mathf.Lerp(pBottomRightX, pTopRightX, y / pHeight);

            vertices[pStartVertexIndex + pUnitNum * 2 + 2] = new Vector2(x1, pBottomY + y);
            vertices[pStartVertexIndex+pUnitNum * 2 + 3] = new Vector2(x2, pBottomY + y);
        }



        if (tUnitNum > 0)
        {
            int tStartVertexIndex = pStartVertexIndex + pUnitNum * 2 + 2;
            int tStartIndexIndex = pStartIndexIndex + pUnitNum * 6 + 5 + 1;

            //vertices[tStartVertexIndex] = new Vector2(t, 0);
            //vertices[tStartVertexIndex + 1] = new Vector2(pBottomRightX, 0);

            for (int i = 0; i < tUnitNum + 1; i++)
            {
                float y = unitSize + unitSize * i;

                float x1 = Mathf.Lerp(pTopLeftX, tTopX, y / tHeight);
                float x2 = Mathf.Lerp(pTopRightX, tTopX, y / tHeight);

                vertices[tStartVertexIndex + i * 2 + 2] = new Vector2(x1, pHeight + y);
                vertices[tStartVertexIndex + i * 2 + 3] = new Vector2(x2, pHeight + y);

                indices[tStartIndexIndex + i * 6] = tStartVertexIndex+ 2 * i;
                indices[tStartIndexIndex + i * 6 + 1] =tStartVertexIndex+2 * i + 2;
                indices[tStartIndexIndex + i * 6 + 2] = tStartVertexIndex+ 2 * i + 3;

                indices[tStartIndexIndex + i * 6 + 3] = tStartVertexIndex+2 * i + 3;
                indices[tStartIndexIndex + i * 6 + 4] = tStartVertexIndex+2 * i + 1;
                indices[tStartIndexIndex + i * 6 + 5] = tStartVertexIndex+2 * i;
            }
            {
                float x1 = tTopX;
                vertices[tStartVertexIndex + tUnitNum * 2 + 2] = new Vector2(x1, pHeight + tHeight);
                vertices[tStartVertexIndex + tUnitNum * 2 + 3] = new Vector2(x1, pHeight + tHeight);
            }
        }
    }
    public void GenerateTriangle(Vector2 p1, Vector2 p2, Vector2 p3,
        float unitSize, out Vector2[] vertices, out int[] indices)
    {
        Vector2 v1, v2, v3;
        if (p1.y < p2.y)
        {
            if (p1.y < p3.y)
            {
                v1 = p1;
                if (p2.y < p3.y)
                {
                    v2 = p2;
                    v3 = p3;
                }
                else
                {
                    v2 = p3;
                    v3 = p2;
                }
            }
            else
            {
                v1 = p3;
                v2 = p1;
                v3 = p2;
            }
        }
        else
        {
            if (p2.y < p3.y)
            {
                v1 = p2;
                if (p1.y < p3.y)
                {
                    v2 = p1;
                    v3 = p3;
                }
                else
                {
                    v2 = p3;
                    v3 = p1;
                }
            }
            else
            {
                v1 = p3;
                v2 = p2;
                v3 = p1;
            }
        }


        float height = v3.y - v1.y;

        //lower triangle
        float height1 = v2.y - v1.y;
        int unitNum1 = Mathf.FloorToInt(height1 / unitSize);
        float remainingUnitSize1 = height1 - unitNum1 * unitSize;
        float middlePointX = Mathf.Lerp(v1.x, v3.x, height1 / height);



        //upper triangle
        float height2 = v3.y - v2.y;
        int unitNum2 = Mathf.FloorToInt(height2 / unitSize);
        float remainingUnitSize2 = height2 - unitNum2 * unitSize;

        vertices = new Vector2[(unitNum1)*2+1 + (unitNum2) * 2+1];
        indices = new int[(unitNum1)*6+3 + (unitNum2) * 6+3];

        float leftX = Mathf.Min(v2.x, middlePointX);
        float rightX = Mathf.Max(v2.x, middlePointX);

        float x01 = Mathf.Lerp(v1.x, leftX, unitSize / height1);
        float x02 = Mathf.Lerp(v1.x, rightX, unitSize / height1);

        vertices[0] = v1;
        vertices[1] = new Vector2(x01, v1.y + unitSize);
        vertices[2] = new Vector2(x02, v1.y + unitSize);

        indices[0] = 0;
        indices[1] = 1;
        indices[2] = 2;

        for (int i = 1; i < unitNum1; i++)
        {
            float h = unitSize + unitSize * i;
            float x1 = Mathf.Lerp(v1.x, leftX, h / height1);
            float x2 = Mathf.Lerp(v1.x, rightX, h / height1);

            vertices[i * 2 + 1] = new Vector2(x1,v1.y+ h);
            vertices[i * 2 + 2] = new Vector2(x2, v1.y + h);

            indices[i * 6] = i * 2 - 1;
            indices[i * 6 + 1] = i * 2 + 1;
            indices[i * 6 + 2] = i * 2 + 2;

            indices[i * 6+3] = i * 2 +2;
            indices[i * 6 + 4] = i * 2;
            indices[i * 6 + 5] = i * 2 -1;
        }

        vertices[unitNum1 * 2 + 1] = new Vector2(leftX, v2.y);
        vertices[unitNum1 * 2 + 2] = new Vector2(rightX, v2.y);
        indices[unitNum1 * 6] = unitNum1 * 2 - 1;
        indices[unitNum1 * 6 + 1] = unitNum1 * 2 + 1;
        indices[unitNum1 * 6 + 2] = unitNum1 * 2 + 2;

        indices[unitNum1 * 6 + 3] = unitNum1 * 2 + 2;
        indices[unitNum1 * 6 + 4] = unitNum1 * 2;
        indices[unitNum1 * 6 + 5] = unitNum1 * 2 - 1;


        int startingVIndex = unitNum1* 2 + 3;
        int startingIIndex = (unitNum1) * 6 + 3;

        for (int i = 0; i < unitNum2-1; i++)
        {
            float h = unitSize + unitSize * i;
            float x1 = Mathf.Lerp(leftX, v3.x, h / height2);
            float x2 = Mathf.Lerp(rightX, v3.x, h / height2);

            vertices[startingVIndex + i * 2 ] = new Vector2(x1, v2.y + h);
            vertices[startingVIndex + i * 2 + 1] = new Vector2(x2, v2.y + h);

            indices[startingIIndex + i * 6] = startingVIndex + i * 2 - 2;
            indices[startingIIndex + i * 6 + 1] = startingVIndex + i * 2;
            indices[startingIIndex + i * 6 + 2] = startingVIndex + i * 2 + 1;

            indices[startingIIndex + i * 6 + 3] = startingVIndex + i * 2 + 1;
            indices[startingIIndex + i * 6 + 4] = startingVIndex + i * 2 - 1;
            indices[startingIIndex + i * 6 + 5] = startingVIndex + i * 2 - 2;
        }

        int lastIndex = (unitNum2 - 1);
        vertices[startingVIndex + lastIndex * 2] = v3;

        indices[startingIIndex + lastIndex * 6] = startingVIndex + lastIndex * 2 - 2;
        indices[startingIIndex + lastIndex * 6 + 1] = startingVIndex + lastIndex * 2;
        indices[startingIIndex + lastIndex * 6 + 2] = startingVIndex + lastIndex * 2 - 1;
    }

    public void GeneratePolygon(Vector2 [] polygon, 
        float unitSize, float thickness, out Vector3[] vertices, out int[] indices, 
        out Vector2 leftMost, out Vector2 rightMost,
        out Vector2 bottomMost, out Vector2 topMost
        )
    {
        Triangulator triangulator = new Triangulator(polygon);
        int [] polygonIndices = triangulator.Triangulate();

        List<Vector3> outputPolygon = new List<Vector3>();
        List<int> outputPolygonIndices = new List<int>();

        leftMost = new Vector2(float.MaxValue,0);
        rightMost = new Vector2(float.MinValue,0);
        topMost = new Vector2(0, float.MinValue);
        bottomMost = new Vector2(0, float.MaxValue);
        
        for (int i = 0; i< polygonIndices.Length/3; i++)
        {
            Vector2 v1 = polygon[polygonIndices[i*3]];
            Vector2 v2 = polygon[polygonIndices[i*3+1]];
            Vector2 v3 = polygon[polygonIndices[i*3+2]];
             

            GenerateTriangle(ref v1, ref v2, ref v3, unitSize, thickness,out Vector3[] triangleVertices,out int[] triangleIndices);
            for (int j = 0; j < triangleIndices.Length; j++)
                triangleIndices[j] += outputPolygon.Count;

            outputPolygon.AddRange(triangleVertices);
            outputPolygonIndices.AddRange(triangleIndices);


            if (v1.y < bottomMost.y) bottomMost = v1;
            if (v3.y > topMost.y) topMost = v3;

            if (v1.x < leftMost.x) leftMost = v1;
            if (v2.x < leftMost.x) leftMost = v2;
            if (v3.x < leftMost.x) leftMost = v3;

            if (v1.x > rightMost.x) rightMost = v1;
            if (v2.x > rightMost.x) rightMost = v2;
            if (v3.x > rightMost.x) rightMost = v3;
        }
        vertices = outputPolygon.ToArray();
        indices = outputPolygonIndices.ToArray();
    }
    public void GenerateTriangle(ref Vector2 p1,ref Vector2 p2,ref Vector2 p3,
        float unitSize, float thickness, out Vector3[] vertices, out int[] indices)
    {
        Vector2 v1, v2, v3;
        if (p1.y < p2.y)
        {
            if (p1.y < p3.y)
            {
                v1 = p1;
                if (p2.y < p3.y)
                {
                    v2 = p2;
                    v3 = p3;
                }
                else
                {
                    v2 = p3;
                    v3 = p2;
                }
            }
            else
            {
                v1 = p3;
                v2 = p1;
                v3 = p2;
            }
        }
        else
        {
            if (p2.y < p3.y)
            {
                v1 = p2;
                if (p1.y < p3.y)
                {
                    v2 = p1;
                    v3 = p3;
                }
                else
                {
                    v2 = p3;
                    v3 = p1;
                }
            }
            else
            {
                v1 = p3;
                v2 = p2;
                v3 = p1;
            }
        }
        p1 = v1;
        p2 = v2;
        p3 = v3;

        float height = v3.y - v1.y;

        //lower triangle
        float height1 = v2.y - v1.y;
        int unitNum1 = Mathf.FloorToInt(height1 / unitSize);
        //float remainingUnitSize1 = height1 - unitNum1 * unitSize;



        //upper triangle
        float height2 = v3.y - v2.y;
        int unitNum2 = Mathf.FloorToInt(height2 / unitSize);
        //float remainingUnitSize2 = height2 - unitNum2 * unitSize;

        int oneFaceVertexNum = unitNum1*2 + 2 + (height1>0?1:0) + (unitNum2) * 2 + 1;
        int oneFaceIndexNum = (unitNum1+1) * 6 + (height1 > 0 ? 3 : 0) + (unitNum2) * 6 + 3;
        int borderVertexNum = oneFaceIndexNum * 2+ (height1 > 0 ? 2 : 0) + (height2 > 0 ? 2 : 0);
        int borderIndexNum = ((unitNum1 == 0 ? 1 : (unitNum1+1) * 2) + (unitNum2 == 0 ? 1 : (unitNum2+1) * 2))*6;

        vertices = new Vector3[oneFaceVertexNum*2+borderVertexNum];
        indices = new int[oneFaceIndexNum*2+ borderIndexNum];

        float middlePointX = Mathf.Lerp(v1.x, v3.x, height1 / height);
        float leftX = Mathf.Min(v2.x, middlePointX);
        float rightX = Mathf.Max(v2.x, middlePointX);

        float x01 = Mathf.Lerp(v1.x, leftX, unitSize / height1);
        float x02 = Mathf.Lerp(v1.x, rightX, unitSize / height1);

        int startingVIndex = (unitNum1 * 2 + 3);
        int startingIIndex = (unitNum1) * 6 + 6;
        int startingI2Index = (unitNum1) * 6 * 2 + 6;
        int startingBorderIndex = oneFaceVertexNum*2;

        int endingI2Index = oneFaceIndexNum * 2 + borderIndexNum-6;

        if (height1 == 0)
        {
            vertices[0] = new Vector3(leftX, thickness, v1.y);
            vertices[1] = new Vector3(rightX, thickness, v1.y);
            vertices[oneFaceVertexNum] = new Vector3(leftX, 0, v1.y);
            vertices[oneFaceVertexNum + 1] = new Vector3(rightX, 0, v1.y);

            startingVIndex = 2;
            startingIIndex = 0;
            startingI2Index = 0;

            vertices[startingBorderIndex] = vertices[0];
            vertices[startingBorderIndex+1] = vertices[1];
            vertices[startingBorderIndex+ oneFaceVertexNum] = vertices[oneFaceVertexNum];
            vertices[startingBorderIndex+ oneFaceVertexNum + 1] = vertices[oneFaceVertexNum+1];

            indices[endingI2Index + 0] = startingBorderIndex + oneFaceVertexNum;
            indices[endingI2Index + 1] = startingBorderIndex + 0;
            indices[endingI2Index + 2] = startingBorderIndex + 1;
            indices[endingI2Index + 3] = startingBorderIndex + 1;
            indices[endingI2Index + 4] = startingBorderIndex + oneFaceVertexNum+1;
            indices[endingI2Index + 5] = startingBorderIndex + oneFaceVertexNum;
        }
        else
        {
            //three first points and their opposite
            vertices[0] = new Vector3(v1.x, thickness, v1.y);
            vertices[1] = new Vector3(x01, thickness, v1.y + unitSize);
            vertices[2] = new Vector3(x02, thickness, v1.y + unitSize);

            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;

            vertices[oneFaceVertexNum + 0] = new Vector3(v1.x, 0, v1.y);
            vertices[oneFaceVertexNum + 1] = new Vector3(x01, 0, v1.y + unitSize);
            vertices[oneFaceVertexNum + 2] = new Vector3(x02, 0, v1.y + unitSize);

            indices[oneFaceIndexNum + 0] = oneFaceVertexNum + 0;
            indices[oneFaceIndexNum + 1] = oneFaceVertexNum + 2;
            indices[oneFaceIndexNum + 2] = oneFaceVertexNum + 1;

            //the borders of the first three point

            vertices[startingBorderIndex + 0] = vertices[0];
            vertices[startingBorderIndex + 1] = vertices[1];
            vertices[startingBorderIndex + 2] = vertices[2];
            vertices[startingBorderIndex+oneFaceVertexNum + 0] =  vertices[oneFaceVertexNum + 0];
            vertices[startingBorderIndex+oneFaceVertexNum + 1] =  vertices[oneFaceVertexNum + 1];
            vertices[startingBorderIndex+oneFaceVertexNum + 2] =  vertices[oneFaceVertexNum + 2];

            vertices[borderVertexNum - 2] = vertices[0];
            vertices[borderVertexNum - 1] = vertices[oneFaceVertexNum + 0];


            indices[oneFaceIndexNum * 2 + 0] = startingBorderIndex+oneFaceVertexNum + 1;
            indices[oneFaceIndexNum * 2 + 1] = startingBorderIndex+1;
            indices[oneFaceIndexNum * 2 + 2] = startingBorderIndex+0;
            indices[oneFaceIndexNum * 2 + 3] = startingBorderIndex+0;
            indices[oneFaceIndexNum * 2 + 4] = startingBorderIndex+oneFaceVertexNum + 0;
            indices[oneFaceIndexNum * 2 + 5] = startingBorderIndex+oneFaceVertexNum + 1;

            indices[oneFaceIndexNum * 2 + 6 + 0] = borderVertexNum - 1;
            indices[oneFaceIndexNum * 2 + 6 + 1] = borderVertexNum - 2;
            indices[oneFaceIndexNum * 2 + 6 + 2] = startingBorderIndex+2;
            indices[oneFaceIndexNum * 2 + 6 + 3] = startingBorderIndex+2;
            indices[oneFaceIndexNum * 2 + 6 + 4] = startingBorderIndex+oneFaceVertexNum + 2;
            indices[oneFaceIndexNum * 2 + 6 + 5] = borderVertexNum - 1;


            if (height2 == 0)
            {
                indices[endingI2Index + 0] = oneFaceVertexNum + startingVIndex-2;
                indices[endingI2Index+1] = startingVIndex - 1;
                indices[endingI2Index+2] = startingVIndex - 2;
                indices[endingI2Index+3] = startingVIndex - 1;
                indices[endingI2Index+4] = oneFaceVertexNum + startingVIndex - 2;
                indices[endingI2Index+5] = oneFaceVertexNum + startingVIndex - 1;
            }
        }

        if (unitNum1>0)
        {
            for (int i = 1; i < unitNum1; i++)
            {
                float h = unitSize + unitSize * i;
                float x1 = Mathf.Lerp(v1.x, leftX, h / height1);
                float x2 = Mathf.Lerp(v1.x, rightX, h / height1);

                //upface
                vertices[i * 2 + 1] = new Vector3(x1, thickness, v1.y + h);
                vertices[i * 2 + 2] = new Vector3(x2, thickness, v1.y + h);

                indices[i * 6] = i * 2 - 1;
                indices[i * 6 + 1] = i * 2 + 1;
                indices[i * 6 + 2] = i * 2 + 2;

                indices[i * 6 + 3] = i * 2 + 2;
                indices[i * 6 + 4] = i * 2;
                indices[i * 6 + 5] = i * 2 - 1;

                //downface
                vertices[oneFaceVertexNum + i * 2 + 1] = new Vector3(x1, 0, v1.y + h);
                vertices[oneFaceVertexNum + i * 2 + 2] = new Vector3(x2, 0, v1.y + h);

                indices[oneFaceIndexNum + i * 6] = oneFaceVertexNum + i * 2 - 1;
                indices[oneFaceIndexNum + i * 6 + 1] = oneFaceVertexNum + i * 2 + 2;
                indices[oneFaceIndexNum + i * 6 + 2] = oneFaceVertexNum + i * 2 + 1;

                indices[oneFaceIndexNum + i * 6 + 3] = oneFaceVertexNum + i * 2 + 2;
                indices[oneFaceIndexNum + i * 6 + 4] = oneFaceVertexNum + i * 2 - 1;
                indices[oneFaceIndexNum + i * 6 + 5] = oneFaceVertexNum + i * 2;

                //border indices
                vertices[startingBorderIndex+i * 2 + 1]=vertices[i * 2 + 1];
                vertices[startingBorderIndex+i * 2 + 2]=vertices[i * 2 + 2];
                vertices[startingBorderIndex+oneFaceVertexNum + i * 2 + 1] = vertices[oneFaceVertexNum + i * 2 + 1];
                vertices[startingBorderIndex+oneFaceVertexNum + i * 2 + 2] = vertices[oneFaceVertexNum + i * 2 + 2];
                //left border


                indices[oneFaceIndexNum * 2 + i * 6 * 2] = startingBorderIndex+oneFaceVertexNum + i * 2 + 1;
                indices[oneFaceIndexNum * 2 + i * 6 * 2 + 1] = startingBorderIndex+i * 2 + 1;
                indices[oneFaceIndexNum * 2 + i * 6 * 2 + 2] = startingBorderIndex+(i - 1) * 2 + 1;
                indices[oneFaceIndexNum * 2 + i * 6 * 2 + 3] = startingBorderIndex+(i - 1) * 2 + 1;
                indices[oneFaceIndexNum * 2 + i * 6 * 2 + 4] = startingBorderIndex+oneFaceVertexNum + (i - 1) * 2 + 1;
                indices[oneFaceIndexNum * 2 + i * 6 * 2 + 5] = startingBorderIndex+oneFaceVertexNum + i * 2 + 1;

                //right border

                indices[oneFaceIndexNum * 2 + i * 6 * 2 + 6] = startingBorderIndex+oneFaceVertexNum + (i - 1) * 2 + 2;
                indices[oneFaceIndexNum * 2 + i * 6 * 2 + 6 + 1] = startingBorderIndex+(i - 1) * 2 + 2;
                indices[oneFaceIndexNum * 2 + i * 6 * 2 + 6 + 2] = startingBorderIndex+i * 2 + 2;
                indices[oneFaceIndexNum * 2 + i * 6 * 2 + 6 + 3] = startingBorderIndex+i * 2 + 2;
                indices[oneFaceIndexNum * 2 + i * 6 * 2 + 6 + 4] = startingBorderIndex+oneFaceVertexNum + i * 2 + 2;
                indices[oneFaceIndexNum * 2 + i * 6 * 2 + 6 + 5] = startingBorderIndex+oneFaceVertexNum + (i - 1) * 2 + 2;
            }
            //the remaining points
            vertices[unitNum1 * 2 + 1] = new Vector3(leftX, thickness, v2.y);
            vertices[unitNum1 * 2 + 2] = new Vector3(rightX, thickness, v2.y);
            indices[unitNum1 * 6] = unitNum1 * 2 - 1;
            indices[unitNum1 * 6 + 1] = unitNum1 * 2 + 1;
            indices[unitNum1 * 6 + 2] = unitNum1 * 2 + 2;
            indices[unitNum1 * 6 + 3] = unitNum1 * 2 + 2;
            indices[unitNum1 * 6 + 4] = unitNum1 * 2;
            indices[unitNum1 * 6 + 5] = unitNum1 * 2 - 1;

            //opposite point of the remaining points
            vertices[oneFaceVertexNum + unitNum1 * 2 + 1] = new Vector3(leftX, 0, v2.y);
            vertices[oneFaceVertexNum + unitNum1 * 2 + 2] = new Vector3(rightX, 0, v2.y);
            indices[oneFaceIndexNum + unitNum1 * 6] = oneFaceVertexNum + unitNum1 * 2 - 1;
            indices[oneFaceIndexNum + unitNum1 * 6 + 1] = oneFaceVertexNum + unitNum1 * 2 + 2;
            indices[oneFaceIndexNum + unitNum1 * 6 + 2] = oneFaceVertexNum + unitNum1 * 2 + 1;
            indices[oneFaceIndexNum + unitNum1 * 6 + 3] = oneFaceVertexNum + unitNum1 * 2 + 2;
            indices[oneFaceIndexNum + unitNum1 * 6 + 4] = oneFaceVertexNum + unitNum1 * 2 - 1;
            indices[oneFaceIndexNum + unitNum1 * 6 + 5] = oneFaceVertexNum + unitNum1 * 2;

            //borders of the remaining points
            vertices[startingBorderIndex+unitNum1 * 2 + 1] = vertices[unitNum1 * 2 + 1];
            vertices[startingBorderIndex+unitNum1 * 2 + 2] = vertices[unitNum1 * 2 + 2];
            vertices[startingBorderIndex+oneFaceVertexNum + unitNum1 * 2 + 1] = vertices[oneFaceVertexNum + unitNum1 * 2 + 1];
            vertices[startingBorderIndex+oneFaceVertexNum + unitNum1 * 2 + 2]=vertices[oneFaceVertexNum + unitNum1 * 2 + 2] ;
            //left border
            indices[oneFaceIndexNum * 2 + unitNum1 * 6 * 2] = startingBorderIndex+oneFaceVertexNum + unitNum1 * 2 + 1;
            indices[oneFaceIndexNum * 2 + unitNum1 * 6 * 2 + 1] = startingBorderIndex+unitNum1 * 2 + 1;
            indices[oneFaceIndexNum * 2 + unitNum1 * 6 * 2 + 2] = startingBorderIndex+(unitNum1 - 1) * 2 + 1;
            indices[oneFaceIndexNum * 2 + unitNum1 * 6 * 2 + 3] = startingBorderIndex+(unitNum1 - 1) * 2 + 1;
            indices[oneFaceIndexNum * 2 + unitNum1 * 6 * 2 + 4] = startingBorderIndex+oneFaceVertexNum + (unitNum1 - 1) * 2 + 1;
            indices[oneFaceIndexNum * 2 + unitNum1 * 6 * 2 + 5] = startingBorderIndex + oneFaceVertexNum + unitNum1 * 2 + 1;

            //right border

            indices[oneFaceIndexNum * 2 + unitNum1 * 6 * 2 + 6] = startingBorderIndex + oneFaceVertexNum + (unitNum1 - 1) * 2 + 2;
            indices[oneFaceIndexNum * 2 + unitNum1 * 6 * 2 + 6 + 1] = startingBorderIndex+(unitNum1 - 1) * 2 + 2;
            indices[oneFaceIndexNum * 2 + unitNum1 * 6 * 2 + 6 + 2] = startingBorderIndex+unitNum1 * 2 + 2;
            indices[oneFaceIndexNum * 2 + unitNum1 * 6 * 2 + 6 + 3] = startingBorderIndex+unitNum1 * 2 + 2;
            indices[oneFaceIndexNum * 2 + unitNum1 * 6 * 2 + 6 + 4] = startingBorderIndex+oneFaceVertexNum + unitNum1 * 2 + 2;
            indices[oneFaceIndexNum * 2 + unitNum1 * 6 * 2 + 6 + 5] = startingBorderIndex + oneFaceVertexNum + (unitNum1 - 1) * 2 + 2;

        }

        if (unitNum2 > 0)
        {

            for (int i = 0; i < unitNum2 - 1; i++)
            {
                float h = unitSize + unitSize * i;
                float x1 = Mathf.Lerp(leftX, v3.x, h / height2);
                float x2 = Mathf.Lerp(rightX, v3.x, h / height2);

                vertices[startingVIndex + i * 2] = new Vector3(x1, thickness, v2.y + h);
                vertices[startingVIndex + i * 2 + 1] = new Vector3(x2, thickness, v2.y + h);
                indices[startingIIndex + i * 6] = startingVIndex + i * 2 - 2;
                indices[startingIIndex + i * 6 + 1] = startingVIndex + i * 2;
                indices[startingIIndex + i * 6 + 2] = startingVIndex + i * 2 + 1;
                indices[startingIIndex + i * 6 + 3] = startingVIndex + i * 2 + 1;
                indices[startingIIndex + i * 6 + 4] = startingVIndex + i * 2 - 1;
                indices[startingIIndex + i * 6 + 5] = startingVIndex + i * 2 - 2;



                vertices[oneFaceVertexNum + startingVIndex + i * 2] = new Vector3(x1, 0, v2.y + h);
                vertices[oneFaceVertexNum + startingVIndex + i * 2 + 1] = new Vector3(x2, 0, v2.y + h);

                indices[oneFaceIndexNum + startingIIndex + i * 6] = oneFaceVertexNum + startingVIndex + i * 2 - 2;
                indices[oneFaceIndexNum + startingIIndex + i * 6 + 1] = oneFaceVertexNum + startingVIndex + i * 2 + 1;
                indices[oneFaceIndexNum + startingIIndex + i * 6 + 2] = oneFaceVertexNum + startingVIndex + i * 2;

                indices[oneFaceIndexNum + startingIIndex + i * 6 + 3] = oneFaceVertexNum + startingVIndex + i * 2 + 1;
                indices[oneFaceIndexNum + startingIIndex + i * 6 + 4] = oneFaceVertexNum + startingVIndex + i * 2 - 2;
                indices[oneFaceIndexNum + startingIIndex + i * 6 + 5] = oneFaceVertexNum + startingVIndex + i * 2 - 1;


                //border indices

                vertices[startingBorderIndex+startingVIndex + i * 2] = vertices[startingVIndex + i * 2];
                vertices[startingBorderIndex+startingVIndex + i * 2 + 1] = vertices[startingVIndex + i * 2 + 1];
                vertices[startingBorderIndex+oneFaceVertexNum + startingVIndex + i * 2] = vertices[oneFaceVertexNum + startingVIndex + i * 2];
                vertices[startingBorderIndex+oneFaceVertexNum + startingVIndex + i * 2 + 1] = vertices[oneFaceVertexNum + startingVIndex + i * 2 + 1];

                //left border
                indices[oneFaceIndexNum * 2 + startingI2Index + i * 6 * 2 + 6] = startingBorderIndex + oneFaceVertexNum + startingVIndex + i * 2;
                indices[oneFaceIndexNum * 2 + startingI2Index + i * 6 * 2 + 6 + 1] = startingBorderIndex+startingVIndex + i * 2;
                indices[oneFaceIndexNum * 2 + startingI2Index + i * 6 * 2 + 6 + 2] = startingBorderIndex+startingVIndex + (i - 1) * 2;
                indices[oneFaceIndexNum * 2 + startingI2Index + i * 6 * 2 + 6 + 3] = startingBorderIndex+startingVIndex + (i - 1) * 2;
                indices[oneFaceIndexNum * 2 + startingI2Index + i * 6 * 2 + 6 + 4] = startingBorderIndex+startingVIndex + oneFaceVertexNum + (i - 1) * 2;
                indices[oneFaceIndexNum * 2 + startingI2Index + i * 6 * 2 + 6 + 5] = startingBorderIndex + startingVIndex + oneFaceVertexNum + i * 2;

                //right border
                indices[oneFaceIndexNum * 2 + startingI2Index + i * 6 * 2] = startingBorderIndex + oneFaceVertexNum + startingVIndex + (i - 1) * 2 + 1;
                indices[oneFaceIndexNum * 2 + startingI2Index + i * 6 * 2 + 1] = startingBorderIndex+startingVIndex + (i - 1) * 2 + 1;
                indices[oneFaceIndexNum * 2 + startingI2Index + i * 6 * 2 + 2] = startingBorderIndex+startingVIndex + i * 2 + 1;
                indices[oneFaceIndexNum * 2 + startingI2Index + i * 6 * 2 + 3] = startingBorderIndex+startingVIndex + i * 2 + 1;
                indices[oneFaceIndexNum * 2 + startingI2Index + i * 6 * 2 + 4] = startingBorderIndex+startingVIndex + oneFaceVertexNum + i * 2 + 1;
                indices[oneFaceIndexNum * 2 + startingI2Index + i * 6 * 2 + 5] = startingBorderIndex + startingVIndex + oneFaceVertexNum + (i - 1) * 2 + 1;

            }
            if (height2 > 0)
            {
                int lastIndex = (unitNum2 - 1);
                vertices[startingVIndex + lastIndex * 2] = new Vector3(v3.x, thickness, v3.y);
                indices[startingIIndex + lastIndex * 6] = startingVIndex + lastIndex * 2 - 2;
                indices[startingIIndex + lastIndex * 6 + 1] = startingVIndex + lastIndex * 2;
                indices[startingIIndex + lastIndex * 6 + 2] = startingVIndex + lastIndex * 2 - 1;

                vertices[oneFaceVertexNum + startingVIndex + lastIndex * 2] = new Vector3(v3.x, 0, v3.y);
                indices[oneFaceIndexNum + startingIIndex + lastIndex * 6] = oneFaceVertexNum + startingVIndex + lastIndex * 2 - 2;
                indices[oneFaceIndexNum + startingIIndex + lastIndex * 6 + 1] = oneFaceVertexNum + startingVIndex + lastIndex * 2 - 1;
                indices[oneFaceIndexNum + startingIIndex + lastIndex * 6 + 2] = oneFaceVertexNum + startingVIndex + lastIndex * 2;

                vertices[startingBorderIndex + startingVIndex + lastIndex * 2] = vertices[startingVIndex + lastIndex * 2];
                vertices[startingBorderIndex + oneFaceVertexNum + startingVIndex + lastIndex * 2] = vertices[oneFaceVertexNum + startingVIndex + lastIndex * 2];

                int last2BorderVerticesStartingIndex = borderVertexNum-(height1 > 0 ? 4 : 2);
                vertices[last2BorderVerticesStartingIndex] = vertices[startingVIndex + lastIndex * 2];
                vertices[last2BorderVerticesStartingIndex+1] = vertices[oneFaceVertexNum + startingVIndex + lastIndex * 2];

                indices[oneFaceIndexNum * 2 + startingI2Index + lastIndex * 6 * 2] = startingBorderIndex + oneFaceVertexNum + startingVIndex + lastIndex * 2;
                indices[oneFaceIndexNum * 2 + startingI2Index + lastIndex * 6 * 2 + 1] = startingBorderIndex +startingVIndex + lastIndex * 2;
                indices[oneFaceIndexNum * 2 + startingI2Index + lastIndex * 6 * 2 + 2] = startingBorderIndex +startingVIndex + (lastIndex - 1) * 2;
                indices[oneFaceIndexNum * 2 + startingI2Index + lastIndex * 6 * 2 + 3] = startingBorderIndex +startingVIndex + (lastIndex - 1) * 2;
                indices[oneFaceIndexNum * 2 + startingI2Index + lastIndex * 6 * 2 + 4] = startingBorderIndex +startingVIndex + oneFaceVertexNum + (lastIndex - 1) * 2;
                indices[oneFaceIndexNum * 2 + startingI2Index + lastIndex * 6 * 2 + 5] = startingBorderIndex + startingVIndex + oneFaceVertexNum + lastIndex * 2;

                indices[oneFaceIndexNum * 2 + startingI2Index + lastIndex * 6 * 2 + 6] = startingBorderIndex + oneFaceVertexNum + startingVIndex + (lastIndex - 1) * 2 + 1;
                indices[oneFaceIndexNum * 2 + startingI2Index + lastIndex * 6 * 2 + 6 + 1] = startingBorderIndex +startingVIndex + (lastIndex - 1) * 2 + 1;
                indices[oneFaceIndexNum * 2 + startingI2Index + lastIndex * 6 * 2 + 6 + 2] = last2BorderVerticesStartingIndex;
                indices[oneFaceIndexNum * 2 + startingI2Index + lastIndex * 6 * 2 + 6 + 3] = last2BorderVerticesStartingIndex;
                indices[oneFaceIndexNum * 2 + startingI2Index + lastIndex * 6 * 2 + 6 + 4] = last2BorderVerticesStartingIndex+1;
                indices[oneFaceIndexNum * 2 + startingI2Index + lastIndex * 6 * 2 + 6 + 5] = startingBorderIndex + startingVIndex + oneFaceVertexNum + (lastIndex - 1) * 2 + 1;
            }
        }
        else
        {
            if (height2 > 0)
            {
                vertices[startingVIndex] = new Vector3(v3.x, thickness, v3.y);
                indices[startingIIndex] = startingVIndex - 2;
                indices[startingIIndex+1] = startingVIndex ;
                indices[startingIIndex+2] = startingVIndex - 1;

                vertices[oneFaceVertexNum+startingVIndex] = new Vector3(v3.x, 0, v3.y);
                indices[oneFaceIndexNum+startingIIndex] = oneFaceVertexNum+startingVIndex - 2;
                indices[oneFaceIndexNum+startingIIndex + 1] = oneFaceVertexNum+startingVIndex;
                indices[oneFaceIndexNum+startingIIndex + 2] = oneFaceVertexNum+startingVIndex - 1;


                vertices[startingBorderIndex+startingVIndex] = vertices[startingVIndex];
                vertices[startingBorderIndex+oneFaceVertexNum + startingVIndex] = vertices[oneFaceVertexNum + startingVIndex];
                indices[oneFaceIndexNum * 2 + startingI2Index] = startingBorderIndex + oneFaceVertexNum + startingVIndex -1;
                indices[oneFaceIndexNum * 2 + startingI2Index +1] = startingBorderIndex+startingVIndex - 1;
                indices[oneFaceIndexNum * 2 + startingI2Index +2] = startingBorderIndex+startingVIndex;
                indices[oneFaceIndexNum * 2 + startingI2Index +3] = startingBorderIndex+startingVIndex;
                indices[oneFaceIndexNum * 2 + startingI2Index +4] = startingBorderIndex+startingVIndex + oneFaceVertexNum;
                indices[oneFaceIndexNum * 2 + startingI2Index +5] = startingBorderIndex + startingVIndex + oneFaceVertexNum-1;

                indices[oneFaceIndexNum * 2 + startingI2Index +6] = startingBorderIndex + oneFaceVertexNum + startingVIndex;
                indices[oneFaceIndexNum * 2 + startingI2Index +6 + 1] = startingBorderIndex+startingVIndex;
                indices[oneFaceIndexNum * 2 + startingI2Index +6 + 2] = startingBorderIndex+startingVIndex - 2;
                indices[oneFaceIndexNum * 2 + startingI2Index +6 + 3] = startingBorderIndex+startingVIndex - 2;
                indices[oneFaceIndexNum * 2 + startingI2Index +6 + 4] = startingBorderIndex+startingVIndex + oneFaceVertexNum -2;
                indices[oneFaceIndexNum * 2 + startingI2Index +6 + 5] = startingBorderIndex + startingVIndex + oneFaceVertexNum;

            }
        }

    }


    //public void GeneratePolygon(Vector2 [] polygon, float unitSize, out Vector2[] vertices, out int[] indices)
    //{

    //}
    private bool lineIntersect(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersectingPoint)
    {

        float s1_x, s1_y, s2_x, s2_y;
        s1_x = a2.x - a1.x;
        s1_y = a2.y - a1.y;

        s2_x = b2.x - b1.x;
        s2_y = b2.y - b1.y;

        //b1.x -= s2_x*10f;
        //b1.y -= s2_y*10f;

        float s, t;

        s = (-s1_y * (a1.x - b1.x) + s1_x * (a1.y - b1.y)) / (-s2_x * s1_y + s1_x * s2_y);
        t = (s2_x * (a1.y - b1.y) + s2_y * (a1.x - b1.x)) / (-s2_x * s1_y + s1_x * s2_y);

        if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
        {
            //collision detected
            intersectingPoint = new Vector2(a1.x + (t * s1_x), a1.y + (t * s1_y));
            return true;
        }
        intersectingPoint = new Vector2();
        return false;
    }
}
