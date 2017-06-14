namespace danmu
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.txtRoom = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_ok = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtContent = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtRoom
            // 
            this.txtRoom.Location = new System.Drawing.Point(96, 52);
            this.txtRoom.Name = "txtRoom";
            this.txtRoom.Size = new System.Drawing.Size(194, 21);
            this.txtRoom.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "房间：";
            // 
            // btn_ok
            // 
            this.btn_ok.Location = new System.Drawing.Point(337, 50);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(75, 23);
            this.btn_ok.TabIndex = 2;
            this.btn_ok.Text = "确定";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtContent);
            this.panel1.Location = new System.Drawing.Point(51, 110);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(542, 495);
            this.panel1.TabIndex = 3;
            // 
            // txtContent
            // 
            this.txtContent.Location = new System.Drawing.Point(3, 3);
            this.txtContent.Name = "txtContent";
            this.txtContent.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.txtContent.Size = new System.Drawing.Size(536, 453);
            this.txtContent.TabIndex = 0;
            this.txtContent.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(94, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 12);
            this.label2.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 617);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtRoom);
            this.Name = "Form1";
            this.Text = "弹幕";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtRoom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox txtContent;
    }
}

