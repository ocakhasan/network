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
            this.textBox_filename = new System.Windows.Forms.TextBox();
            this.label_filename = new System.Windows.Forms.Label();
            this.button_create_copy = new System.Windows.Forms.Button();
            this.button_delete = new System.Windows.Forms.Button();
            this.button_download = new System.Windows.Forms.Button();
            this.button_make_public = new System.Windows.Forms.Button();
            this.button_get_public = new System.Windows.Forms.Button();
            this.label_public_owner = new System.Windows.Forms.Label();
            this.textBox_public_owner = new System.Windows.Forms.TextBox();
            this.checkBox_public = new System.Windows.Forms.CheckBox();
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
            this.logs.Size = new System.Drawing.Size(376, 378);
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
            this.button_disconnect.Size = new System.Drawing.Size(115, 23);
            this.button_disconnect.TabIndex = 9;
            this.button_disconnect.Text = "DISCONNECT";
            this.button_disconnect.UseVisualStyleBackColor = true;
            this.button_disconnect.Click += new System.EventHandler(this.button_disconnect_Click);
            // 
            // button_list_files
            // 
            this.button_list_files.Location = new System.Drawing.Point(104, 267);
            this.button_list_files.Name = "button_list_files";
            this.button_list_files.Size = new System.Drawing.Size(100, 23);
            this.button_list_files.TabIndex = 10;
            this.button_list_files.Text = "List Files";
            this.button_list_files.UseVisualStyleBackColor = true;
            this.button_list_files.Visible = false;
            this.button_list_files.Click += new System.EventHandler(this.Button_list_files_Click);
            // 
            // textBox_filename
            // 
            this.textBox_filename.Location = new System.Drawing.Point(116, 331);
            this.textBox_filename.Name = "textBox_filename";
            this.textBox_filename.Size = new System.Drawing.Size(94, 20);
            this.textBox_filename.TabIndex = 11;
            this.textBox_filename.Text = "Hasan";
            this.textBox_filename.Visible = false;
            // 
            // label_filename
            // 
            this.label_filename.AutoSize = true;
            this.label_filename.Location = new System.Drawing.Point(52, 334);
            this.label_filename.Name = "label_filename";
            this.label_filename.Size = new System.Drawing.Size(60, 13);
            this.label_filename.TabIndex = 12;
            this.label_filename.Text = "FILENAME";
            this.label_filename.Visible = false;
            // 
            // button_create_copy
            // 
            this.button_create_copy.Location = new System.Drawing.Point(26, 357);
            this.button_create_copy.Name = "button_create_copy";
            this.button_create_copy.Size = new System.Drawing.Size(100, 23);
            this.button_create_copy.TabIndex = 13;
            this.button_create_copy.Text = "CREATE COPY";
            this.button_create_copy.UseVisualStyleBackColor = true;
            this.button_create_copy.Visible = false;
            this.button_create_copy.Click += new System.EventHandler(this.Button_create_copy_Click);
            // 
            // button_delete
            // 
            this.button_delete.Location = new System.Drawing.Point(132, 357);
            this.button_delete.Name = "button_delete";
            this.button_delete.Size = new System.Drawing.Size(78, 23);
            this.button_delete.TabIndex = 16;
            this.button_delete.Text = "DELETE";
            this.button_delete.UseVisualStyleBackColor = true;
            this.button_delete.Visible = false;
            this.button_delete.Click += new System.EventHandler(this.Button_delete_Click);
            // 
            // button_download
            // 
            this.button_download.Location = new System.Drawing.Point(29, 386);
            this.button_download.Name = "button_download";
            this.button_download.Size = new System.Drawing.Size(97, 23);
            this.button_download.TabIndex = 17;
            this.button_download.Text = "DOWNLOAD";
            this.button_download.UseVisualStyleBackColor = true;
            this.button_download.Visible = false;
            this.button_download.Click += new System.EventHandler(this.Button_download_Click);
            // 
            // button_make_public
            // 
            this.button_make_public.Location = new System.Drawing.Point(216, 331);
            this.button_make_public.Name = "button_make_public";
            this.button_make_public.Size = new System.Drawing.Size(122, 23);
            this.button_make_public.TabIndex = 18;
            this.button_make_public.Text = "MAKE PUBLIC";
            this.button_make_public.UseVisualStyleBackColor = true;
            this.button_make_public.Visible = false;
            this.button_make_public.Click += new System.EventHandler(this.Button1_Click);
            // 
            // button_get_public
            // 
            this.button_get_public.Location = new System.Drawing.Point(216, 357);
            this.button_get_public.Name = "button_get_public";
            this.button_get_public.Size = new System.Drawing.Size(122, 23);
            this.button_get_public.TabIndex = 19;
            this.button_get_public.Text = "GET PUBLIC FILES";
            this.button_get_public.UseVisualStyleBackColor = true;
            this.button_get_public.Visible = false;
            this.button_get_public.Click += new System.EventHandler(this.Button_get_public_Click);
            // 
            // label_public_owner
            // 
            this.label_public_owner.AutoSize = true;
            this.label_public_owner.Location = new System.Drawing.Point(130, 424);
            this.label_public_owner.Name = "label_public_owner";
            this.label_public_owner.Size = new System.Drawing.Size(49, 13);
            this.label_public_owner.TabIndex = 22;
            this.label_public_owner.Text = "OWNER";
            this.label_public_owner.Visible = false;
            // 
            // textBox_public_owner
            // 
            this.textBox_public_owner.Location = new System.Drawing.Point(185, 421);
            this.textBox_public_owner.Name = "textBox_public_owner";
            this.textBox_public_owner.Size = new System.Drawing.Size(81, 20);
            this.textBox_public_owner.TabIndex = 21;
            this.textBox_public_owner.Visible = false;
            // 
            // checkBox_public
            // 
            this.checkBox_public.AutoSize = true;
            this.checkBox_public.Location = new System.Drawing.Point(133, 390);
            this.checkBox_public.Name = "checkBox_public";
            this.checkBox_public.Size = new System.Drawing.Size(83, 17);
            this.checkBox_public.TabIndex = 23;
            this.checkBox_public.Text = "IS PUBLIC?";
            this.checkBox_public.UseVisualStyleBackColor = true;
            this.checkBox_public.Visible = false;
            this.checkBox_public.CheckedChanged += new System.EventHandler(this.CheckBox_public_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 477);
            this.Controls.Add(this.checkBox_public);
            this.Controls.Add(this.label_public_owner);
            this.Controls.Add(this.textBox_public_owner);
            this.Controls.Add(this.button_get_public);
            this.Controls.Add(this.button_make_public);
            this.Controls.Add(this.button_download);
            this.Controls.Add(this.button_delete);
            this.Controls.Add(this.button_create_copy);
            this.Controls.Add(this.label_filename);
            this.Controls.Add(this.textBox_filename);
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
        private System.Windows.Forms.TextBox textBox_filename;
        private System.Windows.Forms.Label label_filename;
        private System.Windows.Forms.Button button_create_copy;
        private System.Windows.Forms.Button button_delete;
        private System.Windows.Forms.Button button_download;
        private System.Windows.Forms.Button button_make_public;
        private System.Windows.Forms.Button button_get_public;
        private System.Windows.Forms.Label label_public_owner;
        private System.Windows.Forms.TextBox textBox_public_owner;
        private System.Windows.Forms.CheckBox checkBox_public;
    }
}

