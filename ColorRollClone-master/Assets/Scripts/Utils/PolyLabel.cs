using System;
using PriorityQueue;
using UnityEngine;

namespace SkiaDemo1
{
    public class PolyLabel
    {
        private const float EPSILON = 1E-8f;



        //this function supports for single ring polygon

        public static Vector2 GetPolyLabel(Vector2[] polygon, float precision = 1f)
        {
            //Find the bounding box of the outer ring
            float minX = 0, minY = 0, maxX = 0, maxY = 0;
            for (int i = 0; i < polygon.Length; i++)
            {
                var p = polygon[i];
                if (i == 0 || p.x < minX)
                    minX = p.x;
                if (i == 0 || p.y < minY)
                    minY = p.y;
                if (i == 0 || p.x > maxX)
                    maxX = p.x;
                if (i == 0 || p.y > maxY)
                    maxY = p.y;
            }

            float width = maxX - minX;
            float height = maxY - minY;
            float cellSize = Math.Min(width, height);
            float h = cellSize / 2;

            //A priority queue of cells in order of their "potential" (max distance to polygon)
            PriorityQueue<float, Cell2> cellQueue = new PriorityQueue<float, Cell2>();

            if (FloatEquals(cellSize, 0))
                return new Vector2( minX, minY );

            //Cover polygon with initial cells
            for (float x = minX; x < maxX; x += cellSize)
            {
                for (float y = minY; y < maxY; y += cellSize)
                {
                    Cell2 cell = new Cell2(x + h, y + h, h, polygon);
                    cellQueue.Enqueue(cell.Max, cell);
                }
            }
            //Take centroid as the first best guess
            Cell2 bestCell = GetCentroidCell2(polygon);

            //Special case for rectangular polygons
            var bboxCell = new Cell2(minX + width / 2, minY + height / 2, 0, polygon);
            if (bboxCell.D > bestCell.D)
                bestCell = bboxCell;

            int numProbes = cellQueue.Count;

            while (cellQueue.Count > 0)
            {
                //Pick the most promising cell from the queue
                var cell = cellQueue.Dequeue();

                //Update the best cell if we found a better one
                if (cell.D > bestCell.D)
                {
                    bestCell = cell;
                }

                //Do not drill down further if there's no chance of a better solution
                if (cell.Max - bestCell.D <= precision)
                    continue;

                //Split the cell into four cells
                h = cell.H / 2;
                var cell1 = new Cell2(cell.X - h, cell.Y - h, h, polygon);
                cellQueue.Enqueue(cell1.Max, cell1);
                var cell2 = new Cell2(cell.X + h, cell.Y - h, h, polygon);
                cellQueue.Enqueue(cell2.Max, cell2);
                var cell3 = new Cell2(cell.X - h, cell.Y + h, h, polygon);
                cellQueue.Enqueue(cell3.Max, cell3);
                var cell4 = new Cell2(cell.X + h, cell.Y + h, h, polygon);
                cellQueue.Enqueue(cell4.Max, cell4);
                numProbes += 4;
            }

            return new Vector2(bestCell.X, bestCell.Y );

        }
        private class Cell2
        {
            public float X { get; private set; }
            public float Y { get; private set; }
            public float H { get; private set; }
            public float D { get; private set; }
            public float Max { get; private set; }

            public Cell2(float x, float y, float h, Vector2[] polygon)
            {
                X = x;
                Y = y;
                H = h;
                D = PointToPolygonDist(X, Y, polygon);
                Max = D + H * (float)Math.Sqrt(2);
            }
        }
        //Signed distance from point to polygon outline (negative if point is outside)
        private static float PointToPolygonDist(float x, float y, Vector2[] polygon)
        {
            bool inside = false;
            float minDistSq = float.PositiveInfinity;


            for (int i = 0, len = polygon.Length, j = len - 1; i < len; j = i++)
            {
                var a = polygon[i];
                var b = polygon[j];

                if ((a.y > y != b.y > y) && (x < (b.x- a.x) * (y - a.y) / (b.y- a.y) + a.x))
                    inside = !inside;

                minDistSq = Math.Min(minDistSq, GetSegDistSq(x, y, a, b));
            }

            return ((inside ? 1 : -1) * (float)Math.Sqrt(minDistSq));
        }

        //Get squared distance from a point to a segment
        private static float GetSegDistSq(float px, float py,Vector2 a,Vector2 b)
        {
            float x = a.x;
            float y = a.y;
            float dx = b.x- x;
            float dy = b.y - y;

            if (!FloatEquals(dx, 0) || !FloatEquals(dy, 0))
            {
                float t = ((px - x) * dx + (py - y) * dy) / (dx * dx + dy * dy);
                if (t > 1)
                {
                    x = b.x;
                    y = b.y;
                }
                else if (t > 0)
                {
                    x += dx * t;
                    y += dy * t;
                }
            }
            dx = px - x;
            dy = py - y;
            return (dx * dx + dy * dy);
        }

        //Get polygon centroid
        private static Cell2 GetCentroidCell2(Vector2[] polygon)
        {
            float area = 0;
            float x = 0;
            float y = 0;
            var ring = polygon;

            for (int i = 0, len = ring.Length, j = len - 1; i < len; j = i++)
            {
                var a = ring[i];
                var b = ring[j];
                float f = a.x * b.y - b.x * a.y;
                x += (a.x + b.x) * f;
                y += (a.y + b.y) * f;
                area += f * 3;
            }
            if (FloatEquals(area, 0))
                return (new Cell2(ring[0].x, ring[0].y, 0, polygon));
            return (new Cell2(x / area, y / area, 0, polygon));
        }




        //End of my modification





        public static float[] GetPolyLabel(float[][][] polygon, float precision = 1f)
        {
            //Find the bounding box of the outer ring
            float minX = 0, minY = 0, maxX = 0, maxY = 0;
            for (int i = 0; i < polygon[0].Length; i++)
            {
                float[] p = polygon[0][i];
                if (i == 0 || p[0] < minX)
                    minX = p[0];
                if (i == 0 || p[1] < minY)
                    minY = p[1];
                if (i == 0 || p[0] > maxX)
                    maxX = p[0];
                if (i == 0 || p[1] > maxY)
                    maxY = p[1];
            }

            float width = maxX - minX;
            float height = maxY - minY;
            float cellSize = Math.Min(width, height);
            float h = cellSize / 2;

            //A priority queue of cells in order of their "potential" (max distance to polygon)
            PriorityQueue<float, Cell> cellQueue = new PriorityQueue<float, Cell>();

            if (FloatEquals(cellSize, 0))
                return new[] { minX, minY };

            //Cover polygon with initial cells
            for (float x = minX; x < maxX; x += cellSize)
            {
                for (float y = minY; y < maxY; y += cellSize)
                {
                    Cell cell = new Cell(x + h, y + h, h, polygon);
                    cellQueue.Enqueue(cell.Max, cell);
                }
            }

            //Take centroid as the first best guess
            Cell bestCell = GetCentroidCell(polygon);

            //Special case for rectangular polygons
            Cell bboxCell = new Cell(minX + width / 2, minY + height / 2, 0, polygon);
            if (bboxCell.D > bestCell.D)
                bestCell = bboxCell;

            int numProbes = cellQueue.Count;

            while (cellQueue.Count > 0)
            {
                //Pick the most promising cell from the queue
                Cell cell = cellQueue.Dequeue();

                //Update the best cell if we found a better one
                if (cell.D > bestCell.D)
                {
                    bestCell = cell;
                }

                //Do not drill down further if there's no chance of a better solution
                if (cell.Max - bestCell.D <= precision)
                    continue;

                //Split the cell into four cells
                h = cell.H / 2;
                Cell cell1 = new Cell(cell.X - h, cell.Y - h, h, polygon);
                cellQueue.Enqueue(cell1.Max, cell1);
                Cell cell2 = new Cell(cell.X + h, cell.Y - h, h, polygon);
                cellQueue.Enqueue(cell2.Max, cell2);
                Cell cell3 = new Cell(cell.X - h, cell.Y + h, h, polygon);
                cellQueue.Enqueue(cell3.Max, cell3);
                Cell cell4 = new Cell(cell.X + h, cell.Y + h, h, polygon);
                cellQueue.Enqueue(cell4.Max, cell4);
                numProbes += 4;
            }

            return (new[] { bestCell.X, bestCell.Y });
        }

        //Signed distance from point to polygon outline (negative if point is outside)
        private static float PointToPolygonDist(float x, float y, float[][][] polygon)
        {
            bool inside = false;
            float minDistSq = float.PositiveInfinity;

            for (int k = 0; k < polygon.Length; k++)
            {
                float[][] ring = polygon[k];

                for (int i = 0, len = ring.Length, j = len - 1; i < len; j = i++)
                {
                    float[] a = ring[i];
                    float[] b = ring[j];

                    if ((a[1] > y != b[1] > y) && (x < (b[0] - a[0]) * (y - a[1]) / (b[1] - a[1]) + a[0]))
                        inside = !inside;

                    minDistSq = Math.Min(minDistSq, GetSegDistSq(x, y, a, b));
                }
            }

            return ((inside ? 1 : -1) * (float)Math.Sqrt(minDistSq));
        }

        //Get squared distance from a point to a segment
        private static float GetSegDistSq(float px, float py, float[] a, float[] b)
        {
            float x = a[0];
            float y = a[1];
            float dx = b[0] - x;
            float dy = b[1] - y;

            if (!FloatEquals(dx, 0) || !FloatEquals(dy, 0))
            {
                float t = ((px - x) * dx + (py - y) * dy) / (dx * dx + dy * dy);
                if (t > 1)
                {
                    x = b[0];
                    y = b[1];
                }
                else if (t > 0)
                {
                    x += dx * t;
                    y += dy * t;
                }
            }
            dx = px - x;
            dy = py - y;
            return (dx * dx + dy * dy);
        }

        //Get polygon centroid
        private static Cell GetCentroidCell(float[][][] polygon)
        {
            float area = 0;
            float x = 0;
            float y = 0;
            float[][] ring = polygon[0];

            for (int i = 0, len = ring.Length, j = len - 1; i < len; j = i++)
            {
                float[] a = ring[i];
                float[] b = ring[j];
                float f = a[0] * b[1] - b[0] * a[1];
                x += (a[0] + b[0]) * f;
                y += (a[1] + b[1]) * f;
                area += f * 3;
            }
            if (FloatEquals(area, 0))
                return (new Cell(ring[0][0], ring[0][1], 0, polygon));
            return (new Cell(x / area, y / area, 0, polygon));
        }


        private static bool FloatEquals(float a, float b)
        {
            return (Math.Abs(a - b) < EPSILON);
        }

        private class Cell
        {
            public float X { get; private set; }
            public float Y { get; private set; }
            public float H { get; private set; }
            public float D { get; private set; }
            public float Max { get; private set; }

            public Cell(float x, float y, float h, float[][][] polygon)
            {
                X = x;
                Y = y;
                H = h;
                D = PointToPolygonDist(X, Y, polygon);
                Max = D + H * (float)Math.Sqrt(2);
            }
        }

    }
}