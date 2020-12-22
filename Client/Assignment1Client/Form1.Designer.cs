namespace Assignment1Client
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_ip = new System.Windows.Forms.TextBox();
            this.textBox_port = new System.Windows.Forms.TextBox();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.button_connect = new System.Windows.Forms.Button();
            this.logs = new System.Windows.Forms.RichTextBox();
            this.button_send = new System.Windows.Forms.Button();
            this.button_disconnect = new System.Windows.Forms.Button();
            this.button_list_files = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(52, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(52, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "PORT";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(52, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "NAME";
            // 
            // textBox_ip
            // 
            this.textBox_ip.Location = new System.Drawing.Point(104, 31);
            this.textBox_ip.Name = "textBox_ip";
            this.textBox_ip.Size = new System.Drawing.Size(100, 20);
            this.textBox_ip.TabIndex = 3;
            this.textBox_ip.Text = "127.0.0.1";
            // 
            // textBox_port
            // 
            this.textBox_port.Location = new System.Drawing.Point(104, 65);
            this.textBox_port.Name = "textBox_port";
            this.textBox_port.Size = new System.Drawing.Size(100, 20);
            this.textBox_port.TabIndex = 4;
            this.textBox_port.Text = "5000";
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(104, 99);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(100, 20);
            this.textBox_name.TabIndex = 5;
            this.textBox_name.Text = "Hasan";
            // 
            // button_connect
            // 
            this.button_connect.Location = new System.Drawing.Point(104, 156);
            this.button_connect.Name = "button_connect";
            this.button_connect.Size = new System.Drawing.Size(112, 23);
            this.button_connect.TabIndex = 6;
            this.button_connect.Text = "CONNECT";
            this.button_connect.UseVisualStyleBackColor = true;
            this.button_connect.Click += new System.EventHandler(this.button_connect_Click);
            // 
            // logs
            // 
            this.logs.Location = new System.Drawing.Point(359, 31);
            this.logs.Name = "logs";
            this.logs.Size = new System.Drawing.Size(376, 334);
            this.logs.TabIndex = 7;
            this.logs.Text = "";
            // 
            // button_send
            // 
            this.button_send.Location = new System.Drawing.Point(29, 238);
            this.button_send.Name = "button_send";
            this.button_send.Size = new System.Drawing.Size(112, 23);
            this.button_send.TabIndex = 8;
            this.button_send.Text = "SEND FILE";
            this.button_send.UseVisualStyleBackColor = true;
            this.button_send.Click += new System.EventHandler(this.button_send_Click_1);
            // 
            // button_disconnect
            // 
            this.button_disconnect.Location = new System.Drawing.Point(173, 238);
            this.button_disconnect.Name = "button_disconnect";
            this.button_disconnect.Size = new System.Drawing.Size(130, 23);
            this.button_disconnect.TabIndex = 9;
            this.button_disconnect.Text = "DISCONNECT";
            this.button_disconnect.UseVisualStyleBackColor = true;
            this.button_disconnect.Click += new System.EventHandler(this.button_disconnect_Click);
            // 
            // button_list_files
            // 
            this.button_list_files.Location = new System.Drawing.Point(104, 276);
            this.button_list_files.Name = "button_list_files";
            this.button_list_files.Size = new System.Drawing.Size(112, 23);
            this.button_list_files.TabIndex = 10;
            this.button_list_files.Text = "List Files";
            this.button_list_files.UseVisualStyleBackColor = true;
            this.button_list_files.Visible = false;
            this.button_list_files.Click += new System.EventHandler(this.Button_list_files_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(783, 413);
            this.Controls.Add(this.button_list_files);
            this.Controls.Add(this.button_disconnect);
            this.Controls.Add(this.button_send);
            this.Controls.Add(this.logs);
            this.Controls.Add(this.button_connect);
            this.Controls.Add(this.textBox_name);
            this.Controls.Add(this.textBox_port);
            this.Controls.Add(this.textBox_ip);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_ip;
        private System.Windows.Forms.TextBox textBox_port;
        private System.Windows.Forms.TextBox textBox_name;
        private System.Windows.Forms.Button button_connect;
        private System.Windows.Forms.RichTextBox logs;
        private System.Windows.Forms.Button button_send;
        private System.Windows.Forms.Button button_disconnect;
        private System.Windows.Forms.Button button_list_files;
    }
}

