using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utility;
using Scripting;
using System.IO;

namespace PythonScriptTest
{
    public partial class Form1 : Form
    {
        public Utilities myUtils;
        public bool AppRunning = false;
        public bool AbortScript = false;
        bool ScriptRunning = false;
        PythonScripting myPythonHandler;
        string SCPFileName;

        public Form1()
        {
            AppRunning = true;
            InitializeComponent();
        }

        public void Display(string line)
        {
            LogResults(line);
            Application.DoEvents();
        }

        public void outString(string line)
        {
            UIBlockingInvoke(new MethodInvoker(delegate ()
            {
                txtOutput.Text = line;
            }));
            Application.DoEvents();
        }

        public void LogResults(string line, bool Comment = false, bool OmitTime = false)
        {
            string myTime;
            string CommentChar;
            string DelimiterChars = "\t";
            string LogLine;

            if (Comment)
            {
                CommentChar = "#";
            }
            else
            {
                CommentChar = "";
            }
            myTime = DateTime.Now.ToString("HH:mm:ss.fff tt");

            if (!OmitTime)
            {
                LogLine = CommentChar + myTime + DelimiterChars + line;
            }
            else
            {
                LogLine = CommentChar + line;
            }

            UIBlockingInvoke(new MethodInvoker(delegate ()
            {
                lstResults.Items.Add(LogLine);
                lstResults.SelectedIndex = lstResults.Items.Count - 1;
                Application.DoEvents();
            }));
        }

        public void csSleep(double milliseconds)
        {
            System.Threading.Thread.Sleep((int)milliseconds);
            Application.DoEvents();
        }

        /// <summary>
        /// Runs a MethodInvoker delegate on the UI thread from whichever thread we are currently calling from.
        /// </summary>
        /// <param name="ivk"></param>
        public void UI(MethodInvoker ivk)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(ivk);
            else
                ivk();
        }

        /// <summary>
        /// Runs a MethodInvoker delegate on the UI thread from whichever thread we are currently calling from and BLOCKS until it is complete
        /// </summary>
        /// <param name="ivk"></param>
        public void UIBlockingInvoke(MethodInvoker ivk)
        {
            bool result;
            System.Threading.ManualResetEvent UIAsyncComplete = new System.Threading.ManualResetEvent(false);
            UIAsyncComplete.Reset();
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(delegate ()
                {
                    try
                    {
                        ivk();
                    }
                    finally
                    {
                        UIAsyncComplete.Set();
                    }
                }));

                while (AppRunning)
                {
                    //Check AppRunning...
                    //Don't call WaitOne(int32)!!!  It was added in .NET framework 2.0 SERVICE PACK 2!!  
                    //Instead call WaitOne(int32, false).  This works in .NET framework 2.0 RTM.
                    result = UIAsyncComplete.WaitOne(500, false);      //timeout after 500mS to check if AppRunning is still true
                    if (result)
                    {
                        break;  //Exit when UIAsyncComplete has been set OR if AppRunning becomes false.
                    }
                }
            }
            else
            {
                ivk();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            AppRunning = false;
        }

        private void btnLoadScript_Click(object sender, EventArgs e)
        {

            OpenFileDialog o = new OpenFileDialog();
            o.Filter = "Python files (*.py)|*.py|All files (*.*)|*.*";
            o.InitialDirectory = Application.StartupPath;
            if (o.ShowDialog() == DialogResult.OK)
            {
                string ScriptFile = o.FileName;
                LoadScript(ScriptFile);
            }
        }

        public string StripLeadingSpaces(string str_Renamed)
        {
            return str_Renamed.TrimStart(new char[] { ' ' });
        }

        // This function strips trailing spaces from any string
        public string StripTrailingSpaces(string str_Renamed)
        {
            return str_Renamed.TrimEnd(new char[] { ' ' });
        }

        private void LoadScript(string myScriptFileName, Boolean DontUpdateScriptBox = false)
        {
            DateTime StartTime = DateTime.Now;

            //ScriptEngine = ScriptEngine_t.Python;
            SCPFileName = myScriptFileName;
            //LogResults("Start LoadScript: " + myScriptFileName);

            if (!DontUpdateScriptBox) cbxScriptNames.Items.Clear();

            if (!File.Exists(myScriptFileName))
            {
                LogResults("Error.  Script not found: " + myScriptFileName);
                return;
            }

            FileInfo fi = new FileInfo(myScriptFileName);
            //SCPFileFolder = fi.DirectoryName;

            myPythonHandler = new PythonScripting(this, myScriptFileName);

            if (myPythonHandler.CompileErrors != "")
            {
                LogResults("Script " + fi.Name + " Contains errors.  Double-click on the below error lines to open the associated editor at the specified line.");
                DisplayCompileErrors(myPythonHandler.CompileErrors);
            }

            if (!DontUpdateScriptBox)
            {
                foreach (string name in myPythonHandler.MethodNames)
                {
                    //Maybe later:  Create a method delegate on load:
                    //ScriptStruct myScriptStruct = new ScriptStruct();
                    //myScriptStruct.Name = name;
                    //ScriptDomain.GetStaticMethod(name);
                    cbxScriptNames.Items.Add(name);
                }
                if (cbxScriptNames.Items.Count > 0)
                {
                    cbxScriptNames.Text = cbxScriptNames.Items[0].ToString();
                }
            }

            DateTime EndTime = DateTime.Now;

            var ScriptLoadTime = EndTime - StartTime;
            var ScriptCompileTime = myPythonHandler.EndCompileTime - myPythonHandler.StartCompileTime;
            LogResults("Finished loading script: " + myScriptFileName + " Time taken = " + ScriptLoadTime.TotalSeconds.ToString("0.000") + "  Compile time = " + ScriptCompileTime.TotalSeconds.ToString("0.000"));

        }

        void DisplayCompileErrors(string ScriptCompileErrors)
        {
            int ErrorEndIndex;
            if (ScriptCompileErrors != "")
            {
                string[] splitters = { "\r\n" };
                string[] AllErrors;

                ErrorEndIndex = ScriptCompileErrors.IndexOf("\r\n   at", 0);
                if (ErrorEndIndex > 0)
                {
                    ScriptCompileErrors = ScriptCompileErrors.Substring(0, ErrorEndIndex);
                }

                AllErrors = ScriptCompileErrors.Split(splitters, StringSplitOptions.None);

                foreach (string Error in AllErrors)
                {
                    if (Error != "") lstResults.Items.Add(Error);
                }
            }
        }


        private void btnAbortScript_Click(object sender, EventArgs e)
        {
            if (ScriptRunning)
            {
                AbortScript = true;

                //Request cancellation.. wait 2 seconds, then Abort the thread forcibly if it's not quitting on its own.
                for (int i = 0; i < 200; i++)
                {
                    System.Threading.Thread.Sleep(10);
                    Application.DoEvents();
                    if (!ScriptRunning) break;
                }

                //if (ScriptRunning)
                //{
                //    ScriptThread.Abort();
                //    lstResults.Items.Add("Script Abort request failed.  Forcibly aborted the script. (Potentially unsafe)");
                //}
                //else
                //{
                //    lstResults.Items.Add("Script Abort request successful.");
                //}
            }
        }

        private void btnRunScript_Click(object sender, EventArgs e)
        {
            if (cbxResetExecEnv.Checked)
            {
                //reload the script file:
                LoadScript(SCPFileName, true);
            }

            string ScriptName = "";

            if (cbxScriptNames.Items.Count < 1) return;

            //if (ScriptsComboBox.SelectedIndex >= 0 && ScriptsComboBox.SelectedIndex < ScriptsComboBox.Items.Count)
            //{
            //    ScriptName = ScriptsComboBox.Items[ScriptsComboBox.SelectedIndex].ToString();
            //}
            ScriptName = cbxScriptNames.Text;

            RunScript(ScriptName);
        }

        private void RunScript(string ScriptName)
        {
            txtOutput.Text = "";

            AbortScript = false;

            try
            {
                //python
                //if (!myPythonHandler.MethodNames.Contains(ScriptName))
                //{
                //    LogResults("RunScript Error: Script not found: '" + ScriptName + "'");
                //}
                myPythonHandler.InvokeMethod(ScriptName);
                if (myPythonHandler.CompileErrors != "")
                {
                    DisplayCompileErrors(myPythonHandler.CompileErrors);
                }
            }
            catch (Exception ex)
            {
                HandleException((new System.Diagnostics.StackTrace(true)).GetFrame(0).GetFileName().ToString() + ":" + (new System.Diagnostics.StackTrace(true)).GetFrame(0).GetFileLineNumber().ToString(), System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex);
            }
        }


        private void btnEval_Click(object sender, EventArgs e)
        {
            string Script;
            Script = myUtils.InputBox("Enter script", "Enter C# script", "Host.Display(\"A\");");
            if (Script == "") return;

            //myScriptHandler.EvaluateCS(Script);
        }

        public void HandleException(string FileName, string FunctionName, Exception e)
        {
            // Purpose    : Provides a central mechanism for exception handling.
            //            : Displays a message box that describes the exception.

            // Accepts    : moduleName - the module where the exception occurred.
            //            : e - the exception

            try
            {
                // Create an error message.
                LogResults("----------------", true, true);
                LogResults("Exception: " + e.Message, true, true);
                LogResults("File: " + FileName, true, true);
                LogResults("Function: " + FunctionName, true, true);
                LogResults("Exception source: " + e.TargetSite.Name, true, true);
            }
            finally { }

        }

        private void btnExecuteScript_Click(object sender, EventArgs e)
        {

        }

        private void btnClearResults_Click(object sender, EventArgs e)
        {
            lstResults.Items.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadScript(Application.StartupPath + @"\test_simple.py");
        }
    }
}
