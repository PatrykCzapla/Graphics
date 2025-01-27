﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    public static class Tools
    {
        public static double distance(Vertex v1, Vertex v2)
        {
            return Math.Sqrt(Math.Pow(v2.x - v1.x, 2) + Math.Pow(v2.y - v1.y, 2));
        }

        public static int cross(Vector a, Vector b, Vector c)
        {
            double value = (b.values[0] - a.values[0]) * (c.values[1] - a.values[1]) - (b.values[1] - a.values[1]) * (c.values[0] - a.values[0]);
            return Math.Abs(value) < 1e-10 ? 0 : value < 0 ? -1 : 1;
        }

        public static double triangleArea(Vertex v1, Vertex v2, Vertex v3)
        {
            double a = distance(v1, v2);
            double b = distance(v1, v3);
            double c = distance(v2, v3);
            double p = 0.5 * (a + b + c);
            return 0.5 * Math.Sqrt(p * (p - a) * (p - b) * (p - c));
        }
    }
}