using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp3
{
    public partial class Form1 : Form
    {
        List<Process> processes = new List<Process>();
        Random rng = new Random();

        public Form1()
        {
            InitializeComponent();
            InitializeGrid();
            WireEvents();

            flpGantt.WrapContents = false;
            flpGantt.AutoScroll = true;
        }

        private void WireEvents()
        {
            btnAdd.Click += BtnAdd_Click;

            // --- BUTTON CLICKS ---
            btnFCFS.Click += async (s, e) =>
            {
                ShowFormulaOnRight("FCFS");
                await RunScheduling("FCFS");
            };

            btnSJF.Click += async (s, e) =>
            {
                ShowFormulaOnRight("SJF");
                await RunScheduling("SJF");
            };

            btnPriority.Click += async (s, e) =>
            {
                ShowFormulaOnRight("Priority");
                await RunScheduling("Priority");
            };

            btnRR.Click += async (s, e) =>
            {
                ShowFormulaOnRight("RR");
                await RunRoundRobin();
            };

            btnClear.Click += (s, e) =>
            {
                processes.Clear();
                flpGantt.Controls.Clear();
                RefreshGrid();
                UpdateAverages(); 
            };
        }

        private void ShowFormulaOnRight(string algorithm)
        {
            using (FormFormula info = new FormFormula(algorithm))
            {
                info.StartPosition = FormStartPosition.Manual;
                int gap = 20; 
                int x = this.Location.X + this.Width + gap;
                int y = this.Location.Y;
                info.Location = new Point(x, y);
                info.ShowDialog();
            }
        }

        private void InitializeGrid()
        {
            dgvProcess.Columns.Clear();
            dgvProcess.Columns.Add("PID", "PID");
            dgvProcess.Columns.Add("AT", "Arrival");
            dgvProcess.Columns.Add("BT", "Burst");
            dgvProcess.Columns.Add("Prio", "Priority");
            dgvProcess.Columns.Add("WT", "Waiting");
            dgvProcess.Columns.Add("TAT", "Turnaround");
            dgvProcess.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtArrival.Text, out int at) || !int.TryParse(txtBurst.Text, out int bt))
            {
                MessageBox.Show("Please enter valid numbers for Arrival and Burst.");
                return;
            }

            int prio = 0;
            var prioControl = this.Controls.Find("txtPriority", true).FirstOrDefault();
            if (prioControl != null) int.TryParse(prioControl.Text, out prio);

            processes.Add(new Process
            {
                PID = string.IsNullOrWhiteSpace(txtPID.Text) ? $"P{processes.Count + 1}" : txtPID.Text,
                ArrivalTime = at,
                BurstTime = bt,
                Priority = prio,
                RemainingTime = bt,
                Color = Color.FromArgb(rng.Next(100, 255), rng.Next(100, 255), rng.Next(100, 255))
            });

            RefreshGrid();
            txtPID.Text = ""; txtArrival.Text = ""; txtBurst.Text = "";
            if (prioControl != null) prioControl.Text = "";
        }

        private async Task RunScheduling(string algorithm)
        {
            flpGantt.Controls.Clear();
            ResetStats();
            int currentTime = 0;
            var remaining = new List<Process>(processes);

            while (remaining.Count > 0)
            {
                var available = remaining.Where(p => p.ArrivalTime <= currentTime).ToList();
                if (!available.Any()) { currentTime++; continue; }

                Process current = algorithm switch
                {
                    "FCFS" => available.OrderBy(p => p.ArrivalTime).First(),
                    "SJF" => available.OrderBy(p => p.BurstTime).First(),
                    "Priority" => available.OrderBy(p => p.Priority).First(),
                    _ => available.First()
                };

                current.WaitingTime = currentTime - current.ArrivalTime;
                current.TurnaroundTime = current.WaitingTime + current.BurstTime;

                await DrawGanttBlock(current.PID, current.BurstTime, current.Color, currentTime);
                currentTime += current.BurstTime;
                remaining.Remove(current);
                RefreshGrid();
            }
            UpdateAverages(); 
        }

        private async Task RunRoundRobin()
        {
            flpGantt.Controls.Clear();
            ResetStats();
            int quantum = 2;
            var qControl = this.Controls.Find("txtQuantum", true).FirstOrDefault();
            if (qControl != null) int.TryParse(qControl.Text, out quantum);

            int currentTime = 0;
            Queue<Process> queue = new Queue<Process>();
            var unarrived = processes.OrderBy(p => p.ArrivalTime).ToList();

            while (queue.Any() || unarrived.Any())
            {
                while (unarrived.Any() && unarrived.First().ArrivalTime <= currentTime)
                {
                    queue.Enqueue(unarrived.First());
                    unarrived.RemoveAt(0);
                }

                if (!queue.Any()) { currentTime++; continue; }

                var p = queue.Dequeue();
                int slice = Math.Min(p.RemainingTime, quantum);
                await DrawGanttBlock(p.PID, slice, p.Color, currentTime);

                currentTime += slice;
                p.RemainingTime -= slice;

                while (unarrived.Any() && unarrived.First().ArrivalTime <= currentTime)
                {
                    queue.Enqueue(unarrived.First());
                    unarrived.RemoveAt(0);
                }

                if (p.RemainingTime > 0) queue.Enqueue(p);
                else
                {
                    p.TurnaroundTime = currentTime - p.ArrivalTime;
                    p.WaitingTime = p.TurnaroundTime - p.BurstTime;
                }
                RefreshGrid();
            }
            UpdateAverages(); 
        }


        private void UpdateAverages()
        {
            if (processes.Count == 0) return;

            double avgWT = processes.Average(p => p.WaitingTime);
            double avgTAT = processes.Average(p => p.TurnaroundTime);

            Control lblAvg = this.Controls.Find("lblAverages", true).FirstOrDefault();

            if (lblAvg != null)
            {
                lblAvg.Text = $"Avg Waiting Time: {avgWT:F2}ms | Avg Turnaround Time: {avgTAT:F2}ms";
            }
        }

        private async Task DrawGanttBlock(string pid, int duration, Color color, int time)
        {
            Panel block = new Panel
            {
                Width = duration * 30,
                Height = flpGantt.Height - 30,
                BackColor = color,
                Margin = new Padding(0, 5, 0, 5),
                BorderStyle = BorderStyle.FixedSingle
            };

            block.Controls.Add(new Label { Text = pid, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
            block.Controls.Add(new Label { Text = time.ToString(), Dock = DockStyle.Bottom, Height = 15, Font = new Font("Consolas", 7) });

            flpGantt.Controls.Add(block);
            flpGantt.ScrollControlIntoView(block);
            await Task.Delay(500);
        }

        private void ResetStats() { processes.ForEach(p => { p.RemainingTime = p.BurstTime; p.WaitingTime = 0; p.TurnaroundTime = 0; }); }

        private void RefreshGrid() { dgvProcess.Rows.Clear(); processes.ForEach(p => dgvProcess.Rows.Add(p.PID, p.ArrivalTime, p.BurstTime, p.Priority, p.WaitingTime, p.TurnaroundTime)); }

        private void exit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }

    public class Process
    {
        public string PID { get; set; } = "";
        public int ArrivalTime { get; set; }
        public int BurstTime { get; set; }
        public int Priority { get; set; }
        public int RemainingTime { get; set; }
        public int WaitingTime { get; set; }
        public int TurnaroundTime { get; set; }
        public Color Color { get; set; }
    }
}