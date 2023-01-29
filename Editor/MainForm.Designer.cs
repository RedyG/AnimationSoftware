using Editor.Forms.Controls;

namespace Editor.Forms
{
    partial class MainForm
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
            this.blazorWebView = new Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView();
            this.previewControl1 = new Editor.Forms.Controls.PreviewControl();
            this.SuspendLayout();
            // 
            // blazorWebView
            // 
            this.blazorWebView.Location = new System.Drawing.Point(0, 0);
            this.blazorWebView.Name = "blazorWebView";
            this.blazorWebView.Size = new System.Drawing.Size(496, 352);
            this.blazorWebView.TabIndex = 0;
            this.blazorWebView.Text = "blazorWebView1";
            // 
            // previewControl1
            // 
            this.previewControl1.BackColor = System.Drawing.Color.Black;
            this.previewControl1.Location = new System.Drawing.Point(309, 21);
            this.previewControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.previewControl1.Name = "previewControl1";
            this.previewControl1.Size = new System.Drawing.Size(427, 365);
            this.previewControl1.TabIndex = 1;
            this.previewControl1.VSync = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.previewControl1);
            this.Controls.Add(this.blazorWebView);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView blazorWebView;
        private PreviewControl previewControl1;
    }
}