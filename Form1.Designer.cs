
namespace PythonScriptTest
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
            this.btnEval = new System.Windows.Forms.Button();
            this.btnAbortScript = new System.Windows.Forms.Button();
            this.lstResults = new System.Windows.Forms.ListBox();
            this.btnRunScript = new System.Windows.Forms.Button();
            this.cbxScriptNames = new System.Windows.Forms.ComboBox();
            this.btnLoadScript = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.cbxResetExecEnv = new System.Windows.Forms.CheckBox();
            this.btnClearResults = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnEval
            // 
            this.btnEval.Location = new System.Drawing.Point(675, 173);
            this.btnEval.Name = "btnEval";
            this.btnEval.Size = new System.Drawing.Size(113, 28);
            this.btnEval.TabIndex = 20;
            this.btnEval.Text = "Eval";
            this.btnEval.UseVisualStyleBackColor = true;
            this.btnEval.Visible = false;
            this.btnEval.Click += new System.EventHandler(this.btnEval_Click);
            // 
            // btnAbortScript
            // 
            this.btnAbortScript.Location = new System.Drawing.Point(326, 223);
            this.btnAbortScript.Name = "btnAbortScript";
            this.btnAbortScript.Size = new System.Drawing.Size(87, 21);
            this.btnAbortScript.TabIndex = 18;
            this.btnAbortScript.Text = "Abort Script";
            this.btnAbortScript.UseVisualStyleBackColor = true;
            this.btnAbortScript.Click += new System.EventHandler(this.btnAbortScript_Click);
            // 
            // lstResults
            // 
            this.lstResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstResults.FormattingEnabled = true;
            this.lstResults.HorizontalScrollbar = true;
            this.lstResults.Location = new System.Drawing.Point(12, 250);
            this.lstResults.Name = "lstResults";
            this.lstResults.Size = new System.Drawing.Size(776, 303);
            this.lstResults.TabIndex = 17;
            // 
            // btnRunScript
            // 
            this.btnRunScript.Location = new System.Drawing.Point(233, 223);
            this.btnRunScript.Name = "btnRunScript";
            this.btnRunScript.Size = new System.Drawing.Size(87, 21);
            this.btnRunScript.TabIndex = 15;
            this.btnRunScript.Text = "Run Script";
            this.btnRunScript.UseVisualStyleBackColor = true;
            this.btnRunScript.Click += new System.EventHandler(this.btnRunScript_Click);
            // 
            // cbxScriptNames
            // 
            this.cbxScriptNames.FormattingEnabled = true;
            this.cbxScriptNames.Location = new System.Drawing.Point(12, 223);
            this.cbxScriptNames.Name = "cbxScriptNames";
            this.cbxScriptNames.Size = new System.Drawing.Size(215, 21);
            this.cbxScriptNames.TabIndex = 14;
            // 
            // btnLoadScript
            // 
            this.btnLoadScript.Location = new System.Drawing.Point(675, 30);
            this.btnLoadScript.Name = "btnLoadScript";
            this.btnLoadScript.Size = new System.Drawing.Size(113, 36);
            this.btnLoadScript.TabIndex = 13;
            this.btnLoadScript.Text = "Load Script File";
            this.btnLoadScript.UseVisualStyleBackColor = true;
            this.btnLoadScript.Click += new System.EventHandler(this.btnLoadScript_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.Location = new System.Drawing.Point(12, 30);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(657, 187);
            this.txtOutput.TabIndex = 10;
            // 
            // cbxResetExecEnv
            // 
            this.cbxResetExecEnv.AutoSize = true;
            this.cbxResetExecEnv.Checked = true;
            this.cbxResetExecEnv.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxResetExecEnv.Location = new System.Drawing.Point(12, 7);
            this.cbxResetExecEnv.Name = "cbxResetExecEnv";
            this.cbxResetExecEnv.Size = new System.Drawing.Size(154, 17);
            this.cbxResetExecEnv.TabIndex = 21;
            this.cbxResetExecEnv.Text = "Reset Environment on Run";
            this.cbxResetExecEnv.UseVisualStyleBackColor = true;
            // 
            // btnClearResults
            // 
            this.btnClearResults.Location = new System.Drawing.Point(675, 216);
            this.btnClearResults.Name = "btnClearResults";
            this.btnClearResults.Size = new System.Drawing.Size(113, 28);
            this.btnClearResults.TabIndex = 22;
            this.btnClearResults.Text = "Clear Results";
            this.btnClearResults.UseVisualStyleBackColor = true;
            this.btnClearResults.Click += new System.EventHandler(this.btnClearResults_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 562);
            this.Controls.Add(this.btnClearResults);
            this.Controls.Add(this.cbxResetExecEnv);
            this.Controls.Add(this.btnEval);
            this.Controls.Add(this.btnAbortScript);
            this.Controls.Add(this.lstResults);
            this.Controls.Add(this.btnRunScript);
            this.Controls.Add(this.cbxScriptNames);
            this.Controls.Add(this.btnLoadScript);
            this.Controls.Add(this.txtOutput);
            this.Name = "Form1";
            this.Text = "Python Script test";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnEval;
        private System.Windows.Forms.Button btnAbortScript;
        public System.Windows.Forms.ListBox lstResults;
        private System.Windows.Forms.Button btnRunScript;
        private System.Windows.Forms.ComboBox cbxScriptNames;
        private System.Windows.Forms.Button btnLoadScript;
        public System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.CheckBox cbxResetExecEnv;
        private System.Windows.Forms.Button btnClearResults;
    }
}

