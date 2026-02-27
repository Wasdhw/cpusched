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
        int processIdCounter = 1;

        string selectedAlgorithm = "";
        bool isRunning = false;

        public Form1()
        {
            InitializeComponent();
            InitializeGrid();
            WireEvents();

            flpGantt.WrapContents = false;
            flpGantt.AutoScroll = true;

            ToggleInputs("");
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
            dgvProcess.AllowUserToAddRows = false;
            dgvProcess.ColumnHeadersVisible = true;
            dgvProcess.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvProcess.ColumnHeadersHeight = 25;
        }

        private void WireEvents()
        {
            btnAdd.Click += BtnAdd_Click;
            btnClear.Click += BtnClear_Click;

            // Algorithm Selection (Since you set ButtonMode to RadioButton, we just track the name)
            btnFCFS.Click += (s, e) => SelectAlgorithm("FCFS");
            btnSJF.Click += (s, e) => SelectAlgorithm("SJF");
            btnPriority.Click += (s, e) => SelectAlgorithm("Priority");
            btnRR.Click += (s, e) => SelectAlgorithm("Round Robin");

            Control? runBtn = this.Controls.Find("btnRun", true).FirstOrDefault();
            if (runBtn != null)
            {
                runBtn.Click += BtnRun_Click;
            }
        }

        private void SelectAlgorithm(string algo)
        {
            selectedAlgorithm = algo;
            ToggleInputs(algo);

            ShowFormulaOnRight(algo);
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

        private void ToggleInputs(string algo)
        {
            Control? txtQ = this.Controls.Find("txtQuantum", true).FirstOrDefault();
            Control? txtP = this.Controls.Find("txtPriority", true).FirstOrDefault();

            if (txtQ != null)
            {
                txtQ.Enabled = (algo == "Round Robin");
                if (!txtQ.Enabled) txtQ.Text = "";
            }

            if (txtP != null)
            {
                txtP.Enabled = (algo == "Priority");
                if (!txtP.Enabled) txtP.Text = "";
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtArrival.Text, out int at) || !int.TryParse(txtBurst.Text, out int bt))
            {
                MessageBox.Show("Please enter valid numbers for Arrival and Burst.");
                return;
            }

            int prio = 0;
            Control? txtP = this.Controls.Find("txtPriority", true).FirstOrDefault();
            if (txtP != null && txtP.Enabled) int.TryParse(txtP.Text, out prio);

            processes.Add(new Process
            {
                PID = $"P{processIdCounter++}",
                ArrivalTime = at,
                BurstTime = bt,
                Priority = prio,
                RemainingTime = bt,
                Color = Color.FromArgb(rng.Next(100, 255), rng.Next(100, 255), rng.Next(100, 255))
            });

            RefreshGrid();
            txtArrival.Text = ""; txtBurst.Text = "";
            if (txtP != null) txtP.Text = "";
        }

        private async void BtnRun_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedAlgorithm))
            {
                MessageBox.Show("Please select an algorithm first.");
                return;
            }

            if (processes.Count == 0) return;
            if (isRunning) return;

            isRunning = true;
            Control? runBtn = sender as Control;
            if (runBtn != null) runBtn.Enabled = false;

            try
            {
                if (selectedAlgorithm == "Round Robin")
                    await RunRoundRobin();
                else
                    await RunScheduling(selectedAlgorithm);
            }
            finally
            {
                isRunning = false;
                if (runBtn != null) runBtn.Enabled = true;
            }
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

                if (!available.Any())
                {
                    int nextArrival = remaining.Min(p => p.ArrivalTime);
                    await DrawGanttBlock("IDLE", nextArrival - currentTime, Color.LightGray, currentTime);
                    currentTime = nextArrival;
                    continue;
                }

                Process current = algorithm switch
                {
                    "FCFS" => available.OrderBy(p => p.ArrivalTime).First(),
                    "SJF" => available.OrderBy(p => p.BurstTime).ThenBy(p => p.ArrivalTime).First(),
                    "Priority" => available.OrderBy(p => p.Priority).ThenBy(p => p.ArrivalTime).First(),
                    _ => available.First()
                };

                current.WaitingTime = currentTime - current.ArrivalTime;
                current.TurnaroundTime = current.WaitingTime + current.BurstTime;

                await DrawGanttBlock(current.PID, current.BurstTime, current.Color, currentTime);
                currentTime += current.BurstTime;
                remaining.Remove(current);
                RefreshGrid();
            }

            // --- NEW: Draws the final total time at the very end of the Gantt Chart ---
            DrawFinalTimeMark(currentTime);
            UpdateAverages();
        }

        private async Task RunRoundRobin()
        {
            flpGantt.Controls.Clear();
            ResetStats();

            int quantum = 2;
            Control? txtQ = this.Controls.Find("txtQuantum", true).FirstOrDefault();
            if (txtQ != null && !string.IsNullOrWhiteSpace(txtQ.Text))
                int.TryParse(txtQ.Text, out quantum);

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

                if (!queue.Any())
                {
                    int nextArrival = unarrived.First().ArrivalTime;
                    await DrawGanttBlock("IDLE", nextArrival - currentTime, Color.LightGray, currentTime);
                    currentTime = nextArrival;
                    continue;
                }

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

                if (p.RemainingTime > 0)
                {
                    queue.Enqueue(p);
                }
                else
                {
                    p.TurnaroundTime = currentTime - p.ArrivalTime;
                    p.WaitingTime = p.TurnaroundTime - p.BurstTime;
                }
                RefreshGrid();
            }

            // --- NEW: Draws the final total time at the very end of the Gantt Chart ---
            DrawFinalTimeMark(currentTime);
            UpdateAverages();
        }

        private void BtnClear_Click(object? sender, EventArgs e)
        {
            processes.Clear();
            processIdCounter = 1;
            flpGantt.Controls.Clear();
            RefreshGrid();
            UpdateAverages();
        }

        private async Task DrawGanttBlock(string pid, int duration, Color color, int time)
        {
            Panel block = new Panel
            {
                Width = duration * 40,
                Height = flpGantt.Height - 30,
                BackColor = color,
                Margin = new Padding(0, 5, 0, 5),
                BorderStyle = BorderStyle.FixedSingle
            };

            block.Controls.Add(new Label { Text = pid, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
            block.Controls.Add(new Label { Text = time.ToString(), Dock = DockStyle.Bottom, Height = 15, Font = new Font("Consolas", 8, FontStyle.Bold), TextAlign = ContentAlignment.BottomLeft, BackColor = Color.Transparent });

            flpGantt.Controls.Add(block);
            flpGantt.ScrollControlIntoView(block);

            await Task.Delay(300);
        }

        // --- NEW METHOD: This adds the transparent number block at the end ---
        private void DrawFinalTimeMark(int time)
        {
            Panel mark = new Panel
            {
                Width = 35, // Wide enough to hold double digits
                Height = flpGantt.Height - 30,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 5, 0, 5)
            };

            mark.Controls.Add(new Label
            {
                Text = time.ToString(),
                Dock = DockStyle.Bottom,
                Height = 15,
                Font = new Font("Consolas", 8, FontStyle.Bold),
                TextAlign = ContentAlignment.BottomLeft,
                BackColor = Color.Transparent
            });

            flpGantt.Controls.Add(mark);
            flpGantt.ScrollControlIntoView(mark);
        }

        private void ResetStats()
        {
            processes.ForEach(p => { p.RemainingTime = p.BurstTime; p.WaitingTime = 0; p.TurnaroundTime = 0; });
        }

        private void RefreshGrid()
        {
            dgvProcess.Rows.Clear();
            processes.ForEach(p => dgvProcess.Rows.Add(p.PID, p.ArrivalTime, p.BurstTime, p.Priority, p.WaitingTime, p.TurnaroundTime));
        }

        private void UpdateAverages()
        {
            Control? lblAvg = this.Controls.Find("lblAverages", true).FirstOrDefault();
            if (lblAvg != null)
            {
                if (processes.Count == 0)
                {
                    lblAvg.Text = "Avg Waiting Time: 0.00ms | Avg Turnaround Time: 0.00ms";
                    return;
                }
                double avgWT = processes.Average(p => p.WaitingTime);
                double avgTAT = processes.Average(p => p.TurnaroundTime);
                lblAvg.Text = $"Avg Waiting Time: {avgWT:F2}ms | Avg Turnaround Time: {avgTAT:F2}ms";
            }
        }

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