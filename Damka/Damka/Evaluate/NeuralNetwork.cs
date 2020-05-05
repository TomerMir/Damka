using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damka.Evaluate
{
    public class NeuralNetwork
    {
        private readonly double mutationRate;
        private Matrix weightHiddenOutput;
        private Matrix weightInputHidden;
        private Random rand = new Random();

        public NeuralNetwork(int numberOfInputNodes, int numberOfHiddenNodes, int numberOfOutputNodes, double mutationRate)
        {
            this.mutationRate = mutationRate;

            this.weightInputHidden = Matrix.Create(numberOfHiddenNodes, numberOfInputNodes);
            this.weightHiddenOutput = Matrix.Create(numberOfOutputNodes, numberOfHiddenNodes);

            RandomizeWeights();
        }

        private NeuralNetwork(double mutationRate)
        {
            this.mutationRate = mutationRate;
        }

        public NeuralNetwork Clone()
        {
            NeuralNetwork tmp = new NeuralNetwork(this.mutationRate);
            tmp.weightHiddenOutput = this.weightHiddenOutput.Clone();
            tmp.weightInputHidden = this.weightInputHidden.Clone();
            return tmp;
        }

        private double GetRandomNumber()
        {
            return rand.NextDouble() * 2 - 1;
        }

        private void RandomizeWeights()
        {
            this.weightHiddenOutput.Initialize(GetRandomNumber);
            this.weightInputHidden.Initialize(GetRandomNumber);
        }

        private void Mutate()
        {
            this.weightHiddenOutput.Mutate(this.mutationRate, this.rand, GetRandomNumber);
            this.weightInputHidden.Mutate(this.mutationRate, this.rand, GetRandomNumber);
        }

        public NeuralNetwork GetMutated()
        {
            NeuralNetwork mutated = this.Clone();
            mutated.Mutate();
            return mutated;
        }

        private double[] Query(double[] inputs)
        {
            Matrix inputSignals = ConvertToMatrix(inputs);

            Matrix hiddenOutputs = Sigmoid(this.weightInputHidden * inputSignals);
            Matrix finalOutputs = Sigmoid(this.weightHiddenOutput * hiddenOutputs);

            return finalOutputs.Value.SelectMany(x => x.Select(y => y)).ToArray();
        }

        public double GetEvalutaion(double[] inputs)
        {
            return Query(inputs)[0];
        }

        private static Matrix ConvertToMatrix(double[] inputArr)
        {
            double[][] input = new double[inputArr.Length][];

            for (int x = 0; x < input.Length; x++)
            {
                input[x] = new[] { inputArr[x] };
            }

            return Matrix.Create(input);
        }

        private Matrix Sigmoid(Matrix matrix)
        {
            Matrix newMatrix = Matrix.Create(matrix.Value.Length, matrix.Value[0].Length);

            for (int x = 0; x < matrix.Value.Length; x++)
            {
                for (int y = 0; y < matrix.Value[x].Length; y++)
                {
                    newMatrix.Value[x][y] = 1 / (1 + Math.Pow(Math.E, -matrix.Value[x][y]));
                }
            }

            return newMatrix;
        }
    }
}
