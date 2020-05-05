using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damka.Evaluate
{
    public class Matrix
    {
        private readonly double[][] matrix;

        private Matrix(int rows, int cols)
        {
            this.matrix = new double[rows][];

            for (int i = 0; i < rows; i++)
            {
                this.matrix[i] = new double[cols];
            }
        }

        private Matrix(double[][] array)
        {
            this.matrix = array;
        }

        private static double[][] CreateJagged(int rows, int cols)
        {
            var jagged = new double[rows][];

            for (int i = 0; i < rows; i++)
            {
                jagged[i] = new double[cols];
            }

            return jagged;
        }

        public static Matrix Create(int rows, int cols)
        {
            return new Matrix(rows, cols);
        }

        public static Matrix Create(double[][] array)
        {
            return new Matrix(array);
        }

        public void Initialize(Func<double> elementInitializer)
        {
            for (int x = 0; x < this.matrix.Length; x++)
            {
                for (int y = 0; y < this.matrix[x].Length; y++)
                {
                    this.matrix[x][y] = elementInitializer();
                }
            }
        }

        public double[][] Value => this.matrix;

        public static Matrix operator -(Matrix a, Matrix b)
        {
            double[][] newMatrix = CreateJagged(a.Value.Length, b.Value[0].Length);

            for (int x = 0; x < a.Value.Length; x++)
            {
                for (int y = 0; y < a.Value[x].Length; y++)
                {
                    newMatrix[x][y] = a.Value[x][y] - b.Value[x][y];
                }
            }

            return Create(newMatrix);
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            double[][] newMatrix = CreateJagged(a.Value.Length, b.Value[0].Length);

            for (int x = 0; x < a.Value.Length; x++)
            {
                for (int y = 0; y < a.Value[x].Length; y++)
                {
                    newMatrix[x][y] = a.Value[x][y] + b.Value[x][y];
                }
            }

            return Create(newMatrix);
        }

        public static Matrix operator +(Matrix a, double b)
        {
            for (int x = 0; x < a.Value.Length; x++)
            {
                for (int y = 0; y < a.Value[x].Length; y++)
                {
                    a.Value[x][y] = a.Value[x][y] + b;
                }
            }

            return a;
        }

        public static Matrix operator -(double a, Matrix m)
        {
            for (int x = 0; x < m.Value.Length; x++)
            {
                for (int y = 0; y < m.Value[x].Length; y++)
                {
                    m.Value[x][y] = a - m.Value[x][y];
                }
            }

            return m;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.Value.Length == b.Value.Length && a.Value[0].Length == b.Value[0].Length)
            {
                double[][] m = CreateJagged(a.Value.Length, a.Value[0].Length);

                Parallel.For(0, a.Value.Length, i =>
                {
                    for (int j = 0; j < a.Value[i].Length; j++)
                    {
                        m[i][j] = a.Value[i][j] * b.Value[i][j];
                    }
                });

                return Create(m);
            }

            double[][] newMatrix = CreateJagged(a.Value.Length, b.Value[0].Length);

            if (a.Value[0].Length == b.Value.Length)
            {
                int length = a.Value[0].Length;

                Parallel.For(0, a.Value.Length, i =>
                {
                    for (int j = 0; j < b.Value[0].Length; j++)
                    {
                        double temp = 0.0;

                        for (int k = 0; k < length; k++)
                        {
                            temp += a.Value[i][k] * b.Value[k][j];
                        }

                        newMatrix[i][j] = temp;
                    }
                });
            }

            return Create(newMatrix);
        }

        public static Matrix operator *(double scalar, Matrix b)
        {
            var newMatrix = CreateJagged(b.Value.Length, b.Value[0].Length);

            for (int x = 0; x < b.Value.Length; x++)
            {
                for (int y = 0; y < b.Value[x].Length; y++)
                {
                    newMatrix[x][y] = b.Value[x][y] * scalar;
                }
            }

            return Create(newMatrix);
        }

        public void Mutate(double mutationRate, Random rnd, Func<double> function)
        {
            for (int x = 0; x < this.matrix.Length; x++)
            {
                for (int y = 0; y < this.matrix[x].Length; y++)
                {
                    if (rnd.NextDouble() < mutationRate)
                    {
                        this.matrix[x][y] = function();
                    }
                }
            }
        }

        public Matrix Clone()
        {
            int rows = this.matrix.Length;

            double[][] newMatrix = new double[this.matrix.Length][];

            for (int row = 0; row < this.matrix.Length; row++)
            {
                newMatrix[row] = new double[this.matrix[row].Length];
                for (int col = 0; col < this.matrix[row].Length; col++)
                {
                    newMatrix[row][col] = this.matrix[row][col];
                }
            }

            return Create(newMatrix);
        }
    }
}
