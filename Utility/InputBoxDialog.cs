﻿using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Utility
{
    /// <summary>
    /// Summary description for InputBox.
    /// </summary>
    public class InputBoxDialog : System.Windows.Forms.Form
    {

        #region Windows Contols and Constructor

        private System.Windows.Forms.Label lblPrompt;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtInput;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public InputBoxDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblPrompt = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblPrompt
            // 
            this.lblPrompt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPrompt.BackColor = System.Drawing.SystemColors.Control;
            this.lblPrompt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrompt.Location = new System.Drawing.Point(12, 9);
            this.lblPrompt.Name = "lblPrompt";
            this.lblPrompt.Size = new System.Drawing.Size(302, 82);
            this.lblPrompt.TabIndex = 3;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(326, 24);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(64, 24);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(326, 56);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(64, 24);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtInput
            // 
            this.txtInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInput.Location = new System.Drawing.Point(8, 100);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(379, 20);
            this.txtInput.TabIndex = 0;
            // 
            // InputBoxDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(398, 128);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblPrompt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputBoxDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "InputBox";
            this.Load += new System.EventHandler(this.InputBox_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        #region Private Variables
        string formCaption = string.Empty;
        string formPrompt = string.Empty;
        string inputResponse = string.Empty;
        string defaultValue = string.Empty;
        #endregion

        #region Public Properties
        public string FormCaption
        {
            get { return formCaption; }
            set { formCaption = value; }
        } // property FormCaption
        public string FormPrompt
        {
            get { return formPrompt; }
            set { formPrompt = value; }
        } // property FormPrompt
        public string InputResponse
        {
            get { return inputResponse; }
            set { inputResponse = value; }
        } // property InputResponse
        public string DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        } // property DefaultValue

        #endregion

        #region Form and Control Events
        private void InputBox_Load(object sender, System.EventArgs e)
        {
            this.txtInput.Text = defaultValue;
            this.lblPrompt.Text = formPrompt;

            int NLines = formPrompt.Split('\n').Length;
            if (NLines > 3)
            {
                Size size = TextRenderer.MeasureText(lblPrompt.Text, lblPrompt.Font);
                var NewWidth = lblPrompt.Width;
                var NewHeight = lblPrompt.Height;

                size.Height += 2;        //add some margin to the height

                if (size.Width > NewWidth) NewWidth = size.Width;
                if (size.Height > NewHeight) NewHeight = size.Height;

                if (NewWidth != lblPrompt.Width || NewHeight != lblPrompt.Height) {
                    this.Height += NewHeight - lblPrompt.Height;
                    this.Width += NewWidth - lblPrompt.Width;
                    //lblPrompt.ClientSize = new Size(NewWidth, NewHeight);
                }
            }

            this.Text = formCaption;
            this.txtInput.SelectionStart = 0;
            this.txtInput.SelectionLength = this.txtInput.Text.Length;
            this.txtInput.Focus();
        }


        private void btnOK_Click(object sender, System.EventArgs e)
        {
            InputResponse = this.txtInput.Text;
            this.Close();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}