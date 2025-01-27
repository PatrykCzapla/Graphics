﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    [Serializable]
    public class Sphere : Model
    {
        public double radius;
        public int slices;
        public int stacks;

        public Sphere(double radius, int slices, int stacks)
        {
            this.radius = radius;
            this.slices = slices;
            this.stacks = stacks;
            position = new Vector(0, 0, 0);
            rotation = new Vector(0, 0, 0);
            scale = new Vector(1, 1, 1);
            this.name = "Sphere";            
            generateMesh();
            mesh.makeMesh();
        }

        public void generateMesh()
        {
            Mesh mesh = new Mesh();

            Triangle triangle = new Triangle();

            for(int t = 0; t < stacks; t++)
            {
                double theta1 = ((double)t / stacks) * Math.PI;
                double theta2 = ((double)(t + 1) / stacks) * Math.PI;

                for(int p = 0; p < slices; p++)
                {
                    double phi1 = ((double)p / slices) * 2 * Math.PI;
                    double phi2 = ((double)(p + 1) / slices) * 2 * Math.PI;

                    Vertex v0 = new Vertex(radius * Math.Sin(theta1) * Math.Cos(phi1), radius * Math.Cos(theta1), radius * Math.Sin(theta1) * Math.Sin(phi1));
                    Vertex v1 = new Vertex(radius * Math.Sin(theta1) * Math.Cos(phi2), radius * Math.Cos(theta1), radius * Math.Sin(theta1) * Math.Sin(phi2));
                    Vertex v2 = new Vertex(radius * Math.Sin(theta2) * Math.Cos(phi2), radius * Math.Cos(theta2), radius * Math.Sin(theta2) * Math.Sin(phi2));
                    Vertex v3 = new Vertex(radius * Math.Sin(theta2) * Math.Cos(phi1), radius * Math.Cos(theta2), radius * Math.Sin(theta2) * Math.Sin(phi1));
                
                    if(t == 0)
                    {
                        triangle.vertices.Add(v0);
                        triangle.vertices.Add(v2);
                        triangle.vertices.Add(v3);
                        foreach (Vertex v in triangle.vertices)
                            v.makeNormal(this.position);
                        mesh.triangles.Add(triangle);
                    }
                    else if(t + 1 == stacks)
                    {
                        triangle = new Triangle();
                        triangle.vertices.Add(v2);
                        triangle.vertices.Add(v0);
                        triangle.vertices.Add(v1);
                        foreach (Vertex v in triangle.vertices)
                            v.makeNormal(this.position);
                        mesh.triangles.Add(triangle);
                    }
                    else
                    {
                        triangle = new Triangle();
                        triangle.vertices.Add(v0);
                        triangle.vertices.Add(v1);
                        triangle.vertices.Add(v3);
                        foreach (Vertex v in triangle.vertices)
                            v.makeNormal(this.position);
                        mesh.triangles.Add(triangle);

                        triangle = new Triangle();
                        triangle.vertices.Add(v1);
                        triangle.vertices.Add(v2);
                        triangle.vertices.Add(v3);
                        foreach (Vertex v in triangle.vertices)
                            v.makeNormal(this.position);
                        mesh.triangles.Add(triangle);
                    }
                }
            }
            this.mesh = mesh;
        }


    }
}
