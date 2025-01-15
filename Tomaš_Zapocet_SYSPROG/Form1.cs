using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Tomaš_Zapocet_SYSPROG
{
    public partial class Form1 : Form
    {
        private Chart pieChart;
        private Button browseButton;
        private TextBox pathTextBox;
        private System.Windows.Forms.Label pathLabel;
        private FolderBrowserDialog folderBrowserDialog;

        public Form1()
        {
            InitializeComponent();
            SetupCustomComponents();
            this.Text = "File Extension Analyzer";
            this.WindowState = FormWindowState.Maximized;
        }

        private void SetupCustomComponents()
        {
            //Nahadzanie prvkov do formularu konfigurácia
            pathLabel = new System.Windows.Forms.Label
            {
                Text = "Selected folder:",
                Location = new Point(12, 15),
                Size = new Size(90, 20),
                AutoSize = true,
                ForeColor = Color.White
            };

            pathTextBox = new TextBox
            {
                Location = new Point(110, 12),
                Size = new Size(500, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            browseButton = new Button
            {
                Text = "Browse",
                Location = new Point(650, 10),
                Size = new Size(75, 23),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.White
            };
            browseButton.Click += BrowseButton_Click;

            pieChart = new Chart
            {
                Location = new Point(12, 45),
                Size = new Size(760, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Gray,
            };

            //Nastavenie grafu
            ChartArea chartArea = new ChartArea
            {
                BackColor = Color.Gray
            };
            pieChart.ChartAreas.Add(chartArea);

            Series series = new Series
            {
                ChartType = SeriesChartType.Pie,
                Name = "Extensions",
                Font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold)
            };
            pieChart.Series.Add(series);

            //Nastavenie popisu grafu
            Legend legend = new Legend
            {
                BackColor = Color.Gray,
                TextWrapThreshold = 100,
                Font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold)
            };
            pieChart.Legends.Add(legend);

            //Nahadzanie prvkov do formularu inicializacia (aksual nahadzanie)
            this.Controls.Add(pathLabel);
            this.Controls.Add(pathTextBox);
            this.Controls.Add(browseButton);
            this.Controls.Add(pieChart);

            folderBrowserDialog = new FolderBrowserDialog();
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                pathTextBox.Text = folderBrowserDialog.SelectedPath;
                AnalyzeFolder(folderBrowserDialog.SelectedPath);
            }
        }

        private void AnalyzeFolder(string folderPath)
        {
            try
            {
                //Zozbierat vsetky subory v danom foldry a vo vsetkych jeho subfolderoch
                var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);

                //osetrenie ak je dani folder bez suborov 
                if (files.Length == 0)
                {
                    MessageBox.Show("The selected folder doesn't contain any browsable files.",
                        "No Files Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    pieChart.Series["Extensions"].Points.Clear();
                    return;
                }

                //Zlucenie suborov podla extensnov a vyratanie percent
                var extensionGroups = files
                    .Select(file => Path.GetExtension(file).ToLower())
                    .GroupBy(ext => string.IsNullOrEmpty(ext) ? "(no extension)" : ext)
                    .Select(group => new
                    {
                        Extension = group.Key,
                        Count = group.Count(),
                        Percentage = (double)group.Count() / files.Length * 100
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList();

                //Updejtovanie grafu
                pieChart.Series["Extensions"].Points.Clear();
                foreach (var group in extensionGroups)
                {
                    if (group.Percentage < 2)
                    {
                        var point = pieChart.Series["Extensions"].Points.Add(group.Count);
                        point.LegendText = $"{group.Extension} -- {group.Count}ks -- ({group.Percentage:F2}%)";
                    }
                    else
                    {
                        var point = pieChart.Series["Extensions"].Points.Add(group.Count);
                        point.LegendText = $"{group.Extension} -- {group.Count}ks -- ({group.Percentage:F2}%)";
                        point.Label = $"{group.Percentage:F1}%";
                    }

                }

                //Obnovovanie grafu
                pieChart.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error analyzing folder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}