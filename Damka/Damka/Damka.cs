using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Damka.Evaluate;

namespace Damka
{
    public partial class Damka : Form
    {
        Board board;
        List<Tuple<DamkaBoard, int>> movesWithEvaluations;
        bool isThinking = false;
        private Thread doTurn;
        private int depth;
        private string directoryPath;
        public Damka()
        {
            InitializeComponent();
            this.board = new Board(this);
            this.Controls.Add(board);
            this.Size = new Size((int)(this.board.Width * 1.02), (int)(this.board.Height * 1.07875));
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.MaximizeBox = false;
            this.medium36SecondsToolStripMenuItem.Checked = true;
            this.depth = 6;

            //file
            string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Damka";
            if (!Directory.Exists(directoryPath))
            {
                DirectoryInfo directory = Directory.CreateDirectory(directoryPath);
                directory.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            this.directoryPath = directoryPath;
        }

        private void BotGames(int firstDepthm, int secondDepth)
        {
            while (true)
            {
                ShowMove(GetBestMove(firstDepthm, true));
                switch (this.board.GetDamkaBoard().WhoWins())
                {
                    case Winner.Black:
                        MessageBox.Show("Black won!");
                        this.board.Reset();
                        break;

                    case Winner.Red:
                        MessageBox.Show("Red won!");
                        this.board.Reset();
                        break;

                    case Winner.Draw:
                        MessageBox.Show("Tie");
                        this.board.Reset();
                        break;
                }
                ShowMove(GetBestMove(secondDepth, false));
                switch (this.board.GetDamkaBoard().WhoWins())
                {
                    case Winner.Black:
                        MessageBox.Show("Black won!");
                        this.board.Reset();
                        break;

                    case Winner.Red:
                        MessageBox.Show("Red won!");
                        this.board.Reset();
                        break;

                    case Winner.Draw:
                        MessageBox.Show("Tie");
                        this.board.Reset();
                        break;
                }
            }
        }

        public void Button_Click(object sender, EventArgs e)
        {
            if (this.isThinking)
            {
                return;
            }
            this.doTurn = new Thread(() =>
            {
                this.isThinking = true;
                Cell clickedCell = sender as Cell;
                if (clickedCell.GetBoard() != null)
                {
                    DamkaBoard tmpBoard = clickedCell.GetBoard();
                    this.board.ClearMoves();
                    this.board.AppendFromDamkaBoard(tmpBoard);
                    Application.DoEvents();
                    DamkaBoard bestMove = GetBestMove(this.depth, false);
                    if (bestMove == null)
                    {
                        MessageBox.Show("Red won!");
                        this.board.Reset();
                        this.isThinking = false;
                        return;
                    }
                    this.board.AppendFromDamkaBoard(bestMove);
                    switch (bestMove.WhoWins())
                    {
                        case Winner.Black:
                            MessageBox.Show("Black won!");
                            this.board.Reset();
                            break;

                        case Winner.Red:
                            MessageBox.Show("Red won!");
                            this.board.Reset();
                            break;

                        case Winner.Draw:
                            MessageBox.Show("Tie");
                            this.board.Reset();
                            break;
                    }
                    this.isThinking = false;
                    return;
                }
                this.board.ClearMoves();
                this.board.ShowMoves(clickedCell);
                this.isThinking = false;
            });
            this.doTurn.Start();
        }

        private int MinMax(DamkaBoard board, int depth, bool isRed, int alpha = int.MinValue, int beta = int.MaxValue)
        {
            if (depth == 0 || board.WhoWins() != Winner.NoOne)
            {
                return board.Evaluate();
            }

            if (isRed)
            {
                int minEval = int.MaxValue;
                foreach (DamkaBoard tmpBoard in board.GetAllMoves(isRed))
                {
                    int eval = MinMax(tmpBoard, depth - 1, !isRed, alpha, beta);
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
                return minEval;
            }

            else
            {
                int maxEval = int.MinValue;
                foreach (DamkaBoard tmpBoard in board.GetAllMoves(isRed))
                {
                    int eval = MinMax(tmpBoard, depth - 1, !isRed, alpha, beta);
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
                return maxEval;
            }
        }

        private void ShowMove(DamkaBoard move)
        {
            this.board.AppendFromDamkaBoard(move);
            Application.DoEvents();
            Thread.Sleep(30);
            Application.DoEvents();
        }

        private DamkaBoard GetBestMove(int depth, bool isRed)
        {
            DamkaBoard[] moves = this.board.GetDamkaBoard().GetAllMoves(isRed);
            if (moves.Length == 1)
            {
                return moves[0];
            }
            this.movesWithEvaluations = new List<Tuple<DamkaBoard, int>>(moves.Length);
            DamkaBoard[] shuffledMoves = moves.OrderBy(a => Guid.NewGuid()).ToArray();
            foreach (DamkaBoard move in shuffledMoves)
            {
                GetEvalNewThread(move, depth, !isRed);
            }
            while (true)
            {
                if (movesWithEvaluations.Count == moves.Length)
                {
                    if (movesWithEvaluations.Count == 0)
                    {
                        return null;
                    }
                    if (isRed)
                    {
                        movesWithEvaluations.Sort((x, y) => x.Item2.CompareTo(y.Item2));
                    }
                    else
                    {
                        movesWithEvaluations.Sort((x, y) => y.Item2.CompareTo(x.Item2));
                    }
                    return movesWithEvaluations.First().Item1;
                }
            }
        }

        private void GetEvalNewThread(DamkaBoard move, int depth, bool isRed)
        {
            Thread Evaluate = new Thread(() =>
            {
                int moveEval = 0;
                Winner winner = move.WhoWins();
                if (winner == Winner.Black)
                {
                    moveEval = int.MaxValue - 1;
                }
                else if (winner == Winner.Red)
                {
                    moveEval = int.MinValue + 1;
                }
                else
                {
                    moveEval = MinMax(move, depth, isRed);
                }
                this.movesWithEvaluations.Add(new Tuple<DamkaBoard, int>(move, moveEval));
            });
            Evaluate.Start();
        }

        private void UncheckAll()
        {
            foreach (ToolStripMenuItem item in levelToolStripMenuItem.DropDownItems)
            {
                item.Checked = false;
            }
        }
        private void EasyInstantToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAll();
            ((ToolStripMenuItem)sender).Checked = true;
            this.depth = 2;
        }

        private void Medium36SecondsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAll();
            ((ToolStripMenuItem)sender).Checked = true;
            this.depth = 6;
        }

        private void Hard710SecondsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAll();
            ((ToolStripMenuItem)sender).Checked = true;
            this.depth = 8;
        }

        private void SuperHard2030SecondsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckAll();
            ((ToolStripMenuItem)sender).Checked = true;
            this.depth = 9;
        }

        private void ResetTurnThread()
        {
            if (this.doTurn == null)
            {
                return;
            }
            if (this.doTurn.IsAlive)
            {
                this.doTurn.Abort();
            }
            this.isThinking = false;
        }

        private void ResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetTurnThread();
            this.board.Reset();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.isThinking)
            {
                MessageBox.Show("Can't save now, wait for your opponent to end his turn...");
                return;
            }
            string hash = this.board.GetDamkaBoard().Hash();
            string filePath = this.directoryPath + "\\" + hash;
            if (File.Exists(filePath))
            {
                MessageBox.Show("You have already saved this board");
                return;
            }
            FileStream myFile = File.Create(filePath);
            myFile.Close();
            File.WriteAllBytes(filePath, this.board.GetDamkaBoard().ConvertTo1DArray());
            File.SetAttributes(filePath, FileAttributes.ReadOnly);
        }
    }
}
