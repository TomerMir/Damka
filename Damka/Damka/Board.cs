﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Damka
{
    class Board : TableLayoutPanel
    {
        private DamkaBoard damkaBoard = new DamkaBoard();
        private Cell[,] board = new Cell[8, 8];

        public Board(Damka form)
        {
            this.Width = 800;
            this.Height = 800;
            this.ColumnCount = 8;
            this.RowCount = 8;
            for (int i = 0; i < 8; i++)
            {
                this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
                this.RowStyles.Add(new RowStyle(SizeType.Percent, 12.5F));

                for (int j = 0; j < 8; j++)
                {
                    Cell tmpCell = new Cell(i, j);

                    tmpCell.TabStop = false;
                    tmpCell.Font = new Font(tmpCell.Font.FontFamily, 70);
                    tmpCell.Size = new Size(this.Width / 8, this.Height / 8);
                    tmpCell.BackgroundImageLayout = ImageLayout.Stretch;
                    tmpCell.Click += new EventHandler(form.Button_Click);

                    if (j % 2 == 0)
                    {
                        if (i % 2 == 0)
                        {
                            tmpCell.SetOriginalColor(Color.LightGray);
                        }
                        else
                        {
                            if (i > 4)
                            {
                                tmpCell.SetPieceWithText(Piece.RedPiece);
                            }
                            else if(i < 3)
                            {
                                tmpCell.SetPieceWithText(Piece.BlackPiece);
                            }
                            tmpCell.SetOriginalColor(Color.DarkGray);
                        }
                    }
                    else
                    {
                        if (i % 2 != 0)
                        {
                            tmpCell.SetOriginalColor(Color.LightGray);
                        }
                        else
                        {
                            if (i > 4)
                            {
                                tmpCell.SetPieceWithText(Piece.RedPiece);
                            }
                            else if (i < 3)
                            {
                                tmpCell.SetPieceWithText(Piece.BlackPiece);
                            }
                            tmpCell.SetOriginalColor(Color.DarkGray);
                        }
                    }
                    this.board[i, j] = tmpCell;
                    this.damkaBoard.SetValueByIndex(i, j, tmpCell.GetPieceValue());
                    this.Controls.Add(tmpCell);
                 }
            }
        }

        public void AppendFromDamkaBoard(DamkaBoard board)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    this.board[i, j].SetPieceWithText(board.GetPieceByIndex(i, j));
                }
            }
            this.damkaBoard = board.Clone();
        }

        private void GetMovedToCell(DamkaBoard otherBoard, bool isSkip)
        {
            Piece[,] thisPices = this.damkaBoard.GetBoard();
            Piece[,] otherPices = otherBoard.GetBoard();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (thisPices[i,j] == Piece.Nothing && otherPices[i, j] != Piece.Nothing)
                    {
                        Cell cell = this.board[i, j];
                        cell.SetBoard(otherBoard);
                        cell.BackColor = isSkip ? Color.Red : Color.Green;
                    }
                }
            }
        }

        public void ClearMoves()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    
                    Cell cell = this.board[i, j];
                    cell.SetBoard(null);
                    cell.SetToOriginalColor();
                    
                }
            }
        }

        private void GetMovedToCells(DamkaBoard[] damkaBoards, bool isSkip)
        {
            foreach (DamkaBoard board in damkaBoards)
            {
                GetMovedToCell(board, isSkip);
            }
        }


        public void ShowMoves(Cell clickedCell)
        {
            int y = clickedCell.colume;
            int x = clickedCell.row;
            if (this.damkaBoard.GetPieceByIndex(y,x) == Piece.Nothing)
            {
                return;
            }

            DamkaBoard[] skipMoves = this.damkaBoard.GetAvailableSkipMoves(y, x);
            if (skipMoves.Length > 0)
            {
                GetMovedToCells(skipMoves, true);
            }

            else
            {
                DamkaBoard[] normalMoves = this.damkaBoard.GetAvailableNormalMoves(y, x);
                if (normalMoves.Length > 0)
                {
                    GetMovedToCells(normalMoves, false);
                }
            }
        }
    }

    class Cell : Button
    {
        public int colume { get; }
        public int row { get; }

        private DamkaBoard tmpBoard;       
        private Piece piece;
        private Color originalColor;

        public Cell(int colume, int row)
        {
            this.row = row;
            this.colume = colume;
            this.piece = Piece.Nothing;
        }

        public void SetToOriginalColor()
        {
            this.BackColor = this.originalColor;
        }

        public void SetOriginalColor(Color color)
        {
            this.BackColor = color;
            this.originalColor = color;
        }

        public void SetBoard(DamkaBoard board)
        {
            this.tmpBoard = board;
        }

        public DamkaBoard GetBoard()
        {
            return this.tmpBoard;
        }
        public int GetPieceValue()
        {
            return (int)this.piece;
        }

        public Cell(int row, int colume, Piece piece)
        {
            this.row = row;
            this.colume = colume;
            this.piece = piece;
        }

        public void SetPiece(Piece piece)
        {
            this.piece = piece;
        }

        public void SetPieceWithText(Piece piece)
        {
            SetPiece(piece);
            if (piece == Piece.Nothing)
            {
                this.BackgroundImage = null;
            }
            else if(piece == Piece.RedPiece)
            {
                this.BackgroundImage = Properties.Resources.RedPiece_remove;
            }
            else if (piece == Piece.BlackPiece)
            {
                this.BackgroundImage = Properties.Resources.BlackPiece_remove;
            }
            else if(piece == Piece.RedQueen)
            {
                this.BackgroundImage = Properties.Resources.RedQueen_remove;
            }
            else
            {
                this.BackgroundImage = Properties.Resources.BlackQueen_remove;
            }
        }
    }

    class DamkaBoard
    {
        private byte[,] board = new byte[8, 4];
        
        public void SetValueByIndex(int i, int j, int value)
        {
            this.board[i, j/2] = Utilities.SetByteValue(this.board[i, j/2], value, j % 2 == 0);
        }
        public Piece GetPieceByIndex(int i, int j)
        {
            return (Piece)Utilities.GetByteValue(this.board[i, j/2], j % 2 == 0);
        }
        public Piece GetPieceByIndex(int i, int j, DamkaBoard board)
        {
            return (Piece)Utilities.GetByteValue(board.board[i, j / 2], j % 2 == 0);
        }
        public Winner WhoWins()
        {
            int[] pices = GetPices();
            if (pices[0] == 0 && pices[1] == 0)
            {
                return Winner.Red;
            }
            else if(pices[2] == 0 && pices[3] == 0)
            {
                return Winner.Black;
            }
            return Winner.NoOne;
        } 

        public int[] GetPices()
        {
            int[] pices = new int[4] {0,0,0,0};
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    switch (GetPieceByIndex(i,j))
                    {
                        case Piece.BlackPiece:
                            pices[0]++;
                            break;
                        case Piece.BlackQueen:
                            pices[1]++;
                            break;
                        case Piece.RedPiece:
                            pices[2]++;
                            break;
                        case Piece.RedQueen:
                            pices[3]++;
                            break;
                    }
                }
            }
            return pices;
        }

        public DamkaBoard Clone()
        {
            DamkaBoard newBoard = new DamkaBoard();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    newBoard.board[i, j] = this.board[i, j];
                }
            }
            return newBoard;
        }

        public void Move(int fromY, int fromX, int toY, int toX)
        {
            Piece piece = GetPieceByIndex(fromY, fromX);
            if (piece == Piece.RedPiece && toY == 0)
            {
                this.board[toY, toX / 2] = Utilities.SetByteValue(this.board[toY, toX / 2], (int)Piece.RedQueen, toX % 2 == 0);
            }
            else if (piece == Piece.BlackPiece && toY == 7)
            {
                this.board[toY, toX / 2] = Utilities.SetByteValue(this.board[toY, toX / 2], (int)Piece.BlackQueen, toX % 2 == 0);
            }
            else
            {
                this.board[toY, toX / 2] = Utilities.SetByteValue(this.board[toY, toX / 2], Utilities.GetByteValue(this.board[fromY, fromX / 2], fromX % 2 == 0), toX % 2 == 0);
            }
            this.board[fromY, fromX / 2] = Utilities.SetByteValue(this.board[fromY, fromX / 2], (int)Piece.Nothing, fromX % 2 == 0);
            
        }

        public void Skip(int fromY, int fromX, int toY, int toX)
        {
            int avrY = (fromY + toY) / 2;
            int avrX = (fromX + toX) / 2;
            Move(fromY, fromX, toY, toX);
            this.board[avrY, avrX/2] = Utilities.SetByteValue(this.board[avrY, avrX / 2], (int)Piece.Nothing, avrX % 2 == 0);
        }

        public DamkaBoard CloneSkip(int fromY, int fromX, int toY, int toX)
        {
            DamkaBoard newBoard = this.Clone();
            newBoard.Skip(fromY, fromX, toY, toX);
            return newBoard;
        }

        public DamkaBoard CloneSkip(int fromY, int fromX, int toY, int toX, DamkaBoard board)
        {
            DamkaBoard newBoard = board.Clone();
            newBoard.Skip(fromY, fromX, toY, toX);
            return newBoard;
        }

        public DamkaBoard CloneMove(int fromY, int fromX, int toY, int toX)
        {
            DamkaBoard newBoard = this.Clone();
            newBoard.Move(fromY, fromX, toY, toX);
            return newBoard;
        }

        public Piece[,] GetBoard()
        {
            Piece[,] pieceBoard = new Piece[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    pieceBoard[i, j] = GetPieceByIndex(i, j);
                }
            }
            return pieceBoard;
        }
            
        public DamkaBoard[] GetAvailableSkipMoves(int y, int x, DamkaBoard board = null, List<DamkaBoard> moves = null)
        {
            if (moves == null)
            {
                moves = new List<DamkaBoard>();
                board = this;
            }
            Piece piece = board.GetPieceByIndex(y, x);

            int uY;
            int uuY;
            int dY;
            int ddY;
            int lX;
            int llX;
            int rX;
            int rrX;

            if (piece == Piece.BlackPiece)
            {
                dY = y + 1;
                ddY = dY + 1;
                rX = x + 1;
                rrX = rX + 1;
                lX = x - 1;
                llX = lX - 1;

                if (dY < 8)
                {
                    if (lX >= 0)
                    {
                        Piece nextPiece = GetPieceByIndex(dY, lX, board);
                        if (nextPiece == Piece.RedPiece || nextPiece == Piece.RedQueen)
                        {
                            if (ddY < 8 && llX >= 0)
                            {
                                if (GetPieceByIndex(ddY, llX, board) == Piece.Nothing)
                                {
                                    DamkaBoard tmpBoard = CloneSkip(y, x, ddY, llX, board);
                                    moves.Add(tmpBoard);
                                    GetAvailableSkipMoves(ddY, llX, tmpBoard, moves);
                                }
                            }
                        }
                    }

                    if (rX < 8)
                    {
                        Piece nextPiece = GetPieceByIndex(dY, rX, board);
                        if (nextPiece == Piece.RedPiece || nextPiece == Piece.RedQueen)
                        {
                            if (ddY < 8 && rrX < 8)
                            {
                                if (GetPieceByIndex(ddY, rrX, board) == Piece.Nothing)
                                {
                                    DamkaBoard tmpBoard = CloneSkip(y, x, ddY, rrX, board);
                                    moves.Add(tmpBoard);
                                    GetAvailableSkipMoves(ddY, rrX, tmpBoard, moves);
                                }
                            }
                        }
                    }
                }
            }

            else if (piece == Piece.RedPiece)
            {
                uY = y - 1;
                uuY = uY - 1;
                rX = x + 1;
                rrX = rX + 1;
                lX = x - 1;
                llX = lX - 1;

                if (uY >= 0)
                {
                    if (lX >= 0)
                    {
                        Piece nextPiece = GetPieceByIndex(uY, lX, board);
                        if (nextPiece == Piece.BlackPiece || nextPiece == Piece.BlackQueen)
                        {
                            if (uuY >= 0 && llX >= 0)
                            {
                                if (GetPieceByIndex(uuY, llX, board) == Piece.Nothing)
                                {
                                    DamkaBoard tmpBoard = CloneSkip(y, x, uuY, llX, board);
                                    moves.Add(tmpBoard);
                                    GetAvailableSkipMoves(uuY, llX, tmpBoard, moves);
                                }
                            }
                        }
                    }

                    if (rX < 8)
                    {
                        Piece nextPiece = GetPieceByIndex(uY, rX, board);
                        if (nextPiece == Piece.BlackPiece || nextPiece == Piece.BlackQueen)
                        {
                            if (uuY >= 0 && rrX < 8)
                            {
                                if (GetPieceByIndex(uuY, rrX, board) == Piece.Nothing)
                                {
                                    DamkaBoard tmpBoard = CloneSkip(y, x, uuY, rrX, board);
                                    moves.Add(tmpBoard);
                                    GetAvailableSkipMoves(uuY, rrX, tmpBoard, moves);
                                }
                            }
                        }
                    }
                }
            }

            else if (piece == Piece.BlackQueen || piece == Piece.RedQueen)
            {
                uY = y - 1;
                uuY = uY - 1;
                dY = y + 1;
                ddY = dY + 1;
                rX = x + 1;
                rrX = rX + 1;
                lX = x - 1;
                llX = lX - 1;

                if (uY >= 0)
                {
                    if (lX >= 0)
                    {
                        Piece nextPiece = GetPieceByIndex(uY, lX, board);
                        if (piece == Piece.BlackQueen? nextPiece == Piece.RedPiece || nextPiece == Piece.RedQueen : nextPiece == Piece.BlackPiece || nextPiece == Piece.BlackQueen)
                        {
                            if (uuY >= 0 && llX >= 0)
                            {
                                if (GetPieceByIndex(uuY, llX, board) == Piece.Nothing)
                                {
                                    DamkaBoard tmpBoard = CloneSkip(y, x, uuY, llX, board);
                                    moves.Add(tmpBoard);
                                    GetAvailableSkipMoves(uuY, llX, tmpBoard, moves);
                                }
                            }
                        }
                    }

                    if (rX < 8)
                    {
                        Piece nextPiece = GetPieceByIndex(uY, rX, board);
                        if (piece == Piece.BlackQueen ? nextPiece == Piece.RedPiece || nextPiece == Piece.RedQueen : nextPiece == Piece.BlackPiece || nextPiece == Piece.BlackQueen)
                        {
                            if (uuY >= 0 && rrX < 8)
                            {
                                if (GetPieceByIndex(uuY, rrX, board) == Piece.Nothing)
                                {
                                    DamkaBoard tmpBoard = CloneSkip(y, x, uuY, rrX, board);
                                    moves.Add(tmpBoard);
                                    GetAvailableSkipMoves(uuY, rrX, tmpBoard, moves);
                                }
                            }
                        }
                    }
                }
                if (dY < 8)
                {
                    if (lX >= 0)
                    {
                        Piece nextPiece = GetPieceByIndex(dY, lX, board);
                        if (piece == Piece.BlackQueen ? nextPiece == Piece.RedPiece || nextPiece == Piece.RedQueen : nextPiece == Piece.BlackPiece || nextPiece == Piece.BlackQueen)
                        {
                            if (ddY < 8 && llX >= 0)
                            {
                                if (GetPieceByIndex(ddY, llX, board) == Piece.Nothing)
                                {
                                    DamkaBoard tmpBoard = CloneSkip(y, x, ddY, llX, board);
                                    moves.Add(tmpBoard);
                                    GetAvailableSkipMoves(ddY, llX, tmpBoard, moves);
                                }
                            }
                        }
                    }

                    if (rX < 8)
                    {
                        Piece nextPiece = GetPieceByIndex(dY, rX, board);
                        if (piece == Piece.BlackQueen ? nextPiece == Piece.RedPiece || nextPiece == Piece.RedQueen : nextPiece == Piece.BlackPiece || nextPiece == Piece.BlackQueen)
                        {
                            if (ddY < 8 && rrX < 8)
                            {
                                if (GetPieceByIndex(ddY, rrX, board) == Piece.Nothing)
                                {
                                    DamkaBoard tmpBoard = CloneSkip(y, x, ddY, rrX, board);
                                    moves.Add(tmpBoard);
                                    GetAvailableSkipMoves(ddY, rrX, tmpBoard, moves);
                                }
                            }
                        }
                    }
                }
            }

            return moves.ToArray();
        }
        public DamkaBoard[] GetAvailableNormalMoves(int y, int x)
        {
            Piece piece = GetPieceByIndex(y, x);
            List<DamkaBoard> moves = new List<DamkaBoard>();
            int nextY;
            int uY;
            int dY;
            int lX;
            int rX;

            if (piece == Piece.BlackPiece)
            {
                nextY = y + 1;
                rX = x + 1;
                lX = x - 1;
                if (nextY < 8)
                {
                    if (lX >= 0)
                    {
                        if (GetPieceByIndex(nextY, lX) == Piece.Nothing)
                        {
                            moves.Add(CloneMove(y, x, nextY, lX));
                        }
                    }

                    if (rX < 8)
                    {
                        if (GetPieceByIndex(nextY, rX) == Piece.Nothing)
                        {
                            moves.Add(CloneMove(y, x, nextY, rX));
                        }
                    }
                }
            }

            else if (piece == Piece.RedPiece)
            {
                nextY = y - 1;
                rX = x + 1;
                lX = x - 1;
                if (nextY >= 0)
                {
                    if (lX >= 0)
                    {
                        if (GetPieceByIndex(nextY, lX) == Piece.Nothing)
                        {
                            moves.Add(CloneMove(y, x, nextY, lX));
                        }
                    }

                    if (rX < 8)
                    {
                        if (GetPieceByIndex(nextY, rX) == Piece.Nothing)
                        {
                            moves.Add(CloneMove(y, x, nextY, rX));
                        }
                    }
                }
            }

            else if (piece == Piece.BlackQueen || piece == Piece.RedQueen)
            {
                uY = y - 1;
                dY = y + 1;
                rX = x + 1;
                lX = x - 1;

                if (uY >= 0)
                {
                    if (lX >= 0)
                    {
                        if (GetPieceByIndex(uY, lX) == Piece.Nothing)
                        {
                            moves.Add(CloneMove(y, x, uY, lX));
                        }
                    }

                    if (rX < 8)
                    {
                        if (GetPieceByIndex(uY, rX) == Piece.Nothing)
                        {
                            moves.Add(CloneMove(y, x, uY, rX));
                        }
                    }
                }
                if (dY < 8)
                {
                    if (lX >= 0)
                    {
                        if (GetPieceByIndex(dY, lX) == Piece.Nothing)
                        {
                            moves.Add(CloneMove(y, x, dY, lX));
                        }
                    }

                    if (rX < 8)
                    {
                        if (GetPieceByIndex(dY, rX) == Piece.Nothing)
                        {
                            moves.Add(CloneMove(y, x, dY, rX));
                        }
                    }
                }
            }
            
            return moves.ToArray();
        }
    }
    enum Winner : byte
    {
        Red,
        Black,
        NoOne
    }

    enum Piece : byte
    {
        Nothing,
        BlackPiece,
        BlackQueen,
        RedPiece,
        RedQueen
    }
}