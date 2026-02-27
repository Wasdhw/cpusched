using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp3
{
    public partial class FormFormula : Form
    {
        public FormFormula(string algorithmName)
        {
            InitializeComponent();


            if (lblDesc != null)
            {
                lblDesc.AutoSize = false;
                lblDesc.Size = new Size(this.Width - 40, 60); 
            }

            SetAlgorithmContent(algorithmName);
        }

        private void SetAlgorithmContent(string algo)
        {
            lblAlgoTitle.Text = algo + " Scheduling Algorithm";

            switch (algo)
            {
                case "FCFS":
                    lblDesc.Text = "FCFS is the simplest scheduling algorithm, executing processes strictly in the order they arrive (FIFO).  It is non-preemptive, easy to implement, and ensures no starvation.  However, it suffers from high average waiting time, especially if a long process arrives first (the convoy effect), making it inefficient for interactive systems.";
                    lblFormula.Text = "TAT = Completion Time - Arrival Time\r\nWT = TAT - Burst Time";
                    break;
                case "SJF":
                    lblDesc.Text = "SJF minimizes average waiting time by prioritizing the process with the shortest burst time.  It can be non-preemptive or preemptive (SRTF).  While optimal for minimizing wait time, it requires knowledge of burst times in advance (often impractical) and can cause starvation for longer processes if short jobs keep arriving.";
                    lblFormula.Text = "WT = Start Time - Arrival Time";
                    break;
                case "Priority":
                    lblDesc.Text = "Priority Scheduling in Operating Systems is a fundamental CPU scheduling algorithm where each process is assigned a priority, and the operating system selects the highest-priority process to execute next.  Higher-priority processes are executed before lower-priority ones, ensuring critical or urgent tasks receive timely attention.";
                    lblFormula.Text = "Sort By: Priority Ascending";
                    break;
                case "Round Robin":
                    lblDesc.Text = "Round Robin (RR) is a preemptive CPU scheduling algorithm designed to ensure fairness and responsiveness in time-sharing systems.  It operates by assigning a fixed time interval, known as a time quantum (or time slice), to each process in the ready queue.";
                    lblFormula.Text = "Quantum = Defined by user input";
                    break;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormFormula_Load(object sender, EventArgs e)
        {
        }
    }
}