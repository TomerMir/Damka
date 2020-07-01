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

namespace Damka
{
    public partial class Damka : Form
    {
        Board board;
        List<Tuple<DamkaBoard, int>> movesWithEvaluations;
        bool isThinking = false;
        private Thread doTurn;
        private Thread evaluationThread;
        private int depth;
        private string directoryPath;
        private string boardsDirectoryPath;
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
            this.board.AppendFromDamkaBoard(GetBestMove(9, false));
            //file
            string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Damka";
            string boardsDirectoryPath = directoryPath + "\\Boards";
            if (!Directory.Exists(directoryPath))
            {
                DirectoryInfo directory = Directory.CreateDirectory(directoryPath);
                directory.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            if (!Directory.Exists(boardsDirectoryPath))
            {
                Directory.CreateDirectory(boardsDirectoryPath);
            }
            this.boardsDirectoryPath = boardsDirectoryPath;
            this.directoryPath = directoryPath;
            SetEvaluation();
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
                    if (this.evaluationThread.IsAlive)
                    {
                        this.evaluationThread.Abort();
                    }
                    this.evaluation.Text = "";
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
                    SetEvaluation();
                    this.isThinking = false;
                    return;
                }
                this.board.ClearMoves();
                this.board.ShowMoves(clickedCell);
                this.isThinking = false;
            });
            this.doTurn.Start();
        }
        private int MinMax(DamkaBoard board, int depth, bool isRed, int alpha = int.MinValue, int beta = int.MaxValue, bool isFirstMove = true, bool doNull = true)
        {
            if (depth <= 0 || board.WhoWins() != Winner.NoOne)
            {
                if (!board.IsSkipRequired(isRed))
                {
                    return board.Evaluate(isRed);
                }
            }

            //if (depth >= 4 && !isFirstMove && doNull)
            //{
            //    int eval = MinMax(board.MakeNullMove(), depth - 4, !isRed,  beta, beta - 1, isFirstMove, false);
            //    if (eval >= beta)
            //    {
            //        return beta;
            //    }
            //}

            if (isRed)
            {
                int minEval = int.MaxValue;
                foreach (DamkaBoard tmpBoard in board.GetAllMoves(isRed))
                {
                    int eval = MinMax(tmpBoard, depth - 1, !isRed, alpha, beta, false, doNull);
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
                    int eval = MinMax(tmpBoard, depth - 1, !isRed, alpha, beta, false, doNull);
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

        private void SetEvaluation()
        {
            evaluationThread = new Thread(() =>
            {
                int depth = 2;
                while (depth <= 20)
                {
                    double eval = (double)(GetEvaluation(depth, true)) / -100;
                    this.evaluation.Text = eval.ToString() + " {" + depth + "}";
                    depth++;
                }
            });
            evaluationThread.Start();
        }

        private int GetEvaluationMultiThread(int depth, bool isRed)
        {
            DamkaBoard[] moves = this.board.GetDamkaBoard().GetAllMoves(isRed);
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
                        if (isRed)
                        {
                            return int.MaxValue - 1;
                        }
                        else
                        {
                            return int.MinValue + 1;
                        }
                    }
                    if (isRed)
                    {
                        movesWithEvaluations.Sort((x, y) => x.Item2.CompareTo(y.Item2));
                    }
                    else
                    {
                        movesWithEvaluations.Sort((x, y) => y.Item2.CompareTo(x.Item2));
                    }
                    return movesWithEvaluations.First().Item2;
                }
            }
        }

        private int GetEvaluation(int depth, bool isRed)
        {
            DamkaBoard[] moves = this.board.GetDamkaBoard().GetAllMoves(isRed);
            this.movesWithEvaluations = new List<Tuple<DamkaBoard, int>>(moves.Length);
            DamkaBoard[] shuffledMoves = moves.OrderBy(a => Guid.NewGuid()).ToArray();
            int bestEval;
            if (isRed)
            {
                bestEval = int.MaxValue;
                foreach (DamkaBoard move in shuffledMoves)
                {
                    int moveEval = MinMax(move, depth, !isRed);
                    if (moveEval < bestEval)
                    {
                        bestEval = moveEval;
                    }
                }
            }
            else
            {
                bestEval = int.MinValue;
                foreach (DamkaBoard move in shuffledMoves)
                {
                    int moveEval = MinMax(move, depth, !isRed);
                    if (moveEval > bestEval)
                    {
                        bestEval = moveEval;
                    }
                }
            }
            return bestEval;
        }

        private void GetEvalNewThread(DamkaBoard move, int depth, bool isRed)
        {
            Thread Evaluate = new Thread(() =>
            {
                int moveEval = MinMax(move, depth, isRed);
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
            if (this.doTurn != null)
            {
                if (this.doTurn.IsAlive)
                {
                    this.doTurn.Abort();
                }
                this.isThinking = false;
            }
            if (this.evaluationThread != null)
            {
                if (this.evaluationThread.IsAlive)
                {
                    this.evaluationThread.Abort();
                }
            }
            
        }

        private void ResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetTurnThread();
            SetEvaluation();
            this.board.Reset();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.isThinking)
            {
                MessageBox.Show("Can't save now, wait for your opponent to end his turn...");
                return;
            }
            string boardName = boardName = ShowDialog("Enter board name", "Board Name");

            if (boardName.Equals(""))
            {
                return;
            }
            boardName.Replace("/", "_");

            string filePath = this.boardsDirectoryPath + "\\" + boardName;
            if (File.Exists(filePath))
            {
                MessageBox.Show("You have already saved a board with this name");
                return;
            }
            FileStream myFile = File.Create(filePath);
            myFile.Close();
            File.WriteAllBytes(filePath, this.board.GetDamkaBoard().ConvertTo1DArray());
            File.SetAttributes(filePath, FileAttributes.ReadOnly);
        }

        private void EvaluateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.isThinking)
            {
                MessageBox.Show("Can't evaluate now, wait for your opponent to end his turn...");
                return;
            }
            Thread GetEval = new Thread(() =>
            {
                MessageBox.Show((GetEvaluation(6, true) * -1).ToString(), "Evaluation");
            });
            GetEval.Start();
        }

        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 300,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 100, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 200 };
            Button confirmation = new Button() { Text = "Ok", Left = 95, Width = 100, Top = 75, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        private void AddBoardsToToolStrip()
        {
            string[] files = Directory.GetFiles(this.boardsDirectoryPath);
            foreach (string file in files)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = Path.GetFileName(file);
                item.Click += (s, a) =>
                {
                    ResetTurnThread();
                    SetEvaluation();
                    this.board.ClearMoves();
                    DamkaBoard tmpBoard = new DamkaBoard(File.ReadAllBytes(file));
                    this.board.AppendFromDamkaBoard(tmpBoard);
                };
                loadToolStripMenuItem.DropDownItems.Add(item);
            }
        }
        
        private void LoadToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            ((ToolStripMenuItem)sender).DropDownItems.Clear();
            AddBoardsToToolStrip();
        }

        private void Damka_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }
    }
}
