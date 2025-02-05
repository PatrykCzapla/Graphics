﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    public class Matrix
    {
        private int size;
        public double[,] values;
        
        public Matrix(double[,] values)
        {
            this.values = values;
            this.size = values.GetLength(0);
        }

        public static Matrix Translation(double x, double y, double z)
        {
            double[,] values = new double[4, 4];
            values[0, 0] = 1;
            values[1, 1] = 1;
            values[2, 2] = 1;
            values[3, 3] = 1;
            values[0, 3] = x;
            values[1, 3] = y;
            values[2, 3] = z;
            return new Matrix(values);
        }

        public static Matrix Scale(double x, double y, double z)
        {
            double[,] values = new double[4, 4];
            values[0, 0] = x;
            values[1, 1] = y;
            values[2, 2] = z;
            values[3, 3] = 1;
            return new Matrix(values);
        }

        public static Matrix xRotation(double a)
        {
            double[,] values = new double[4, 4];
            values[0, 0] = 1;
            values[1, 1] = Math.Cos(a);
            values[2, 2] = Math.Cos(a);
            values[3, 3] = 1;
            values[1, 2] = (-1) * Math.Sin(a);
            values[2, 1] = Math.Sin(a);
            return new Matrix(values);
        }

        public static Matrix yRotation(double a)
        {
            double[,] values = new double[4, 4];
            values[0, 0] = Math.Cos(a);
            values[1, 1] = 1;
            values[2, 2] = Math.Cos(a);
            values[3, 3] = 1;
            values[0, 2] = (-1) * Math.Sin(a);
            values[2, 0] = Math.Sin(a);
            return new Matrix(values);
        }

        public static Matrix zRotation(double a)
        {
            double[,] values = new double[4, 4];
            values[0, 0] = Math.Cos(a);
            values[1, 1] = Math.Cos(a);
            values[2, 2] = 1;
            values[3, 3] = 1;
            values[0, 1] = (-1) * Math.Sin(a);
            values[1, 0] = Math.Sin(a);
            return new Matrix(values);
        }

        public Vector MatrixByVector(Vector vector)
        {
            double[] result = new double[size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    result[i] += values[i, j] * vector.values[j];
            return new Vector(result);
        }

        public Matrix MatrixByMatrix(Matrix matrix)
        {
            double[,] result = new double[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    for (int k = 0; k < size; k++)
                        result[i, j] += values[i, k] * matrix.values[k, j];
            return new Matrix(result);
        }

        public static Matrix projectionMatrix(double fov, double far, double close, double aspect)
        {
            double[,] values = new double[4, 4];

            double ctg = 1 / Math.Tan(fov / 2);

            values[0, 0] = ctg / aspect;
            values[1, 1] = ctg;
            values[2, 2] = (far + close) / (far - close);
            values[3, 2] = -1;
            values[2, 3] = (-2 * far * close) / (far - close);

            return new Matrix(values);
        }

        public static Matrix getModelMatrix(Model model)
        {
            Matrix rotations = Matrix.xRotation(model.rotation.values[0]).MatrixByMatrix(Matrix.yRotation(model.rotation.values[1])).MatrixByMatrix(Matrix.zRotation(model.rotation.values[2]));
            Matrix scales = Matrix.Scale(model.scale.values[0], model.scale.values[1], model.scale.values[2]);
            Matrix positions = Matrix.Translation(model.position.values[0], model.position.values[1], model.position.values[2]);

            return positions.MatrixByMatrix(rotations).MatrixByMatrix(scales);
        }
    }
}
