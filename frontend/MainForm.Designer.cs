namespace BwetForecasterWinForms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox tickerBox;
        private System.Windows.Forms.Button forecastButton;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label priceLabel;
        private System.Windows.Forms.Label forecastLabel;
        private System.Windows.Forms.Label changeLabel;
        private System.Windows.Forms.Label ciLabel;
        private System.Windows.Forms.DataGridView detailsGrid;
        private OxyPlot.WindowsForms.PlotView plotView;

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
            this.components = new System.ComponentModel.Container();
            this.plotView = new OxyPlot.WindowsForms.PlotView();
            this.detailsGrid = new System.Windows.Forms.DataGridView();
            this.priceLabel = new System.Windows.Forms.Label();
            this.forecastLabel = new System.Windows.Forms.Label();
            this.changeLabel = new System.Windows.Forms.Label();
            this.ciLabel = new System.Windows.Forms.Label();
            var infoPanel = new System.Windows.Forms.Panel();
            var topPanel = new System.Windows.Forms.Panel();
            var tickerLabel = new System.Windows.Forms.Label();
            this.tickerBox = new System.Windows.Forms.TextBox();
            this.forecastButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.detailsGrid)).BeginInit();
            infoPanel.SuspendLayout();
            topPanel.SuspendLayout();
            this.SuspendLayout();

            // plotView (Fill) - added first so docking works correctly
            this.plotView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotView.Name = "plotView";

            // detailsGrid (Bottom)
            this.detailsGrid.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.detailsGrid.Height = 260;
            this.detailsGrid.ReadOnly = true;
            this.detailsGrid.AllowUserToAddRows = false;
            this.detailsGrid.AllowUserToDeleteRows = false;
            this.detailsGrid.RowHeadersVisible = false;
            this.detailsGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.detailsGrid.Columns.Add("Day", "Day");
            this.detailsGrid.Columns.Add("Date", "Date");
            this.detailsGrid.Columns.Add("Predicted", "Predicted");
            this.detailsGrid.Columns.Add("Q10", "10th %ile");
            this.detailsGrid.Columns.Add("Q90", "90th %ile");

            // info labels inside infoPanel (Right)
            foreach (var lbl in new[] { this.priceLabel, this.forecastLabel, this.changeLabel, this.ciLabel })
            {
                lbl.Dock = System.Windows.Forms.DockStyle.Top;
                lbl.Height = 70;
                lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                lbl.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            }
            this.priceLabel.Text = "Current Price\n$0.00";
            this.forecastLabel.Text = "Forecast (final day)\n$0.00";
            this.forecastLabel.ForeColor = System.Drawing.Color.Green;
            this.changeLabel.Text = "Expected Change\n+0.00%";
            this.ciLabel.Text = "80% Interval (final day)\n$0.00 - $0.00";
            this.ciLabel.ForeColor = System.Drawing.Color.DarkOrange;

            infoPanel.Dock = System.Windows.Forms.DockStyle.Right;
            infoPanel.Width = 260;
            infoPanel.Padding = new System.Windows.Forms.Padding(10);
            infoPanel.Controls.Add(this.ciLabel);
            infoPanel.Controls.Add(this.changeLabel);
            infoPanel.Controls.Add(this.forecastLabel);
            infoPanel.Controls.Add(this.priceLabel);

            // top controls (Top)
            topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            topPanel.Height = 52;
            topPanel.Padding = new System.Windows.Forms.Padding(8);

            tickerLabel.Text = "Ticker:";
            tickerLabel.AutoSize = true;
            tickerLabel.Location = new System.Drawing.Point(10, 17);

            this.tickerBox.Location = new System.Drawing.Point(70, 14);
            this.tickerBox.Width = 100;
            this.tickerBox.Name = "tickerBox";

            this.forecastButton.Text = "Forecast";
            this.forecastButton.Location = new System.Drawing.Point(185, 12);
            this.forecastButton.Width = 110;
            this.forecastButton.Height = 28;
            this.forecastButton.Name = "forecastButton";

            this.statusLabel.Text = "Starting...";
            this.statusLabel.AutoSize = true;
            this.statusLabel.ForeColor = System.Drawing.Color.Gray;
            this.statusLabel.Location = new System.Drawing.Point(310, 17);

            topPanel.Controls.Add(tickerLabel);
            topPanel.Controls.Add(this.tickerBox);
            topPanel.Controls.Add(this.forecastButton);
            topPanel.Controls.Add(this.statusLabel);

            // MainForm
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Text = "BWET Stock Forecaster - TimesFM";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Controls.Add(this.plotView);
            this.Controls.Add(this.detailsGrid);
            this.Controls.Add(infoPanel);
            this.Controls.Add(topPanel);

            ((System.ComponentModel.ISupportInitialize)(this.detailsGrid)).EndInit();
            infoPanel.ResumeLayout(false);
            topPanel.ResumeLayout(false);
            topPanel.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
