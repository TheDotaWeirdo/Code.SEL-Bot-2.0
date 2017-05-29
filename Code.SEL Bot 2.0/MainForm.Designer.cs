namespace Code.SEL_Bot_2._0
{
	partial class MainForm
{
	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	/// Clean up any resources being used.
	/// </summary>
	/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	protected override void Dispose(bool disposing)
	{
		if (disposing && (components != null))
		{
			components.Dispose();
		}
		try
		{
			base.Dispose(disposing);
		}
		catch (System.Exception) { }
	}

	#region Windows Form Designer generated code

	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.Cons = new System.Windows.Forms.Label();
			this.pictureBox_Listen = new System.Windows.Forms.PictureBox();
			this.pictureBox_ServerStatus = new System.Windows.Forms.PictureBox();
			this.label_ServerStatus = new System.Windows.Forms.Label();
			this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.pictureBox_Stop = new System.Windows.Forms.PictureBox();
			this.pictureBox_Start = new System.Windows.Forms.PictureBox();
			this.pictureBox_SaveData = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_Listen)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_ServerStatus)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_Stop)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_Start)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_SaveData)).BeginInit();
			this.SuspendLayout();
			// 
			// Cons
			// 
			this.Cons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
			this.Cons.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Cons.Font = new System.Drawing.Font("Lucida Console", 8.25F);
			this.Cons.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
			this.Cons.Location = new System.Drawing.Point(13, 12);
			this.Cons.Margin = new System.Windows.Forms.Padding(3);
			this.Cons.Name = "Cons";
			this.Cons.Padding = new System.Windows.Forms.Padding(3);
			this.Cons.Size = new System.Drawing.Size(732, 210);
			this.Cons.TabIndex = 1;
			// 
			// pictureBox_Listen
			// 
			this.pictureBox_Listen.Image = global::Code.SEL_Bot_2._0.Properties.Resources.Listen_Off;
			this.pictureBox_Listen.Location = new System.Drawing.Point(177, 235);
			this.pictureBox_Listen.Name = "pictureBox_Listen";
			this.pictureBox_Listen.Size = new System.Drawing.Size(48, 48);
			this.pictureBox_Listen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox_Listen.TabIndex = 7;
			this.pictureBox_Listen.TabStop = false;
			this.pictureBox_Listen.Click += new System.EventHandler(this.Listen_Click);
			// 
			// pictureBox_ServerStatus
			// 
			this.pictureBox_ServerStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(195)))), ((int)(((byte)(46)))), ((int)(((byte)(43)))));
			this.pictureBox_ServerStatus.Location = new System.Drawing.Point(13, 222);
			this.pictureBox_ServerStatus.Name = "pictureBox_ServerStatus";
			this.pictureBox_ServerStatus.Size = new System.Drawing.Size(732, 7);
			this.pictureBox_ServerStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox_ServerStatus.TabIndex = 8;
			this.pictureBox_ServerStatus.TabStop = false;
			// 
			// label_ServerStatus
			// 
			this.label_ServerStatus.AutoSize = true;
			this.label_ServerStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(195)))), ((int)(((byte)(46)))), ((int)(((byte)(43)))));
			this.label_ServerStatus.Font = new System.Drawing.Font("Lucida Console", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_ServerStatus.ForeColor = System.Drawing.Color.White;
			this.label_ServerStatus.Location = new System.Drawing.Point(630, 225);
			this.label_ServerStatus.Name = "label_ServerStatus";
			this.label_ServerStatus.Size = new System.Drawing.Size(115, 15);
			this.label_ServerStatus.TabIndex = 9;
			this.label_ServerStatus.Text = "Disconnedted";
			this.label_ServerStatus.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// notifyIcon
			// 
			this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
			this.notifyIcon.Text = "Code.SEL Bot";
			this.notifyIcon.Visible = true;
			this.notifyIcon.Click += new System.EventHandler(this.Restore_Click);
			// 
			// pictureBox_Stop
			// 
			this.pictureBox_Stop.Image = global::Code.SEL_Bot_2._0.Properties.Resources.Stop_Bot;
			this.pictureBox_Stop.Location = new System.Drawing.Point(69, 235);
			this.pictureBox_Stop.Name = "pictureBox_Stop";
			this.pictureBox_Stop.Size = new System.Drawing.Size(48, 48);
			this.pictureBox_Stop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox_Stop.TabIndex = 10;
			this.pictureBox_Stop.TabStop = false;
			this.pictureBox_Stop.Click += new System.EventHandler(this.Stop_Click);
			// 
			// pictureBox_Start
			// 
			this.pictureBox_Start.Image = global::Code.SEL_Bot_2._0.Properties.Resources.Start_Bot;
			this.pictureBox_Start.Location = new System.Drawing.Point(15, 235);
			this.pictureBox_Start.Name = "pictureBox_Start";
			this.pictureBox_Start.Size = new System.Drawing.Size(48, 48);
			this.pictureBox_Start.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox_Start.TabIndex = 11;
			this.pictureBox_Start.TabStop = false;
			this.pictureBox_Start.Click += new System.EventHandler(this.Start_Click);
			// 
			// pictureBox_SaveData
			// 
			this.pictureBox_SaveData.Image = global::Code.SEL_Bot_2._0.Properties.Resources.SaveData;
			this.pictureBox_SaveData.Location = new System.Drawing.Point(123, 235);
			this.pictureBox_SaveData.Name = "pictureBox_SaveData";
			this.pictureBox_SaveData.Size = new System.Drawing.Size(48, 48);
			this.pictureBox_SaveData.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox_SaveData.TabIndex = 12;
			this.pictureBox_SaveData.TabStop = false;
			this.pictureBox_SaveData.Click += new System.EventHandler(this.SaveData_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(757, 441);
			this.Controls.Add(this.pictureBox_SaveData);
			this.Controls.Add(this.pictureBox_Start);
			this.Controls.Add(this.pictureBox_Stop);
			this.Controls.Add(this.label_ServerStatus);
			this.Controls.Add(this.pictureBox_ServerStatus);
			this.Controls.Add(this.pictureBox_Listen);
			this.Controls.Add(this.Cons);
			this.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(5);
			this.MinimumSize = new System.Drawing.Size(550, 300);
			this.Name = "MainForm";
			this.Text = "Code.SEL Bot";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.Resize += new System.EventHandler(this.MainForm_Resize);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_Listen)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_ServerStatus)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_Stop)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_Start)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_SaveData)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

	}

	#endregion

	public System.Windows.Forms.Label Cons;
		private System.Windows.Forms.PictureBox pictureBox_Listen;
		private System.Windows.Forms.PictureBox pictureBox_ServerStatus;
		private System.Windows.Forms.Label label_ServerStatus;
		private System.Windows.Forms.NotifyIcon notifyIcon;
		private System.Windows.Forms.PictureBox pictureBox_Stop;
		private System.Windows.Forms.PictureBox pictureBox_Start;
		private System.Windows.Forms.PictureBox pictureBox_SaveData;
	}
}

