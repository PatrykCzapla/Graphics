﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polygon_Filler
{
    public static class Tools
    {
        public static float scalarProduct(float[] v, float[] u)
        {
            if (v.Count() != 3 || u.Count() != 3) return 0;
            return v[0] * u[0] + v[1] * u[1] + v[2] * u[2];
        }

        public static double distance(Vertex v1, Vertex v2)
        {
            return Math.Sqrt(Math.Pow(v2.center.X - v1.center.X, 2) + Math.Pow(v2.center.Y - v1.center.Y, 2));
        }

        public static int cross(Vertex o, Vertex v, Vertex u)
        {
            double value = (v.center.X - o.center.X) * (u.center.Y - o.center.Y) - (v.center.Y - o.center.Y) * (u.center.X - o.center.X);
            return Math.Abs(value) < 1e-1 ? 0 : value < 0 ? -1 : 1;
        }

        public static bool linesIntersect(Point p1, Point p2, Point p3, Point p4)
        {
            if (p1 == p4 || p2 == p3) return false;
            if (p1 == p3 || p2 == p4) return false;

            int d1 = orientation(p1, p2, p3);
            int d2 = orientation(p1, p2, p4);
            int d3 = orientation(p3, p4, p1);
            int d4 = orientation(p3, p4, p2);

            if (d1 != d2 && d3 != d4)
                return true;

            if (d1 == 0 && onLine(p1, p3, p2)) return true;
            if (d2 == 0 && onLine(p1, p4, p2)) return true;
            if (d3 == 0 && onLine(p3, p1, p4)) return true;
            if (d4 == 0 && onLine(p3, p2, p4)) return true;

            return false;
        }

        public static int orientation(Point p1, Point p2, Point q)
        {
            int val = (p2.Y - p1.Y) * (q.X - p2.X) - (p2.X - p1.X) * (q.Y - p2.Y);

            if (val == 0) return 0;
            return (val > 0) ? 1 : 2;
        }

        public static bool onLine(Point p, Point q, Point r)
        {
            if (q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) && q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y)) return true;
            return false;
        }

        public static Vertex searchForVertex(Point p)
        {
            for (int i = -6; i < 7; i++)
                for (int j = -6; j < 7; j++)
                {
                    if (p.X + i < 0 || p.X + i >= Form.dbm.Width || p.Y + j < 0 || p.Y + j >= Form.dbm.Height) continue;

                    if (p.X + i == Form.icon.center.X && p.Y + j == Form.icon.center.Y) return Form.icon;

                }
            for (int i = -2; i < 3; i++)
                for (int j = -2; j < 3; j++)
                {
                    foreach(Polygon polygon in Form.polygons)
                        foreach(Vertex v in polygon.vertices)
                        {
                            if (p.X + i >= Form.dbm.Width || p.X + i < 0 || p.Y + j >= Form.dbm.Height || p.Y + j < 0) continue;
                            if (p.X + i == v.center.X && p.Y + j == v.center.Y) return v;
                        }
                }
            return null;
        }

        public static Polygon searchForPolygon(Point p)
        {
            foreach (Polygon polygon in Form.polygons)
                if (isInside(p, polygon) == true) return polygon;
            foreach (ConvexPolygon polygon in Form.convexPolygons)
                if (isInside(p, polygon) == true) return polygon;
            return null;
        }

        public static bool isInside(Point v, Polygon polygon)
        {
            int yMin = int.MaxValue;
            int yMax = 0;
            int xMin = int.MaxValue;
            int xMax = 0;
            foreach (Vertex vertex in polygon.vertices)
            {
                if (vertex.center.Y < yMin) yMin = vertex.center.Y;
                if (vertex.center.Y > yMax) yMax = vertex.center.Y;
                if (vertex.center.X < xMin) xMin = vertex.center.X;
                if (vertex.center.X > xMax) xMax = vertex.center.X;
            }

            if (v.Y > yMax || v.Y < yMin || v.X > xMax || v.X < xMin) return false;

            Point extreme = new Point(Form.dbm.Width - 1, v.Y);

            int count = 0, i = 0;

            do
            {
                int next = (i + 1) % polygon.vertices.Count;

                Vertex vert = null;
                if (polygon.vertices[i].center.Y == v.Y) vert = polygon.vertices[i];
                if (polygon.vertices[next].center.Y == v.Y) vert = polygon.vertices[next];
                if (vert != null)
                {
                    Edge e1 = polygon.edges.Find(a => a.v1 == vert);
                    Edge e2 = polygon.edges.Find(a => a.v2 == vert);
                    if (e1.v2.center.Y >= v.Y && e2.v1.center.Y >= v.Y || e1.v2.center.Y <= v.Y && e2.v1.center.Y <= v.Y) count += 2;
                    else if (e1.v2.center.Y > v.Y && e2.v1.center.Y < v.Y || e1.v2.center.Y < v.Y && e2.v1.center.Y > v.Y) count++;

                    i = next;
                    continue;
                }

                if (linesIntersect(polygon.vertices[i].center, polygon.vertices[next].center, v, extreme))
                {
                    if (orientation(polygon.vertices[i].center, v, polygon.vertices[next].center) == 0)
                        return onLine(polygon.vertices[i].center, v, polygon.vertices[next].center);

                    count++;
                }
                i = next;
            } while (i != 0);
            return count % 2 == 1 ? true : false;
        }

        //-----WEILER - ATHERTON-----//
        public static (Vertex, Vertex)? getIntersectionPoints(Vertex s1, Vertex s2, Vertex c1, Vertex c2)
        {
            if (s1 == c1 || s1 == c2 || s2 == c1 || s2 == c2) return null;
            double d = (c2.center.Y - c1.center.Y) * (s2.center.X - s1.center.X) - (c2.center.X - c1.center.X) * (s2.center.Y - s1.center.Y);
            if (d == 0.0) return null;
            double toSource = (double)((double)((c2.center.X - c1.center.X) * (s1.center.Y - c1.center.Y) - (c2.center.Y - c1.center.Y) * (s1.center.X - c1.center.X))) / d;
            double toClip = (double)((double)((s2.center.X - s1.center.X) * (s1.center.Y - c1.center.Y) - (s2.center.Y - s1.center.Y) * (s1.center.X - c1.center.X))) / d;

            Vertex s = new Vertex(new Point(s1.center.X + (int)(toSource * (s2.center.X - s1.center.X)), s1.center.Y + (int)(toSource * (s2.center.Y - s1.center.Y))))
            {
                distance = toSource,
                IsIntersection = true
            };
            Vertex c = new Vertex(new Point(s1.center.X + (int)(toSource * (s2.center.X - s1.center.X)), s1.center.Y + (int)(toSource * (s2.center.Y - s1.center.Y))))
            {
                distance = toClip,
                IsIntersection = true
            };

            if (s.center.X < Math.Min(s1.center.X, s2.center.X) || s.center.X > Math.Max(s1.center.X, s2.center.X) || s.center.X < Math.Min(c1.center.X, c2.center.X) || s.center.X > Math.Max(c1.center.X, c2.center.X)) return null;
            if (s.center.Y < Math.Min(s1.center.Y, s2.center.Y) || s.center.Y > Math.Max(s1.center.Y, s2.center.Y) || s.center.Y < Math.Min(c1.center.Y, c2.center.Y) || s.center.Y > Math.Max(c1.center.Y, c2.center.Y)) return null;

            if ((0 < toSource && toSource < 1) && (0 < toClip && toClip < 1))
            {
                return (s, c);
            }
            else
                return null;
        }

        public static (Polygon, Polygon) makeIntersectionPoints(Polygon source, Polygon clip)
        {
            Polygon sourceCopy = new Polygon(source);
            Polygon clipCopy = new Polygon(clip);
            foreach (Edge e in sourceCopy.edges)
                foreach (Edge e1 in clipCopy.edges)
                {
                    Vertex s1 = e.v1;
                    Vertex s2 = e.v2;
                    Vertex c1 = e1.v1;
                    Vertex c2 = e1.v2;
                    (Vertex s, Vertex c)? inter = getIntersectionPoints(s1, s2, c1, c2);
                    if (inter.HasValue)
                    {                   
                        if (sourceCopy.vertices[(sourceCopy.vertices.IndexOf(s1) + 1) % sourceCopy.vertices.Count].IsIntersection && (sourceCopy.vertices[(sourceCopy.vertices.IndexOf(s1) + 1) % sourceCopy.vertices.Count].distance < inter.Value.s.distance))
                        {
                            Vertex tmp = sourceCopy.vertices[(sourceCopy.vertices.IndexOf(s1) + 1) % sourceCopy.vertices.Count];
                            while (sourceCopy.vertices[(sourceCopy.vertices.IndexOf(tmp) + 1) % sourceCopy.vertices.Count].IsIntersection && (sourceCopy.vertices[(sourceCopy.vertices.IndexOf(tmp) + 1) % sourceCopy.vertices.Count].distance < inter.Value.s.distance))
                            {
                                tmp = sourceCopy.vertices[(sourceCopy.vertices.IndexOf(tmp) + 1) % sourceCopy.vertices.Count];
                            }
                            sourceCopy.vertices.Insert((sourceCopy.vertices.IndexOf(tmp) + 1) % sourceCopy.vertices.Count, inter.Value.s);
                        }
                        else
                        {                            
                            sourceCopy.vertices.Insert((sourceCopy.vertices.IndexOf(s1) + 1) % sourceCopy.vertices.Count, inter.Value.s);
                        }
                        if (clipCopy.vertices[(clipCopy.vertices.IndexOf(c1) + 1) % clipCopy.vertices.Count].IsIntersection && (clipCopy.vertices[(clipCopy.vertices.IndexOf(c1) + 1) % clipCopy.vertices.Count].distance < inter.Value.c.distance))
                        {
                            Vertex tmp = clipCopy.vertices[(clipCopy.vertices.IndexOf(c1) + 1) % clipCopy.vertices.Count];
                            while (clipCopy.vertices[(clipCopy.vertices.IndexOf(tmp) + 1) % clipCopy.vertices.Count].IsIntersection && (clipCopy.vertices[(clipCopy.vertices.IndexOf(tmp) + 1) % clipCopy.vertices.Count].distance < inter.Value.c.distance))
                            {
                                tmp = clipCopy.vertices[(clipCopy.vertices.IndexOf(tmp) + 1) % clipCopy.vertices.Count];
                            }
                            clipCopy.vertices.Insert(clipCopy.vertices.IndexOf(tmp) + 1, inter.Value.c);
                        }
                        else clipCopy.vertices.Insert(clipCopy.vertices.IndexOf(c1) + 1, inter.Value.c);
                        inter.Value.s.correspondingVertex = inter.Value.c;
                        inter.Value.c.correspondingVertex = inter.Value.s;
                    }
                }
            sourceCopy.edges = new List<Edge>();
            clipCopy.edges = new List<Edge>();
            for (int i = 0; i < sourceCopy.vertices.Count - 1; i++)
                sourceCopy.edges.Add(new Edge(sourceCopy.vertices[i], sourceCopy.vertices[i + 1]));
            sourceCopy.edges.Add(new Edge(sourceCopy.vertices[sourceCopy.vertices.Count - 1], sourceCopy.vertices[0]));
            for (int i = 0; i < clipCopy.vertices.Count - 1; i++)
                clipCopy.edges.Add(new Edge(clipCopy.vertices[i], clipCopy.vertices[i + 1]));
            clipCopy.edges.Add(new Edge(clipCopy.vertices[clipCopy.vertices.Count - 1], clipCopy.vertices[0]));
            return (sourceCopy, clipCopy);
        }

        public static (Polygon, Polygon) markEntryPoints(Polygon source, Polygon clip)
        {
            (Polygon sourceCopy, Polygon clipCopy) = makeIntersectionPoints(source, clip);
            bool flag = false;
            Vertex s1 = sourceCopy.vertices.First();
            if (isInside(s1.center, clipCopy))
            {
                flag = true;
                s1.Inside = true;
            }
            do
            {
                if (isInside(s1.center, clipCopy)) s1.Inside = true;
                if (s1.IsIntersection == true)
                {

                    s1.IsEntry = !flag;
                    flag = !flag;
                }
                s1 = sourceCopy.vertices[(sourceCopy.vertices.IndexOf(s1) + 1) % sourceCopy.vertices.Count];
            } while (s1 != sourceCopy.vertices.First());
            s1 = clipCopy.vertices.First();
            if (isInside(s1.center, sourceCopy))
            {
                flag = true;
                s1.Inside = true;
            }
            else flag = false;
            do
            {
                if (isInside(s1.center, sourceCopy)) s1.Inside = true;
                if (s1.IsIntersection == true)
                {
                    s1.IsEntry = !flag;
                    flag = !flag;
                }
                s1 = clipCopy.vertices[(clipCopy.vertices.IndexOf(s1) + 1) % clipCopy.vertices.Count];
            } while (s1 != clipCopy.vertices.First());
            return (sourceCopy, clipCopy);
        }

        public static List<Polygon> WeilerAtherton(Polygon source, Polygon clip)
        {
            (Polygon sourceCopy, Polygon clipCopy) = markEntryPoints(source, clip);
            List<Polygon> result = new List<Polygon>();
            List<Vertex> intersections = new List<Vertex>();
            bool isMovingForward = true;
            foreach(Vertex v in sourceCopy.vertices)
                if(v.IsIntersection == true)
                {
                    intersections.Add(v);
                }
            if (intersections.Count == 0)
            {
                if(isInside(clipCopy.vertices.First().center, sourceCopy))
                {
                    result.Add(clipCopy);
                    return result;
                }
                else if (isInside(sourceCopy.vertices.First().center, clipCopy))
                {
                    result.Add(sourceCopy);
                    return result;
                }
            }
            while(intersections.Count > 0)
            {
                List<Vertex> partialResult = new List<Vertex>();
                //??
                //Vertex s1 = sourceCopy.vertices.Find(v => v.center.X == intersections.First().center.X && v.center.Y == intersections.First().center.Y && v.IsEntry == true);
                //if(s1 == null)
                //{
                //    //to jest do usuniecia jak bede dobrze rozpoznawal wierzcholki
                //    intersections.RemoveAt(0);
                //    continue;
                //}
                Vertex s1 = sourceCopy.vertices.Find(v => v.center.X == intersections.First().center.X && v.center.Y == intersections.First().center.Y && v.IsIntersection == true);
                while (true)
                {
                    if (s1.Visited == true) break;
                    s1.Visited = true;
                    partialResult.Add(s1);
                    if(s1.IsIntersection == true)
                    {
                        if (intersections.Contains(s1)) intersections.Remove(s1);
                        else intersections.Remove(s1.correspondingVertex);
                        s1 = s1.correspondingVertex;
                        isMovingForward = s1.IsEntry ? true : false;
                    }
                    if(isMovingForward == true)
                    {
                        if (sourceCopy.vertices.Contains(s1))
                            s1 = sourceCopy.vertices[(sourceCopy.vertices.IndexOf(s1) + 1) % sourceCopy.vertices.Count];
                        else s1 = clipCopy.vertices[(clipCopy.vertices.IndexOf(s1) + 1) % clipCopy.vertices.Count];                        
                    }
                    else
                    {
                        if (sourceCopy.vertices.Contains(s1))
                            s1 = sourceCopy.vertices[sourceCopy.vertices.IndexOf(s1) == 0 ? sourceCopy.vertices.Count - 1 : sourceCopy.vertices.IndexOf(s1) - 1];
                        else s1 = clipCopy.vertices[clipCopy.vertices.IndexOf(s1) == 0 ? clipCopy.vertices.Count - 1 : clipCopy.vertices.IndexOf(s1) - 1];
                    }                        
                }
                if (partialResult.Count == 0) continue;
                List<Edge> edges = new List<Edge>();
                for (int i = 0; i < partialResult.Count - 1; i++)
                    edges.Add(new Edge(partialResult[i], partialResult[i + 1]));
                edges.Add(new Edge(partialResult[partialResult.Count - 1], partialResult[0]));
                result.Add(new Polygon(partialResult, edges));
            }
            return result;
        }
    }
}
