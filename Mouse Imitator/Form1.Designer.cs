namespace Mouse_Imitator
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.replayButton = new System.Windows.Forms.Button();
            this.recordButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.replayProgressBar = new System.Windows.Forms.ProgressBar();
            this.eventsProgressLabel = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.currentStateLabel = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // replayButton
            // 
            this.replayButton.AutoSize = true;
            this.replayButton.Location = new System.Drawing.Point(13, 28);
            this.replayButton.Name = "replayButton";
            this.replayButton.Size = new System.Drawing.Size(52, 25);
            this.replayButton.TabIndex = 0;
            this.replayButton.Text = "Replay";
            this.replayButton.Click += new System.EventHandler(this.replayButton_Click);
            // 
            // recordButton
            // 
            this.recordButton.AutoSize = true;
            this.recordButton.Location = new System.Drawing.Point(119, 28);
            this.recordButton.Name = "recordButton";
            this.recordButton.Size = new System.Drawing.Size(54, 25);
            this.recordButton.TabIndex = 0;
            this.recordButton.Text = "Record";
            this.recordButton.Click += new System.EventHandler(this.recordButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "No replays";
            // 
            // replayProgressBar
            // 
            this.replayProgressBar.Location = new System.Drawing.Point(13, 93);
            this.replayProgressBar.Name = "replayProgressBar";
            this.replayProgressBar.Size = new System.Drawing.Size(160, 23);
            this.replayProgressBar.TabIndex = 2;
            // 
            // eventsProgressLabel
            // 
            this.eventsProgressLabel.AutoSize = true;
            this.eventsProgressLabel.Location = new System.Drawing.Point(179, 97);
            this.eventsProgressLabel.Name = "eventsProgressLabel";
            this.eventsProgressLabel.Size = new System.Drawing.Size(13, 15);
            this.eventsProgressLabel.TabIndex = 3;
            this.eventsProgressLabel.Text = "0";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(227, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3,
            this.toolStripMenuItem4});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(37, 20);
            this.toolStripMenuItem1.Text = "File";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.toolStripMenuItem3.Size = new System.Drawing.Size(140, 22);
            this.toolStripMenuItem3.Text = "Save";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.toolStripMenuItem4.Size = new System.Drawing.Size(140, 22);
            this.toolStripMenuItem4.Text = "Load";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(39, 20);
            this.toolStripMenuItem2.Text = "Edit";
            // 
            // currentStateLabel
            // 
            this.currentStateLabel.AutoSize = true;
            this.currentStateLabel.Location = new System.Drawing.Point(12, 134);
            this.currentStateLabel.Name = "currentStateLabel";
            this.currentStateLabel.Size = new System.Drawing.Size(0, 15);
            this.currentStateLabel.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(227, 158);
            this.Controls.Add(this.currentStateLabel);
            this.Controls.Add(this.eventsProgressLabel);
            this.Controls.Add(this.replayProgressBar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.recordButton);
            this.Controls.Add(this.replayButton);
            this.Controls.Add(this.menuStrip1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button replayButton;
        private System.Windows.Forms.Button recordButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar replayProgressBar;
        private System.Windows.Forms.Label eventsProgressLabel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.Label currentStateLabel;
    }
}

