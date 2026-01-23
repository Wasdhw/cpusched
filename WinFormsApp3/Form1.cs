using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp3
{
    public partial class Form1 : Form
    {
        List<Process> processes = new List<Process>();

        public Form1()
        {
            InitializeComponent();
            InitializeGrid();
            WireEvents();
        }

        private void WireEvents()
        {
            btnAdd.Click += BtnAdd_Click;
            btnFCFS.Click += BtnFCFS_Click;
        }

        private void InitializeGrid()
        {
            dgvProcess.Columns.Clear();
            dgvProcess.Columns.Add("PID", "PID");
            dgvProcess.Columns.Add("AT", "Arrival");
            dgvProcess.Columns.Add("BT", "Burst");
            dgvProcess.Columns.Add("WT", "Waiting");
            dgvProcess.Columns.Add("TAT", "Turnaround");

            dgvProcess.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProcess.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProcess.ReadOnly = true;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtArrival.Text, out int at) ||
                !int.TryParse(txtBurst.Text, out int bt))
            {
                MessageBox.Show("Arrival and Burst must be numbers");
                return;
            }

            processes.Add(new Process
            {
                PID = txtPID.Text,
                ArrivalTime = at,
                BurstTime = bt
            });

            RefreshGrid();
            txtPID.Clear();
            txtArrival.Clear();
            txtBurst.Clear();
        }

        private void BtnFCFS_Click(object sender, EventArgs e)
        {
            flpGantt.Controls.Clear();

            int time = 0;

            foreach (var p in processes.OrderBy(x => x.ArrivalTime))
            {
                if (time < p.ArrivalTime)
                    time = p.ArrivalTime;

                p.WaitingTime = time - p.ArrivalTime;
                p.TurnaroundTime = p.WaitingTime + p.BurstTime;

                DrawGanttBlock(p.PID, p.BurstTime);

                time += p.BurstTime;
            }

            RefreshGrid();
        }

        private void DrawGanttBlock(string pid, int burst)
        {
            Panel block = new Panel
            {
                Width = burst * 30,
                Height = 60,
                BackColor = Color.MediumSeaGreen,
                Margin = new Padding(5)
            };

            Label lbl = new Label
            {
                Text = pid,
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            block.Controls.Add(lbl);
            flpGantt.Controls.Add(block);
        }

        private void RefreshGrid()
        {
            dgvProcess.Rows.Clear();
            foreach (var p in processes)
            {
                dgvProcess.Rows.Add(
                    p.PID,
                    p.ArrivalTime,
                    p.BurstTime,
                    p.WaitingTime,
                    p.TurnaroundTime
                );
            }
        }

    }

    class Process
    {
        public string PID { get; set; }
        public int ArrivalTime { get; set; }
        public int BurstTime { get; set; }
        public int WaitingTime { get; set; }
        public int TurnaroundTime { get; set; }
    }
}
