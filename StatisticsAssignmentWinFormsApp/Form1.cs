using System.Windows.Forms.DataVisualization.Charting;

namespace StatisticsAssignmentWinFormsApp
{
    public partial class Form1 : Form
    {
        // Dataset
        private readonly int[] _data =
        {
            1, 3, 0, 8, 1, 2, 4, 5, 9, 3,
            1, 0, 0, 0, 6, 5, 1, 4, 6, 2,
            0, 1, 7, 8, 0, 2, 4, 3, 0, 1
        };

        private bool _isFarsi = false;

        private TextBox _txtStats;
        private TextBox _txtData;
        private Chart _chart;
        private Button _btnLanguage;

        public Form1()
        {
            InitializeComponent();
            SetupCustomInterface();
        }

        private void SetupCustomInterface()
        {
            this.Size = new Size(1000, 850);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F)); // Button
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F)); // Data
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));  // Stats
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));  // Chart

            this.Controls.Add(layout);

            // Language Button
            _btnLanguage = new Button
            {
                Text = "فارسی",
                Dock = DockStyle.Right,
                Width = 120
            };
            _btnLanguage.Click += ToggleLanguage;
            layout.Controls.Add(_btnLanguage, 0, 0);

            // Raw Data Display
            _txtData = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10),
                BackColor = Color.White
            };
            layout.Controls.Add(_txtData, 0, 1);

            // Statistics Output
            _txtStats = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10)
            };
            layout.Controls.Add(_txtStats, 0, 2);

            // Chart
            _chart = CreateChart();
            layout.Controls.Add(_chart, 0, 3);

            ApplyEnglish();
        }

        // ================= LANGUAGE SWITCH =================

        private void ToggleLanguage(object sender, EventArgs e)
        {
            _isFarsi = !_isFarsi;

            if (_isFarsi)
                ApplyFarsi();
            else
                ApplyEnglish();
        }

        // ================= ENGLISH =================

        private void ApplyEnglish()
        {
            this.Text = "Statistics Assignment - Yas Intersection";
            _btnLanguage.Text = "فارسی";

            _txtData.RightToLeft = RightToLeft.No;
            _txtData.Font = new Font("Consolas", 10);
            _txtData.Text =
                 "Raw Data (Original Order):\r\n" +
                 string.Join(", ", _data) +
                 "\r\n\r\nSorted Data (Ascending Order):\r\n" +
                 GetSortedDataString();


            _txtStats.RightToLeft = RightToLeft.No;
            _txtStats.Font = new Font("Consolas", 10);
            _txtStats.Text = CalculateStatisticsEnglish();

            _chart.Titles.Clear();
            _chart.Titles.Add("Frequency Distribution of Traffic Accidents");
            _chart.ChartAreas[0].AxisX.Title = "Number of Accidents";
            _chart.ChartAreas[0].AxisY.Title = "Frequency (Count)";
        }

        // ================= FARSI =================

        private void ApplyFarsi()
        {
            this.Text = "تحلیل آماری تصادفات - چهارراه یاس";
            _btnLanguage.Text = "English";

            _txtData.RightToLeft = RightToLeft.Yes;
            _txtData.Font = new Font("Tahoma", 10);
            _txtData.Text =
                "داده‌های خام (ترتیب ثبت شده):\r\n" +
                string.Join(" ، ", _data) +
                "\r\n\r\nداده‌های مرتب‌شده (صعودی):\r\n" +
                string.Join(" ، ", _data.OrderBy(x => x));

            _txtStats.RightToLeft = RightToLeft.Yes;
            _txtStats.Font = new Font("Tahoma", 10);
            _txtStats.Text = CalculateStatisticsFarsi();

            _chart.Titles.Clear();
            _chart.Titles.Add("نمودار توزیع فراوانی تصادفات");
            _chart.ChartAreas[0].AxisX.Title = "تعداد تصادف";
            _chart.ChartAreas[0].AxisY.Title = "فراوانی";
        }

        // ================= STATISTICS =================

        private string CalculateStatisticsEnglish()
        {
            int n = _data.Length;
            double mean = _data.Average();
            double median = GetMedian();
            string mode = GetMode();
            int min = _data.Min();
            int max = _data.Max();

            double variance = _data.Sum(x => Math.Pow(x - mean, 2)) / (n - 1);
            double stdDev = Math.Sqrt(variance);

            return
                "--- STATISTICS REPORT ---\r\n" +
                $"Count (n): {n}\r\n" +
                $"Mean: {mean:F2}\r\n" +
                $"Median: {median}\r\n" +
                $"Mode: {mode}\r\n" +
                $"Min: {min}   Max: {max}\r\n" +
                $"Range: {max - min}\r\n" +
                $"Variance: {variance:F2}\r\n" +
                $"Std Deviation: {stdDev:F2}\r\n";
        }

        private string CalculateStatisticsFarsi()
        {
            int n = _data.Length;
            double mean = _data.Average();
            double median = GetMedian();
            string mode = GetMode();
            int min = _data.Min();
            int max = _data.Max();

            double variance = _data.Sum(x => Math.Pow(x - mean, 2)) / (n - 1);
            double stdDev = Math.Sqrt(variance);

            return
                "📊 گزارش آماری\r\n" +
                "----------------------\r\n" +
                $"تعداد داده‌ها: {n}\r\n" +
                $"میانگین: {mean:F2}\r\n" +
                $"میانه: {median}\r\n" +
                $"نما: {mode}\r\n" +
                $"حداقل: {min}   حداکثر: {max}\r\n" +
                $"دامنه تغییرات: {max - min}\r\n" +
                $"واریانس: {variance:F2}\r\n" +
                $"انحراف معیار: {stdDev:F2}\r\n";
        }

        // ================= HELPERS =================
        private string GetSortedDataString()
        {
            return string.Join(", ", _data.OrderBy(x => x));
        }
        private double GetMedian()
        {
            var sorted = _data.OrderBy(x => x).ToArray();
            int n = sorted.Length;
            return n % 2 == 0
                ? (sorted[n / 2 - 1] + sorted[n / 2]) / 2.0
                : sorted[n / 2];
        }

        private string GetMode()
        {
            int maxFreq = _data.GroupBy(x => x).Max(g => g.Count());
            return string.Join(", ",
                _data.GroupBy(x => x)
                     .Where(g => g.Count() == maxFreq)
                     .Select(g => g.Key));
        }

        // ================= CHART =================

        private Chart CreateChart()
        {
            var chart = new Chart { Dock = DockStyle.Fill };

            var area = new ChartArea("Main");
            area.AxisX.Interval = 1;
            chart.ChartAreas.Add(area);

            var series = new Series("Accidents")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.SteelBlue,
                IsValueShownAsLabel = true
            };

            int min = _data.Min();
            int max = _data.Max();

            for (int i = min; i <= max; i++)
            {
                int count = _data.Count(x => x == i);
                series.Points.AddXY(i, count);
            }

            chart.Series.Add(series);
            return chart;
        }
    }
}
