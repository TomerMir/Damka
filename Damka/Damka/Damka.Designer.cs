﻿namespace Damka
{
    partial class Damka
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Upper_Menu = new System.Windows.Forms.MenuStrip();
            this.levelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.easyInstantToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.medium36SecondsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hard710SecondsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.superHard2030SecondsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Upper_Menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // Upper_Menu
            // 
            this.Upper_Menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.levelToolStripMenuItem});
            this.Upper_Menu.Location = new System.Drawing.Point(0, 0);
            this.Upper_Menu.Name = "Upper_Menu";
            this.Upper_Menu.Size = new System.Drawing.Size(789, 24);
            this.Upper_Menu.TabIndex = 0;
            // 
            // levelToolStripMenuItem
            // 
            this.levelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.easyInstantToolStripMenuItem,
            this.medium36SecondsToolStripMenuItem,
            this.hard710SecondsToolStripMenuItem,
            this.superHard2030SecondsToolStripMenuItem});
            this.levelToolStripMenuItem.Name = "levelToolStripMenuItem";
            this.levelToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.levelToolStripMenuItem.Text = "Level";
            // 
            // easyInstantToolStripMenuItem
            // 
            this.easyInstantToolStripMenuItem.Name = "easyInstantToolStripMenuItem";
            this.easyInstantToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.easyInstantToolStripMenuItem.Text = "Easy - Instant";
            this.easyInstantToolStripMenuItem.Click += new System.EventHandler(this.EasyInstantToolStripMenuItem_Click);
            // 
            // medium36SecondsToolStripMenuItem
            // 
            this.medium36SecondsToolStripMenuItem.Name = "medium36SecondsToolStripMenuItem";
            this.medium36SecondsToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.medium36SecondsToolStripMenuItem.Text = "Medium - 3-6 seconds";
            this.medium36SecondsToolStripMenuItem.Click += new System.EventHandler(this.Medium36SecondsToolStripMenuItem_Click);
            // 
            // hard710SecondsToolStripMenuItem
            // 
            this.hard710SecondsToolStripMenuItem.Name = "hard710SecondsToolStripMenuItem";
            this.hard710SecondsToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.hard710SecondsToolStripMenuItem.Text = "Hard - 7-10 seconds";
            this.hard710SecondsToolStripMenuItem.Click += new System.EventHandler(this.Hard710SecondsToolStripMenuItem_Click);
            // 
            // superHard2030SecondsToolStripMenuItem
            // 
            this.superHard2030SecondsToolStripMenuItem.Name = "superHard2030SecondsToolStripMenuItem";
            this.superHard2030SecondsToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.superHard2030SecondsToolStripMenuItem.Text = "Super Hard - 15-30 seconds";
            this.superHard2030SecondsToolStripMenuItem.Click += new System.EventHandler(this.SuperHard2030SecondsToolStripMenuItem_Click);
            // 
            // Damka
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 791);
            this.Controls.Add(this.Upper_Menu);
            this.MainMenuStrip = this.Upper_Menu;
            this.Name = "Damka";
            this.Text = "Checkers";
            this.Upper_Menu.ResumeLayout(false);
            this.Upper_Menu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip Upper_Menu;
        private System.Windows.Forms.ToolStripMenuItem levelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem easyInstantToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem medium36SecondsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hard710SecondsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem superHard2030SecondsToolStripMenuItem;
    }
}

