namespace HIT
{
    partial class Form1
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
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.nameBox = new System.Windows.Forms.TextBox();
            this.connButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ipBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.portBox = new System.Windows.Forms.TextBox();
            this.logBox = new System.Windows.Forms.RichTextBox();
            this.readyButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.passBox = new System.Windows.Forms.TextBox();
            this.sendbtn = new System.Windows.Forms.Button();
            this.chatBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // nameBox
            // 
            this.nameBox.Location = new System.Drawing.Point(56, 59);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(115, 20);
            this.nameBox.TabIndex = 1;
            this.nameBox.Text = "f0X";
            // 
            // connButton
            // 
            this.connButton.Location = new System.Drawing.Point(177, 50);
            this.connButton.Name = "connButton";
            this.connButton.Size = new System.Drawing.Size(81, 30);
            this.connButton.TabIndex = 2;
            this.connButton.Text = "CONNECT";
            this.connButton.UseVisualStyleBackColor = true;
            this.connButton.Click += new System.EventHandler(this.connButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "IP:";
            // 
            // ipBox
            // 
            this.ipBox.Location = new System.Drawing.Point(38, 24);
            this.ipBox.Name = "ipBox";
            this.ipBox.Size = new System.Drawing.Size(95, 20);
            this.ipBox.TabIndex = 4;
            this.ipBox.Text = "188.25.21.100";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(139, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Port:";
            // 
            // portBox
            // 
            this.portBox.Location = new System.Drawing.Point(174, 24);
            this.portBox.Name = "portBox";
            this.portBox.Size = new System.Drawing.Size(66, 20);
            this.portBox.TabIndex = 6;
            this.portBox.Text = "6666";
            // 
            // logBox
            // 
            this.logBox.Location = new System.Drawing.Point(12, 85);
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.Size = new System.Drawing.Size(393, 89);
            this.logBox.TabIndex = 7;
            this.logBox.Text = "";
            // 
            // readyButton
            // 
            this.readyButton.Location = new System.Drawing.Point(348, 50);
            this.readyButton.Name = "readyButton";
            this.readyButton.Size = new System.Drawing.Size(57, 29);
            this.readyButton.TabIndex = 8;
            this.readyButton.Text = "Ready";
            this.readyButton.UseVisualStyleBackColor = true;
            this.readyButton.Click += new System.EventHandler(this.readyButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(246, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Password:";
            // 
            // passBox
            // 
            this.passBox.Location = new System.Drawing.Point(308, 24);
            this.passBox.Name = "passBox";
            this.passBox.PasswordChar = '•';
            this.passBox.Size = new System.Drawing.Size(97, 20);
            this.passBox.TabIndex = 10;
            // 
            // sendbtn
            // 
            this.sendbtn.Location = new System.Drawing.Point(12, 180);
            this.sendbtn.Name = "sendbtn";
            this.sendbtn.Size = new System.Drawing.Size(72, 31);
            this.sendbtn.TabIndex = 11;
            this.sendbtn.Text = "SEND";
            this.sendbtn.UseVisualStyleBackColor = true;
            this.sendbtn.Click += new System.EventHandler(this.sendbtn_Click);
            // 
            // chatBox
            // 
            this.chatBox.Location = new System.Drawing.Point(90, 186);
            this.chatBox.Name = "chatBox";
            this.chatBox.Size = new System.Drawing.Size(315, 20);
            this.chatBox.TabIndex = 12;
            // 
            // Form1
            // 
            this.AcceptButton = this.sendbtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 217);
            this.Controls.Add(this.chatBox);
            this.Controls.Add(this.sendbtn);
            this.Controls.Add(this.passBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.readyButton);
            this.Controls.Add(this.logBox);
            this.Controls.Add(this.portBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ipBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.connButton);
            this.Controls.Add(this.nameBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "SEH Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox nameBox;
        private System.Windows.Forms.Button connButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ipBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox portBox;
        private System.Windows.Forms.RichTextBox logBox;
        private System.Windows.Forms.Button readyButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox passBox;
        private System.Windows.Forms.Button sendbtn;
        private System.Windows.Forms.TextBox chatBox;
    }
}

