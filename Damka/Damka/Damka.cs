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
                return board.Evaluate(isRed);
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

        private int GetEvaluation(int depth, bool isRed)
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
            string boardName = boardName = ShowDialog("Enter board name", "Board Name");

            if (boardName.Equals(""))
            {
                return;
            }
            boardName.Replace("/", "_");

            string filePath = this.directoryPath + "\\" + boardName;
            if (File.Exists(filePath))
            {
                MessageBox.Show("You have already saved a board with this name");
                return;
            }
            FileStream myFile = File.Create(filePath);
            myFile.Close();
            List<byte> bytesToWrite = new List<byte>();
            bytesToWrite.AddRange(this.board.GetDamkaBoard().ConvertTo1DArray());
            bytesToWrite.Add((byte)this.board.GetDamkaBoard().GetNumberOfMovesWithoutSkips());
            File.WriteAllBytes(filePath, bytesToWrite.ToArray());
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
            string[] files = Directory.GetFiles(this.directoryPath);
            foreach (string file in files)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = Path.GetFileName(file);
                item.Click += (s, a) =>
                {
                    if (this.isThinking)
                    {
                        MessageBox.Show("Can't load now, wait for your opponent to end his turn...");
                        return;
                    }
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
    }
}
