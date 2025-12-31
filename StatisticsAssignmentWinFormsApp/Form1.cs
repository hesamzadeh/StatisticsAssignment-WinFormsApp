using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace StatisticsAssignmentWinFormsApp
{
    public partial class Form1 : Form
    {
        private readonly int[] _data =
        {
            1,3,0,8,1,2,4,5,9,3,
            1,0,0,0,6,5,1,4,6,2,
            0,1,7,8,0,2,4,3,0,1
        };

        private bool _isFarsi = true;

        private TextBox txtData;
        private TextBox txtStats;
        private TextBox txtVariableType;
        private DataGridView dgvFrequency;
        private Chart chart;
        private Button btnLang;

        public Form1()
        {
            InitializeComponent();
            BuildUI();
            ApplyFarsi();
        }

        private void BuildUI()
        {
            this.Size = new Size(1100, 900);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 6
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));   // button
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 90));   // raw data
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 140));  // variable type
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 180));  // frequency table
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 200));  // stats
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // chart

            this.Controls.Add(layout);

            btnLang = new Button { Dock = DockStyle.Right, Width = 120 };
            btnLang.Click += (s, e) =>
            {
                _isFarsi = !_isFarsi;
                if (_isFarsi) ApplyFarsi(); else ApplyEnglish();
            };
            layout.Controls.Add(btnLang, 0, 0);

            txtData = CreateTextBox();
            layout.Controls.Add(txtData, 0, 1);

            txtVariableType = CreateTextBox();
            layout.Controls.Add(txtVariableType, 0, 2);

            dgvFrequency = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            layout.Controls.Add(dgvFrequency, 0, 3);

            txtStats = CreateTextBox();
            layout.Controls.Add(txtStats, 0, 4);

            chart = CreateChart();
            layout.Controls.Add(chart, 0, 5);
        }

        private TextBox CreateTextBox()
        {
            return new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Tahoma", 10)
            };
        }

        // ================= FARSI =================

        private void ApplyFarsi()
        {
            this.Text = "تحلیل آماری تصادفات چهارراه یاس";
            btnLang.Text = "English";

            txtData.RightToLeft = RightToLeft.Yes;
            txtData.Text =
                "داده‌های خام:\r\n" +
                string.Join(" ، ", _data) +
                "\r\n\r\nداده‌های مرتب‌شده (صعودی):\r\n" +
                string.Join(" ، ", _data.OrderBy(x => x));

            txtVariableType.RightToLeft = RightToLeft.Yes;
            txtVariableType.Text =
                "نوع متغیر و دلایل:\r\n" +
                "• از نظر ماهیت: کمی (عددی)\r\n" +
                "• از نظر پیوستگی: گسسته (فقط اعداد صحیح)\r\n" +
                "• از نظر مقیاس اندازه‌گیری: نسبتی (دارای صفر واقعی)\r\n" +
                "• از نظر نقش آماری: متغیر تصادفی\r\n" +
                "دلیل: مقدار متغیر حاصل شمارش تعداد تصادفات در هر روز است.";

            FillFrequencyTable("تعداد تصادف", "فراوانی");

            txtStats.RightToLeft = RightToLeft.Yes;
            txtStats.Text = GetStatsFarsi();

            chart.Titles.Clear();
            chart.Titles.Add("نمودار توزیع فراوانی تصادفات");
            chart.ChartAreas[0].AxisX.Title = "تعداد تصادف";
            chart.ChartAreas[0].AxisY.Title = "فراوانی";
        }

        // ================= ENGLISH =================

        private void ApplyEnglish()
        {
            this.Text = "Statistics of Traffic Accidents";
            btnLang.Text = "فارسی";

            txtData.RightToLeft = RightToLeft.No;
            txtData.Text =
                "Raw Data:\r\n" +
                string.Join(", ", _data) +
                "\r\n\r\nSorted Data (Ascending):\r\n" +
                string.Join(", ", _data.OrderBy(x => x));

            txtVariableType.RightToLeft = RightToLeft.No;
            txtVariableType.Text =
                "Variable Type:\r\n" +
                "- Quantitative\r\n" +
                "- Discrete\r\n" +
                "- Ratio scale (true zero)\r\n" +
                "- Random variable";

            FillFrequencyTable("Accidents", "Frequency");

            txtStats.RightToLeft = RightToLeft.No;
            txtStats.Text = GetStatsEnglish();

            chart.Titles.Clear();
            chart.Titles.Add("Frequency Distribution of Accidents");
            chart.ChartAreas[0].AxisX.Title = "Number of Accidents";
            chart.ChartAreas[0].AxisY.Title = "Frequency";
        }

        // ================= FREQUENCY TABLE =================

        private void FillFrequencyTable(string colXi, string colFi)
        {
            dgvFrequency.Columns.Clear();
            dgvFrequency.Rows.Clear();

            dgvFrequency.Columns.Add(colXi, colXi);          // xi
            dgvFrequency.Columns.Add(colFi, colFi);          // fi (فراوانی غیر نسبی)
            dgvFrequency.Columns.Add("ri", "ri (فراوانی نسبی)");  // فراوانی نسبی
            dgvFrequency.Columns.Add("gi", "gi (فراوانی تجمعی)");  // فراوانی تجمعی
            dgvFrequency.Columns.Add("si", "si (فراوانی تجمعی نسبی)");  // فراوانی تجمعی نسبی

            var freqGroups = _data.GroupBy(x => x)
                                  .OrderBy(g => g.Key)
                                  .Select(g => new { Value = g.Key, Frequency = g.Count() })
                                  .ToList();

            int total = _data.Length;
            int cumulativeFreq = 0;
            double cumulativeRelFreq = 0.0;

            foreach (var item in freqGroups)
            {
                cumulativeFreq += item.Frequency;
                double relFreq = (double)item.Frequency / total;
                cumulativeRelFreq += relFreq;

                dgvFrequency.Rows.Add(
                    item.Value,                           // xi
                    item.Frequency,                       // fi
                    relFreq.ToString("P2"),               // ri (مثلاً 23.33%)
                    cumulativeFreq,                      // gi
                    cumulativeRelFreq.ToString("P2")     // si
                );
            }
        }



        // ================= STATISTICS =================

        private string GetStatsFarsi()
        {
            double mean = _data.Average();
            double variance = _data.Sum(x => Math.Pow(x - mean, 2)) / (_data.Length - 1);
            double std = Math.Sqrt(variance);

            return
                "گزارش آماری:\r\n" +
                $"تعداد داده‌ها: {_data.Length}\r\n" +
                $"میانگین: {mean:F2}\r\n" +
                $"میانه: {Median()}\r\n" +
                $"نما: {Mode()}\r\n" +
                $"حداقل: {_data.Min()}   حداکثر: {_data.Max()}\r\n" +
                $"دامنه تغییرات: {_data.Max() - _data.Min()}\r\n" +
                $"واریانس: {variance:F2}\r\n" +
                $"انحراف معیار: {std:F2}";
        }

        private string GetStatsEnglish()
        {
            double mean = _data.Average();
            double variance = _data.Sum(x => Math.Pow(x - mean, 2)) / (_data.Length - 1);
            double std = Math.Sqrt(variance);

            return
                $"Count: {_data.Length}\r\n" +
                $"Mean: {mean:F2}\r\n" +
                $"Median: {Median()}\r\n" +
                $"Mode: {Mode()}\r\n" +
                $"Min: {_data.Min()}   Max: {_data.Max()}\r\n" +
                $"Range: {_data.Max() - _data.Min()}\r\n" +
                $"Variance: {variance:F2}\r\n" +
                $"Std Dev: {std:F2}";
        }

        private double Median()
        {
            var s = _data.OrderBy(x => x).ToArray();
            return (s[14] + s[15]) / 2.0;
        }

        private string Mode()
        {
            int max = _data.GroupBy(x => x).Max(g => g.Count());
            return string.Join(", ", _data.GroupBy(x => x)
                                          .Where(g => g.Count() == max)
                                          .Select(g => g.Key));
        }

        // ================= CHART =================

        private Chart CreateChart()
        {
            var c = new Chart { Dock = DockStyle.Fill };
            var area = new ChartArea();
            area.AxisX.Interval = 1;
            c.ChartAreas.Add(area);

            var s = new Series
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true,
                Color = Color.SteelBlue
            };

            foreach (var g in _data.GroupBy(x => x).OrderBy(g => g.Key))
                s.Points.AddXY(g.Key, g.Count());

            c.Series.Add(s);
            return c;
        }
    }
}
