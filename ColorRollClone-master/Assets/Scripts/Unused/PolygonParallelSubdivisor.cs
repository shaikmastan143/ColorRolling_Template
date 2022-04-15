using System.Collections.Generic;
using UnityEngine;

public class PolygonParallelSubdivisor
{
    public Vector2[] Subdivide(Vector2[] polygon,float size, out int [] indices)
    {
        List<Vector2> left = new List<Vector2>(); 
        List<Vector2> right = new List<Vector2>();

        List<Vector2> outputPolygon = new List<Vector2>();
        List<List<Vector2>> subpolygons = new List<List<Vector2>>();


        float upperBound = float.MinValue;
        float lowerBound = float.MaxValue;
        float rightBound = float.MinValue;
        float leftBound = float.MaxValue;

        for (int i = 1; i < polygon.Length; i++)
        {
            if (polygon[i].y > upperBound)
                upperBound = polygon[i].y;
            if (polygon[i].y < lowerBound)
                lowerBound = polygon[i].y;

            if (polygon[i].x > rightBound)
                rightBound = polygon[i].x;
            if (polygon[i].x < leftBound)
                leftBound = polygon[i].x;
        }


        Vector2 linePointA = new Vector2(leftBound* 2,lowerBound+size);
        Vector2 linePointB = new Vector2(rightBound* 2, lowerBound + size);

        while (linePointA.y < upperBound)
        {
            outputPolygon.Clear();
            for (int i = 0; i < polygon.Length-1; i++)
            {
                Vector2 a= polygon[i];
                Vector2 b = polygon[i == polygon.Length - 1 ? 0 : i + 1];

                if(a.y<linePointA.y&&a.y>linePointA.y-size)
                    outputPolygon.Add(a);
                if (b.y < linePointA.y&&b.y>linePointA.y-size)
                    outputPolygon.Add(b);

                if (lineIntersect(a, b, linePointA, linePointB, out Vector2 intersectingPoint))
                {
                    outputPolygon.Add(intersectingPoint);
                }
            }
            //outputPolygon.Add(polygon[polygon.Length-1]);
            //polygon = outputPolygon.ToArray();

            linePointA.y += size;
            linePointB.y += size;
        }





        Vector2[] vertices = outputPolygon.ToArray();
        Triangulator triangulator = new Triangulator(vertices);
        indices = triangulator.Triangulate();
        return vertices;
    }

    private bool lineIntersect(Vector2 a1,Vector2 a2, Vector2 b1, Vector2 b2,out Vector2 intersectingPoint)
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
            intersectingPoint = new Vector2(a1.x + (t * s1_x),a1.y + (t * s1_y));
            return true;
        }
        intersectingPoint = new Vector2();
        return false;
    }

    //private bool lineIntersect(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersectingPoint)
    //{

    //    float s1_x, s1_y, s2_x, s2_y;
    //    s1_x = a2.x - a1.x;
    //    s1_y = a2.y - a1.y;

    //    s2_x = b2.x - b1.x;
    //    s2_y = b2.y - b1.y;

    //    float s, t;

    //    s = (-s1_y * (a1.x - b1.x) + s1_x * (a1.y - b1.y)) / (-s2_x * s1_y + s1_x * s2_y);
    //    t = (s1_x * (a1.y - b1.y) + s2_y * (a1.x - b1.x)) / (-s2_x * s1_y + s1_x * s2_y);

    //    if (s > -0 && s <= 1 && t >= 0 && t <= 1)
    //    {
    //        //collision detected
    //        intersectingPoint = new Vector2(a1.x + (t * s1_x), a1.y + (t * s1_y));
    //        return true;
    //    }
    //    intersectingPoint = new Vector2();
    //    return false;
    //}
}
