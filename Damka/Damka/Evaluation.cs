using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damka
{
    class PieceForEvaluation
    {
        private int evaluation;
        public PieceForEvaluation(int y, int x, Piece piece)
        {
            DistanceToQueenAndCenter(y, x, piece);
        }

        private void OnlyPices(Piece piece)
        {
            switch (piece)
            {
                case Piece.Nothing:
                    this.evaluation = 0;
                    break;
                case Piece.BlackPiece:
                    this.evaluation = 1;
                    break;
                case Piece.BlackQueen:
                    this.evaluation = 2;
                    break;
                case Piece.RedPiece:
                    this.evaluation = -1;
                    break;
                case Piece.RedQueen:
                    this.evaluation = -2;
                    break;

            }
        }

        private void DistanceToQueenAndCenter(int y, int x, Piece piece)
        {
            if (piece == Piece.RedQueen)
            {
                this.evaluation = -15 + GetDistanceToCenter(y, x);
            }
            else if (piece == Piece.RedPiece)
            {
                int zeroY = y == 4 ? 0 : y;
                this.evaluation = (-1 * (5 + 7 - zeroY)) + GetIsEdge(y, x, piece);
            }
            else if (piece == Piece.BlackQueen)
            {
                this.evaluation = 15 - GetDistanceToCenter(y, x);
            }
            else if (piece == Piece.BlackPiece)
            {
                int zeroY = y == 0 ? 4 : y;
                this.evaluation = 5 + zeroY + GetIsEdge(y, x, piece);
            }
            else
            {
                this.evaluation = 0;
            }
        }

        private int GetIsEdge(int y, int x, Piece piece)
        {
            if ((x == 7 || x == 0) && y != 0 && y != 7)
            {
                if (piece == Piece.RedPiece)
                {
                    return -2;
                }
                if (piece == Piece.BlackPiece)
                {
                    return 2;
                }
            }
            return 0;
        }

        private void DistanceToQueen(int y, int x, Piece piece)
        {
            if (piece == Piece.RedQueen)
            {
                this.evaluation = -20;
            }
            else if (piece == Piece.RedPiece)
            {
                this.evaluation = -1 * (5 + (7 - y));
            }
            else if (piece == Piece.BlackQueen)
            {
                this.evaluation = 20;
            }
            else if (piece == Piece.BlackPiece)
            {
                this.evaluation = 5 + y;
            }
            else
            {
                this.evaluation = 0;
            }
        }

        private int GetDistanceToCenter(int y, int x)
        {
            return (int)Math.Sqrt(Math.Pow((x - 3), 2) + Math.Pow((y - 4), 2));
        }

        public static int GetEval(PieceForEvaluation[] pices)
        {
            int total = 0;
            for (int i = 0; i < pices.Length; i++)
            {
                total = total + pices[i].evaluation;
            }
            return total;
        }
    }
}
