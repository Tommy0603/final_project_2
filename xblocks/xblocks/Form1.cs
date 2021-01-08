using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xblocks
{
    public partial class Form1 : Form
    {
        Label[,] grids = new Label[20, 10];
        Label[,] next = new Label[4, 3];
        bool[,] signs = new bool[24, 10];
        Color[,] grids_color = new Color[24, 10];
        uint block_row = 20;
        uint block_col = 4;
        uint block_type;
        uint block_row_pre = 20;
        uint block_col_pre = 4;
        uint block_type_pre;
        uint block_type_next;
        bool block_changed = false;
        Random rander = new Random(System.DateTime.Now.Millisecond);
        int timer_interval = 1010;
        int game_mode = 1;
        uint block_count = 0;
        uint score = 0;

        public Form1()
        {
            InitializeComponent();
            block_type = (uint)rander.Next(0, 7) + 1;
            block_type_pre = block_type;
            block_type_next = block_type;
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    grids[i, j] = new Label();
                    grids[i, j].Width = 30;
                    grids[i, j].Height = 30;
                    grids[i, j].BorderStyle = BorderStyle.FixedSingle;
                    grids[i, j].BackColor = Color.Black;
                    grids[i, j].Left = 30 + 30 * j;
                    grids[i, j].Top = 600 - i * 30;
                    grids[i, j].Visible = true;
                    this.Controls.Add(grids[i, j]);
                }
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    next[i, j] = new Label();
                    next[i, j].Width = 20;
                    next[i, j].Height = 20;
                    next[i, j].BorderStyle = BorderStyle.None;
                    next[i, j].BackColor = Color.White;
                    next[i, j].Left = 350 + 20 * j;
                    next[i, j].Top = 150 - i * 20;
                    next[i, j].Visible = true;
                    this.Controls.Add(next[i, j]);
                }
            }
            init_game();
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            player.SoundLocation = "Tetris.wav";
            player.Load();
            player.PlayLooping();

        }

        void init_game()
        {
            block_type = (uint)rander.Next(0, 7) + 1;
            block_row = 20;
            block_col = 4;
            block_row_pre = 20;
            block_col_pre = 4;
            block_type_pre = block_type;
            block_type_next = block_type;
            block_changed = false;
            timer_interval = 1010;
            timer1.Interval = timer_interval;
            block_count = 0;
            score = 0;
            game_mode = 1;
            for (uint i = 0; i < 24; i++)
            {
                for (uint j = 0; j < 10; j++)
                {
                    signs[i, j] = false;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (y_direction(block_type, block_row, block_col))
            {
                block_row_pre = block_row;
                block_type_pre = block_type;
                block_row--;
                if (block_row == 19)
                {
                    block_type_next = (uint)rander.Next(0, 7) + 1;
                    display_next_block(block_type_next);
                    block_count++;
                    score += 5;
                    label_block_count.Text = "Blocks:" + block_count.ToString();
                    label_score.Text = "Score:" + score.ToString();
                    if (game_mode == 1)
                    {
                        timer_interval = 1010 - (int)(score / 150) * 50;
                        if (timer_interval <= 0)
                        {
                            timer_interval = 10;
                        }
                        timer1.Interval = timer_interval;
                        label_level.Text = "Level:" + (1010 - timer_interval) / 50;
                    }
                }
                erase_block(block_row_pre, block_col_pre, block_type_pre);
                update_block(block_row, block_col, block_type);
                show_grids();
                block_row_pre = block_row;
                block_changed = false;
            }
            else
            {
                show_grids();
                full_line_check();
                if (block_row == 20)
                {
                    label_info.Text = "Game Over!";
                    button1.Visible = true;
                    button1.Enabled = true;
                    timer1.Enabled = false;
                    return;
                }
                block_type = block_type_next;
                block_row = 20;
                block_col = 4;
                block_row_pre = 20;
                block_col_pre = 4;
                block_type_pre = block_type;
                block_changed = false;
            }
        }

        void full_line_check()
        {
            uint row_sum;
            uint i, j;
            i = 0;
            while (i < 20)
            {
                row_sum = 0;
                for (j = 0; j < 10; j++)
                {
                    if (signs[i, j])
                    {
                        row_sum++;
                    }
                }
                if (row_sum == 10)
                {
                    score += 20;
                    label_score.Text = "Score:" + score.ToString();
                    for (j = 0; j < 10; j++)
                    {
                        signs[i, j] = false;
                    }
                    show_grids();
                    for (uint y = i; y < 21; y++)
                    {
                        for (j = 0; j < 10; j++)
                        {
                            signs[y, j] = signs[y + 1, j];
                            grids_color[y, j] = grids_color[y + 1, j];
                        }
                    }
                    show_grids();
                }
                else i++;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.P)
            {
                if (game_mode == 0)
                {
                    game_mode = 1;
                    timer1.Enabled = true;
                }
                else
                {
                    game_mode = 0;
                    timer1.Enabled = false;
                }
            }
            if (e.KeyCode == Keys.Left)
            {
                if (x_direction(block_type, block_row, block_col, -1))
                {
                    block_col_pre = block_col;
                    block_col--;
                    block_changed = true;
                }
            }
            if (e.KeyCode == Keys.Right)
            {
                if (x_direction(block_type, block_row, block_col, 1))
                {
                    block_col_pre = block_col;
                    block_col++;
                    block_changed = true;
                }
            }
            if (e.KeyCode == Keys.Up)
            {
                block_type_pre = block_type;
                block_col_pre = block_col;
                block_row_pre = block_row;
                block_type = next_block_type(block_type, block_row, block_col);
                if (block_type != block_type_pre)
                {
                    block_changed = true;
                }
            }
            if (e.KeyCode == Keys.S)
            {
                game_mode = 2;
                timer_interval -= 50;
                if (timer_interval <= 0)
                {
                    timer_interval = 1;
                }
                timer1.Interval = timer_interval;
                label_level.Text = "Level:" + (1010 - timer_interval) / 50;
            }
            if (e.KeyCode == Keys.A)
            {
                game_mode = 2;
                timer_interval += 50;
                if (timer_interval >= 1010)
                {
                    timer_interval = 1010;
                }
                timer1.Interval = timer_interval;
                label_level.Text = "Level:" + (1010 - timer_interval) / 50;
            }
            if (e.KeyCode == Keys.Down)
            {
                timer1.Interval = 50;
            }
            if (e.KeyCode == Keys.Space)
            {
                timer1.Interval = 20;
                for (; ; )
                {
                    if (y_direction(block_type, block_row, block_col))
                    {
                        block_row_pre = block_row;
                        block_row--;
                        erase_block(block_row_pre, block_col_pre, block_type_pre);
                        update_block(block_row, block_col, block_type);
                        show_grids();
                        block_row_pre = block_row;
                        block_col_pre = block_col;
                        block_type_pre = block_type;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (block_changed)
            {
                erase_block(block_row_pre, block_col_pre, block_type_pre);
                update_block(block_row, block_col, block_type);
                show_grids();
                block_row_pre = block_row;
                block_col_pre = block_col;
                block_type_pre = block_type;
                block_changed = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            init_game();
            label_info.Text = "";
            button1.Visible = false;
            button1.Enabled = false;
            timer1.Enabled = true;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                timer1.Interval = timer_interval;
            }
            if (e.KeyCode == Keys.Space)
            {
                timer1.Interval = timer_interval;
            }
        }
    }
}
