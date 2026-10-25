namespace BwetForecasterWinForms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ComboBox tickerBox;
        private System.Windows.Forms.Button clearTickerButton;
        private System.Windows.Forms.Button clearHistoryButton;
        private System.Windows.Forms.Button forecastButton;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label lastUpdatedLabel;
        private System.Windows.Forms.ProgressBar loadingIndicator;
        private System.Windows.Forms.ComboBox rangeBox;
        private System.Windows.Forms.Button resetZoomButton;
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
            this.tickerBox = new System.Windows.Forms.ComboBox();
            this.clearTickerButton = new System.Windows.Forms.Button();
            this.clearHistoryButton = new System.Windows.Forms.Button();
            this.forecastButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.lastUpdatedLabel = new System.Windows.Forms.Label();
            this.loadingIndicator = new System.Windows.Forms.ProgressBar();
            this.rangeBox = new System.Windows.Forms.ComboBox();
            this.resetZoomButton = new System.Windows.Forms.Button();
            var rangeLabel = new System.Windows.Forms.Label();
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
            topPanel.Height = 64;
            topPanel.Padding = new System.Windows.Forms.Padding(10);

            tickerLabel.Text = "Ticker:";
            tickerLabel.AutoSize = true;
            tickerLabel.Location = new System.Drawing.Point(12, 22);

            this.tickerBox.Location = new System.Drawing.Point(65, 17);
            this.tickerBox.Width = 150;
            this.tickerBox.Name = "tickerBox";
            this.tickerBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.tickerBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.tickerBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;

            this.clearTickerButton.Text = "×";
            this.clearTickerButton.Location = new System.Drawing.Point(220, 16);
            this.clearTickerButton.Size = new System.Drawing.Size(28, 28);
            this.clearTickerButton.Name = "clearTickerButton";
            this.clearTickerButton.TabStop = false;

            this.forecastButton.Text = "Forecast";
            this.forecastButton.Location = new System.Drawing.Point(255, 16);
            this.forecastButton.Width = 105;
            this.forecastButton.Height = 28;
            this.forecastButton.Name = "forecastButton";

            this.statusLabel.Text = "Starting...";
            this.statusLabel.AutoEllipsis = true;
            this.statusLabel.BackColor = System.Drawing.Color.Gainsboro;
            this.statusLabel.ForeColor = System.Drawing.Color.DimGray;
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.statusLabel.Location = new System.Drawing.Point(675, 17);
            this.statusLabel.Size = new System.Drawing.Size(220, 28);

            this.loadingIndicator.Location = new System.Drawing.Point(645, 21);
            this.loadingIndicator.Size = new System.Drawing.Size(22, 20);
            this.loadingIndicator.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.loadingIndicator.MarqueeAnimationSpeed = 25;
            this.loadingIndicator.Visible = false;

            this.lastUpdatedLabel.Text = "Updated: —";
            this.lastUpdatedLabel.AutoEllipsis = true;
            this.lastUpdatedLabel.ForeColor = System.Drawing.Color.DimGray;
            this.lastUpdatedLabel.Location = new System.Drawing.Point(900, 22);
            this.lastUpdatedLabel.Size = new System.Drawing.Size(130, 20);

            rangeLabel.Text = "History:";
            rangeLabel.AutoSize = true;
            rangeLabel.Location = new System.Drawing.Point(375, 22);

            this.rangeBox.Location = new System.Drawing.Point(435, 17);
            this.rangeBox.Width = 70;
            this.rangeBox.Name = "rangeBox";
            this.rangeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rangeBox.Items.AddRange(new object[] { "1M", "3M", "6M", "1Y", "2Y" });
            this.rangeBox.SelectedItem = "6M";

            this.resetZoomButton.Text = "Reset zoom";
            this.resetZoomButton.Location = new System.Drawing.Point(520, 16);
            this.resetZoomButton.Width = 110;
            this.resetZoomButton.Height = 28;
            this.resetZoomButton.Name = "resetZoomButton";

            this.clearHistoryButton.Text = "Clear history";
            this.clearHistoryButton.Location = new System.Drawing.Point(1040, 16);
            this.clearHistoryButton.Width = 100;
            this.clearHistoryButton.Height = 28;
            this.clearHistoryButton.Name = "clearHistoryButton";

            topPanel.Controls.Add(tickerLabel);
            topPanel.Controls.Add(this.tickerBox);
            topPanel.Controls.Add(this.clearTickerButton);
            topPanel.Controls.Add(this.forecastButton);
            topPanel.Controls.Add(rangeLabel);
            topPanel.Controls.Add(this.rangeBox);
            topPanel.Controls.Add(this.resetZoomButton);
            topPanel.Controls.Add(this.loadingIndicator);
            topPanel.Controls.Add(this.statusLabel);
            topPanel.Controls.Add(this.lastUpdatedLabel);
            topPanel.Controls.Add(this.clearHistoryButton);

            // MainForm
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Text = "Stock Forecaster - TimesFM";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.KeyPreview = true;
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
