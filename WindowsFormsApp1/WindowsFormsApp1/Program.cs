using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KTMT
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm()); // Chạy Form chính
        }
    }
}


namespace KTMT
{
    public partial class MainForm : Form
    {
        private Queue<int> frames; // Dùng cho FIFO
        private List<int> frameList; // Dùng cho LRU
        private int frameSize = 3; // Số khung trang
        private int[] pageAccessSequence = { 7, 0, 1, 2, 0, 3, 0, 4, 2, 3, 0, 3, 2 }; // Chuỗi truy cập trang

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnRunFIFO_Click(object sender, EventArgs e)
        {
            RunFIFO();
        }

        private void btnRunLRU_Click(object sender, EventArgs e)
        {
            RunLRU();
        }

        private void RunFIFO()
        {
            frames = new Queue<int>();
            listBoxPageFaults.Items.Clear();
            int pageFaults = 0;

            foreach (var page in pageAccessSequence)
            {
                if (!frames.Contains(page))
                {
                    if (frames.Count == frameSize)
                    {
                        frames.Dequeue(); // Xóa trang đầu tiên (FIFO)
                    }
                    frames.Enqueue(page);
                    pageFaults++;
                    listBoxPageFaults.Items.Add($"Page Fault: Truy cập {page} -> {string.Join(", ", frames)}");
                }
                else
                {
                    listBoxPageFaults.Items.Add($"Hit: Truy cập {page} -> {string.Join(", ", frames)}");
                }
                UpdateRAMSwap(frames.Count, pageFaults);
            }

            MessageBox.Show($"FIFO hoàn thành với {pageFaults} lỗi trang.", "Kết quả");
        }

        private void RunLRU()
        {
            frameList = new List<int>();
            listBoxPageFaults.Items.Clear();
            int pageFaults = 0;

            foreach (var page in pageAccessSequence)
            {
                if (!frameList.Contains(page))
                {
                    if (frameList.Count == frameSize)
                    {
                        frameList.RemoveAt(0); // Xóa trang ít được sử dụng nhất (LRU)
                    }
                    frameList.Add(page);
                    pageFaults++;
                    listBoxPageFaults.Items.Add($"Page Fault: Truy cập {page} -> {string.Join(", ", frameList)}");
                }
                else
                {
                    frameList.Remove(page);
                    frameList.Add(page); // Đưa trang vừa truy cập lên cuối
                    listBoxPageFaults.Items.Add($"Hit: Truy cập {page} -> {string.Join(", ", frameList)}");
                }
                UpdateRAMSwap(frameList.Count, pageFaults);
            }

            MessageBox.Show($"LRU hoàn thành với {pageFaults} lỗi trang.", "Kết quả");
        }
     // Hàm trả về phần trăm RAM sử dụng
        private int GetRAMPercent(int ramUsed)
        {
            return (int)((ramUsed * 100.0) / frameSize);
        }

        // Hàm trả về phần trăm Swap sử dụng
        private int GetSwapPercent(int pageFaults)
        {
            int swapUsed = pageFaults > frameSize ? pageFaults - frameSize : 0;
            int maxSwap = pageAccessSequence.Length - frameSize;
            return maxSwap > 0 ? (int)((swapUsed * 100.0) / maxSwap) : 0;
        }

        // Hàm cập nhật progress bar và label
        private void UpdateRAMSwap(int ramUsed, int pageFaults)
        {
            int ramPercent = GetRAMPercent(ramUsed);
            int swapPercent = GetSwapPercent(pageFaults);

            progressBarRAM.Value = ramPercent;
            labelRAM.Text = $"RAM: {ramPercent}%";

            progressBarSwap.Value = swapPercent;
            labelSwap.Text = $"Swap: {swapPercent}%";
        }
    }
}
namespace   KTMT 
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ProgressBar progressBarRAM;
        private System.Windows.Forms.ProgressBar progressBarSwap;
        private System.Windows.Forms.Label labelRAM;
        private System.Windows.Forms.Label labelSwap;
        private System.Windows.Forms.ListBox listBoxPageFaults;
        private System.Windows.Forms.Button btnRunFIFO;
        private System.Windows.Forms.Button btnRunLRU;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.progressBarRAM = new System.Windows.Forms.ProgressBar();
            this.progressBarSwap = new System.Windows.Forms.ProgressBar();
            this.labelRAM = new System.Windows.Forms.Label();
            this.labelSwap = new System.Windows.Forms.Label();
            this.listBoxPageFaults = new System.Windows.Forms.ListBox();
            this.btnRunFIFO = new System.Windows.Forms.Button();
            this.btnRunLRU = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // ProgressBar RAM
            this.progressBarRAM.Location = new System.Drawing.Point(20, 40);
            this.progressBarRAM.Name = "progressBarRAM";
            this.progressBarRAM.Size = new System.Drawing.Size(300, 23);

            // ProgressBar Swap
            this.progressBarSwap.Location = new System.Drawing.Point(20, 100);
            this.progressBarSwap.Name = "progressBarSwap";
            this.progressBarSwap.Size = new System.Drawing.Size(300, 23);

            // Label RAM
            this.labelRAM.AutoSize = true;
            this.labelRAM.Location = new System.Drawing.Point(20, 20);
            this.labelRAM.Name = "labelRAM";
            this.labelRAM.Size = new System.Drawing.Size(70, 15);
            this.labelRAM.Text = "RAM: 0%";

            // Label Swap
            this.labelSwap.AutoSize = true;
            this.labelSwap.Location = new System.Drawing.Point(20, 80);
            this.labelSwap.Name = "labelSwap";
            this.labelSwap.Size = new System.Drawing.Size(74, 15);
            this.labelSwap.Text = "Swap: 0%";

            // ListBox Page Faults
            this.listBoxPageFaults.FormattingEnabled = true;
            this.listBoxPageFaults.ItemHeight = 15;
            this.listBoxPageFaults.Location = new System.Drawing.Point(20, 140);
            this.listBoxPageFaults.Name = "listBoxPageFaults";
            this.listBoxPageFaults.Size = new System.Drawing.Size(300, 109);

            // Button Run FIFO
            this.btnRunFIFO.Location = new System.Drawing.Point(20, 260);
            this.btnRunFIFO.Name = "btnRunFIFO";
            this.btnRunFIFO.Size = new System.Drawing.Size(140, 23);
            this.btnRunFIFO.Text = "Chạy FIFO";
            this.btnRunFIFO.UseVisualStyleBackColor = true;
            this.btnRunFIFO.Click += new System.EventHandler(this.btnRunFIFO_Click);

            // Button Run LRU
            this.btnRunLRU.Location = new System.Drawing.Point(180, 260);
            this.btnRunLRU.Name = "btnRunLRU";
            this.btnRunLRU.Size = new System.Drawing.Size(140, 23);
            this.btnRunLRU.Text = "Chạy LRU";
            this.btnRunLRU.UseVisualStyleBackColor = true;
            this.btnRunLRU.Click += new System.EventHandler(this.btnRunLRU_Click);

            // MainForm
            this.ClientSize = new System.Drawing.Size(350, 300);
            this.Controls.Add(this.progressBarRAM);
            this.Controls.Add(this.progressBarSwap);
            this.Controls.Add(this.labelRAM);
            this.Controls.Add(this.labelSwap);
            this.Controls.Add(this.listBoxPageFaults);
            this.Controls.Add(this.btnRunFIFO);
            this.Controls.Add(this.btnRunLRU);
            this.Name = "MainForm";
            this.Text = "Mô phỏng thuật toán thay thế trang";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}