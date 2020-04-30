using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Damka
{
    public partial class Damka : Form
    {
        Board board;
        public Damka()
        {
            InitializeComponent();
            this.board = new Board(this);
            this.Controls.Add(board);
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }

        public void Button_Click(object sender, EventArgs e)
        {
            Cell clickedCell = sender as Cell;
            if (clickedCell.GetBoard() != null)
            {
                this.board.AppendFromDamkaBoard(clickedCell.GetBoard());
                this.board.ClearMoves();
                return;
            }
            this.board.ClearMoves();
            this.board.ShowMoves(clickedCell);
        }
    }
}
