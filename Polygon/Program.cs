using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Polygon
{
    public class Geometry
    {
        public string type { get; set; }
        public List<List<List<double>>> coordinates { get; set; }
    }

    public class Root
    {
        public Geometry geometry { get; set; }
    }

    class Point
    {
        public Point()
        {
            this.x = 0;
            this.y = 0;
        }
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public double x { get; set; }
        public double y { get; set; }

    }

    class YaxisIntersectionLine
    {
        public YaxisIntersectionLine()
        {
            startX = 0;
            endX = 0;
            y = 0;
        }
        public double startX { get; set; }
        public double endX { get; set; }
        public double y { get; set; }



    }
    class Program
    {
        static void Main(string[] args)
        {
            List<Root> items = new List<Root>();
            using (System.IO.StreamReader r = new System.IO.StreamReader(@"C:\Users\hazem\OneDrive\Desktop\Polygons.json"))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<Root>>(json);
            }

            
            for (int i = 0; i < items.Count; i++)
            {
                List<Point> Polygon = new List<Point>();
                for (int j = 0; j < items[i].geometry.coordinates.Count; j++)
                {
                    for (int k = 0; k < items[i].geometry.coordinates[j].Count; k++)
                    {
                        var x = items[i].geometry.coordinates[j][k][0];
                        var y = items[i].geometry.coordinates[j][k][1];
                        Polygon.Add(new Point(x, y));
                    }
                }


                //Polygon.Add(new Point(1, 2));
                //Polygon.Add(new Point(1, 5));
                //Polygon.Add(new Point(4.5, 5));
                //Polygon.Add(new Point(7, 2.5));
                //Polygon.Add(new Point(5, 0));
                //Polygon.Add(new Point(3, 2));
                //Polygon.Add(Polygon[0]); //closed Polygon

                double maxY = Polygon.Max(e => e.y);
                double minY = Polygon.Min(e => e.y);

                double step = GetBestStepValue(ref Polygon);

                YaxisIntersectionLine bestIntersection = new YaxisIntersectionLine();


                for (double d = minY + step; d < maxY; d = d + step)
                {
                    var intersection = GetBestIntersectionByYaxis(ref Polygon, d);
                    if (Math.Abs(bestIntersection.startX - bestIntersection.endX) < Math.Abs(intersection.startX - intersection.endX))
                    {
                        bestIntersection = intersection;
                    }
                }

                Console.WriteLine("(" + bestIntersection.startX + "," + bestIntersection.y + ") (" + bestIntersection.endX + "," + bestIntersection.y + ")");
            }
        }

        static double XonYIntersect(Point p1, Point p2, double y)
        {
            double ydiff1 = p1.y - y;
            double ydiff2 = p2.y - y;

            if ((ydiff1 > 0 && ydiff2 > 0) || (ydiff1 < 0 && ydiff2 < 0))
            {
                return Double.PositiveInfinity;
            }

            if (p1.x == p2.x)
            {
                return p1.x;
            }

            if (p1.y == p2.y)
            {
                return Double.PositiveInfinity;
            }



            double ratio = Math.Abs(p2.y - y) / Math.Abs(p2.y - p1.y);
            return p2.x - ((p2.x - p1.x) * ratio);
        }
        static YaxisIntersectionLine GetBestIntersectionByYaxis(ref List<Point> Polygon, double y)
        {
            YaxisIntersectionLine yiMax = new YaxisIntersectionLine();

            List<double> intersectionXs = new List<double>();
            for (int i = 0; i < Polygon.Count - 1; i++)
            {
                var p1 = Polygon[i];
                var p2 = Polygon[i + 1];
                double xIntersction = XonYIntersect(p1, p2, y);
                if (!Double.IsInfinity(xIntersction))
                {
                    intersectionXs.Add(xIntersction);
                }
            }

            if (intersectionXs.Count > 1)
            {
                for (int i = 0; i < intersectionXs.Count - 1; i++)
                {
                    if (i % 2 == 0)
                    {
                        YaxisIntersectionLine yi = new YaxisIntersectionLine();
                        if (intersectionXs[i] > intersectionXs[i + 1])
                        {
                            yi.startX = intersectionXs[i];
                            yi.endX = intersectionXs[i + 1];
                            yi.y = y;
                        }
                        else
                        {
                            yi.startX = intersectionXs[i + 1];
                            yi.endX = intersectionXs[i];
                            yi.y = y;
                        }

                        if (Math.Abs(yiMax.startX - yiMax.endX) < Math.Abs(yi.startX - yi.endX))
                        {
                            yiMax = yi;
                        }
                    }
                }
            }

            return yiMax;
        }

        static double GetBestStepValue(ref List<Point> Polygon)
        {
            double minYDiffrence = Double.PositiveInfinity;
            for (int i = 0; i < Polygon.Count - 1; i++)
            {
                var d = Math.Abs(Polygon[i].y - Polygon[i + 1].y);
                if (d > 0 && d < minYDiffrence)
                    minYDiffrence = d;
            }

            return minYDiffrence / 10;
        }
    }
}
