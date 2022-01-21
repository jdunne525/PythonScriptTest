using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronPython.Hosting;
using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Utils;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PythonScriptTest;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Scripting
{

    class PythonScripting
    {
        public string ScriptFile;

        public string[] MethodNames;
        public string CompileErrors;
        public string ScriptCompileErrors;
        object myHostReference;
        Form1 Host;
        string LastLogCodeToExecute = "";
        string LastMeterLogCodeToExecute = "";
        string ScriptCode;
        public Boolean LogItemsValid = false;
        public Boolean MeterItemsValid = false;

        public DateTime StartCompileTime;
        public DateTime EndCompileTime;

        string LogItemScriptPath = "";
        string MeterItemScriptPath = "";
        //public string[] LogItemScriptNames = new string[20];

        ScriptEngine engine;
        ScriptScope scope;
        ScriptSource source;
        ScriptSource MeterSource;

        public PythonScripting(Form1 ObjectReference, string ScriptFile)
        {
            Host = ObjectReference;
            myHostReference = ObjectReference;
            this.ScriptFile = ScriptFile;

            CompileErrors = "";
            ScriptCompileErrors = "";

            engine = Python.CreateEngine();     //~2-3mS
            scope = engine.CreateScope();

            //diag.. disable this
            //Pass the host form to the scripting environment as a global:
            //scope.SetVariable("Host", myHostReference);

            MethodNames = new string[0];

            if (ScriptFile == "") return;

            ScriptCode = "";

            if (File.Exists(ScriptFile))
            {
                StreamReader ScriptStream = new StreamReader(ScriptFile);
                ScriptCode = ScriptStream.ReadToEnd();
                ScriptStream.Close();
            }

            FileInfo fi = new FileInfo(ScriptFile);

            var paths = engine.GetSearchPaths();
            paths.Add(Application.StartupPath + "\\Scripting");         //add scripting folder
            paths.Add(fi.Directory.ToString());
            //paths.Add(Host.SCPPath);

            //string pythonlib = Host.ini.ReadValue("SUITESETTINGS", "PythonLib");
            string pythonlib  = @"C:\Program Files\IronPython 2.7";
            pythonlib = pythonlib.Replace("%LOCALAPPDATA%", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            pythonlib = pythonlib.Replace("%PROGRAMDATA%", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));

            if (Directory.Exists(pythonlib))
            {
                paths.Add(pythonlib);
            }

            //Write the python sys path to a file so the script can pull it in:
            //This allows vscode or other editors to utilize the same set of paths that hidsuite uses.
            //C:\Users\jdunne\AppData\Local\Tripp Lite\hidsuite3_settings\scriptpath.cfg
            string scriptpathsettingfolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Tripp Lite\hidsuite3_settings\";
            if (!Directory.Exists(scriptpathsettingfolder))
            {
                Directory.CreateDirectory(scriptpathsettingfolder);
            }
            string scriptpathsettingfile = scriptpathsettingfolder + "scriptpath.cfg";
            var OutFile = new StreamWriter(scriptpathsettingfile);
            foreach (var path in paths)
            {
                OutFile.WriteLine(path);
            }
            OutFile.Close();

            engine.SetSearchPaths(paths);

            //The above takes 1mS or less.
            try
            {
                //This method causes the file to get locked until the application closes:
                //ScriptSource source = engine.CreateScriptSourceFromFile(ScriptFile);

                source = engine.CreateScriptSourceFromString(ScriptCode, SourceCodeKind.Statements);
                StartCompileTime = DateTime.Now;
                source.Execute(scope);              //100mS
                EndCompileTime = DateTime.Now;
                source.Compile();

            }
            catch (Exception ex)
            {
                CompileErrors = FormatException(ScriptFile, ex);
                ScriptCompileErrors = CompileErrors;
            }
            //source.C

            scope.SetVariable("Host", myHostReference);
            scope.SetVariable("ScriptHelper.Host", myHostReference);
            scope.SetVariable("ScriptStubs.Host", myHostReference);

            try
            {
                MethodNames = GetMethodNames();
            }
            catch (Exception ex)
            {
                CompileErrors += FormatException(ScriptFile, ex);
            }
        }

        private string FormatException(string FileName, Exception ex)
        {
            string errors = engine.GetService<ExceptionOperations>().FormatException(ex).ToString();

            var pattern = "File \".*\", line ([0-9]+)";
            if (Regex.IsMatch(errors, pattern))
            {
                return Regex.Replace(errors, pattern, FileName + "($1,0):").Trim();
            }
            else
            {
                return "-1";
            }
        }

        //execute a single expression and return the result in a dynamic type.  For example: dynamic result = Execute("2+2")
        public dynamic Eval(string OneLiner)
        {
            try
            {
                ScriptSource source = engine.CreateScriptSourceFromString(OneLiner, SourceCodeKind.Expression);
                return source.Execute(scope);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //execute a single expression and return the result in a dynamic type.  For example: dynamic result = Execute("2+2")
        public dynamic EvalNoCatch(string OneLiner)
        {
            ScriptSource source = engine.CreateScriptSourceFromString(OneLiner, SourceCodeKind.Expression);
            return source.Execute(scope);
        }

        //Check if the presently loaded script has access to the Display() function provided by ScriptHelper.py
        public bool ScriptHelperValid()
        {
            dynamic check = false;
            try
            {
                check = engine.Execute(@"'Display' in dir()", scope);
                if ((bool)check)
                {
                    check = engine.Execute(@"'sys' in dir()", scope);
                    if ((bool)check)
                    {
                        //Check if this module contains a Host attribute:
                        check = engine.Execute(@"hasattr(sys.modules[__name__], 'Host')", scope);
                    }
                }
            }
            catch (Exception) // ex) // warning CS0168 - The variable is declared but never used
            {
                return false;
            }

            //If we have a Display function, then we have a functional ScriptHelper.
            return (bool)check;
        }

//        /// <summary>
//        /// Add ScriptStubs.py and ScriptHelper.py to the script scope
//        /// </summary>
//        public void LoadScriptHelperAndStubs()
//        {
//#if LoadHelpersOldMethod
//            //stubs goes first since it contains all public functions.
//            string ScriptStubsPath = Application.StartupPath + "\\Scripting\\ScriptStubs.py";
//            try
//            {

//                //Add script helper class:
//                if (!File.Exists(ScriptStubsPath))
//                {
//                    throw new Exception("Could not find: " + ScriptStubsPath);
//                }
//                var ScriptStubs = File.ReadAllText(ScriptStubsPath);

//                ScriptSource source = engine.CreateScriptSourceFromString(ScriptStubs, SourceCodeKind.Statements);
//                source.Execute(scope);
//                source.Compile();
//            }
//            catch (Exception ex)
//            {
//                CompileErrors = FormatException(ScriptStubsPath, ex);
//            }

//            //Helper goes after so it can override any functions supplied by stubs.
//            string ScriptHelperPath = Application.StartupPath + "\\Scripting\\ScriptHelper.py";
//            try
//            {

//                //Add script helper class:
//                if (!File.Exists(ScriptHelperPath))
//                {
//                    throw new Exception("Could not find: " + ScriptHelperPath);
//                }
//                var ScriptHelper = File.ReadAllText(ScriptHelperPath);

//                ScriptSource source = engine.CreateScriptSourceFromString(ScriptHelper, SourceCodeKind.Statements);
//                source.Execute(scope);
//                source.Compile();
//            }
//            catch (Exception ex)
//            {
//                CompileErrors = FormatException(ScriptHelperPath, ex);
//            }
//#endif

//            string ScriptHeader = @"

//global Host
//Host = None
//import clr
//import System
//import sys
//sys.path.append(r'C:\ProgramData\Tripp Lite\hidsuite3\hidsuite3_stubs')     #This line is to support vscode's autocomplete
//sys.path.pop()          #undo the previous sys.path.append().  (The python parser doesn't pay attention to this line.))
//import ScriptSubs
//import ScriptHelper
//from ScriptStubs import *
//from ScriptHelper import *
//clr.AddReference('System.Windows.Forms')
//clr.AddReference('System.IO')
//clr.AddReference('System')
//clr.AddReference('Microsoft.VisualBasic')
//from System.Windows.Forms import *      #MessageBox, MessageBoxButtons, DialogResult, etc.
//from System.IO import *      #File.Exists, etc
//from Microsoft.VisualBasic import *     #Strings

//def __init__():
//    ScriptHelper.Host = Host
//    ScriptStubs.Host = Host
//";

//            var paths = engine.GetSearchPaths();
//            paths.Add(Application.StartupPath + "\\Scripting");         //add scripting folder
//            paths.Add(Host.SCPPath);

//            string pythonlib = Host.ini.ReadValue("SUITESETTINGS", "PythonLib");
//            if (File.Exists(pythonlib))
//            {
//                paths.Add(pythonlib);
//            }

//            pythonlib = Host.ini.ReadValue("SUITESETTINGS", "PythonLib");
//            if (File.Exists(@"C:\Program Files\IronPython 2.7\Lib"))
//            {
//                paths.Add(@"C:\Program Files\IronPython 2.7\Lib");
//            }
//            engine.SetSearchPaths(paths);

//            try
//            {
//                ScriptSource source = engine.CreateScriptSourceFromString(ScriptHeader, SourceCodeKind.Statements);
//                source.Execute(scope);
//                source.Compile();
//            }
//            catch (Exception ex)
//            {
//                CompileErrors = FormatException("", ex);
//                Host.LogResults(CompileErrors);
//                return;
//            }

//            scope.SetVariable("Host", myHostReference);
//            scope.SetVariable("ScriptHelper.Host", myHostReference);
//            scope.SetVariable("ScriptStubs.Host", myHostReference);

//            engine.Execute("__init__()", scope);
//        }

        public dynamic Evaluate(string ScriptCode)
        {
            CompileErrors = "";
            dynamic result = null;
            try
            {
                scope.SetVariable("Script", myHostReference);
                result = Eval(ScriptCode);
            }
            catch (Exception ex)
            {
                CompileErrors = FormatException("HidSuiteSource:PythonScripting.cs", ex);
            }

            return result;

            //            string CodeToExecute;
            //            string ReturnValue = "";
            //            CompileErrors = "";

            //            CodeToExecute = @"
            //def EvalFunc():
            //    " + ScriptCode + @"
            //";

            //            try
            //            {

            //                source = engine.CreateScriptSourceFromString(CodeToExecute, SourceCodeKind.Statements);

            //                //Pass the host form to the scripting environment as a global:
            //                //scope.SetVariable("Host", myHostReference);           //already done at creation
            //                scope.SetVariable("Script", myHostReference);

            //                source.Execute(scope);

            //                //Get a function pointer to the method (this passes a parameter to the function)
            //                //var fRunHostMethod = scope.GetVariable<Func<string, string>>("EvalFunc");

            //                var fRunHostMethod = scope.GetVariable("EvalFunc");

            //                fRunHostMethod();
            //            }
            //            catch (Exception ex)
            //            {
            //                CompileErrors = FormatException(ex);
            //            }

            //return;
        }

        public string[] GetMethodNames()
        {
            IList<string> listresult;
            System.Collections.ArrayList FunctionNames = new System.Collections.ArrayList();

            //get method names from python directly:
            dynamic result = engine.Execute(@"dir()", scope);
            listresult = ((IList<object>)result).Cast<string>().ToList();

            MatchCollection matches;
            var pattern = @"def (\w+)\(";
            matches = Regex.Matches(ScriptCode, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            foreach (Match match in matches)
            {
                //retrieve the index 1 capture group within the match, which is our method name:
                string method = match.Groups[1].ToString();

                if (method == "__init__")
                {
                    //If the script has an __init__() function, call it.
                    engine.Execute("__init__()", scope);
                }
                else if (method != "Host" && 
                    (
                    method.StartsWith("__")     //normal Python private function prefix is __
                    || method.StartsWith("_")   //HIDSuite internal functions use a prefix of _.  Single underscore functions in hidsuite are also hidden from the dropdown box.
                    || method.StartsWith("LogItem_")
                    )
                    )
                {
                    //__ prefixed functions are private.  don't add them to the list.
                    //don't add this to the list.
                }
                else
                {
                    //Only add the method if python recognizes it:
                    if (listresult.Contains(method))
                        FunctionNames.Add(method);
                }
            }

            return (string[])FunctionNames.ToArray(typeof(string));
        }

        public void InvokeMethod(string MethodName)
        {
            try
            {
                //scriptHelper.InvokeInst(ScriptInstance, "*." + MethodName);
                var FuncToRun = scope.GetVariable(MethodName);
                FuncToRun();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("unsupported operand type(s)"))
                {
                    //TODO: use python ast and astunparse:
                    //pip install astunparse

                    //astunparse is now here:
                    //C:\Users\jdunne\AppData\Local\Programs\Python\Python37-32\Lib\site-packages\astunparse


                    /*
import ast
import astunparse
# tree = ast.parse("print('hello world')")

# Error line in question:
# print('Input Line Voltage=' + float(8+5) / 10)

tree = ast.parse("print('Input Line Voltage=' + int(8+5) / 10 + 'Amps)'")
print("Start")
# for fieldname, value in ast.iter_fields(tree):
# print(fieldname)
# print(value)
# print("Type: " type(value))

# print(astunparse.dump(tree))

for node in ast.walk(tree):
    if isinstance(node, ast.BinOp):
        if (isinstance(node.op, ast.Add)):
# print(f'Nodetype: {type(node).__name__:{16}} {node}')
            print("Left: " + astunparse.unparse(node.left))
            print("Right: " + astunparse.unparse(node.right))
            print(astunparse.unparse(node))

print("Done")
                        */


                    //OUTPUT:
                    /*
Start
Left: 'Input Line Voltage='
Right: (float((8 + 5)) / 10)
('Input Line Voltage=' + (float((8 + 5)) / 10))
Left: 8
Right: 5
(8 + 5)
Done
                     */




                }
                CompileErrors = FormatException(ScriptFile, ex);
            }
        }
        public string GetLogItem(DataGridViewRow LogItem)
        {
            dynamic Result = "No Response Given";
            CompileErrors = "";
            try
            {
                if (!Host.myUtils.IsNullOrEmpty(LogItem.Cells["ScriptName"].Value))
                {
                    var FuncToRun = scope.GetVariable(LogItem.Cells["ScriptName"].Value.ToString());
                    scope.SetVariable("Item", "No Response Given");         //initialize a global variable called "Item" (to allow differentation from null, which is a valid script result)
                    FuncToRun();                                            //LogItem script modifies this global variable.
                    Result = scope.GetVariable("Item");                     //Return the resulting value.
                    //engine.Runtime.Globals.
                }

                if (Result == null) Result = "-1";
                else
                {
                    if (Result.GetType() == typeof(String))
                    {
                        if (Result == "No Response Given") Result = "";
                    }
                    else
                    {
                        Result = Result.ToString();
                    }
                }
                return Result;
            }
            catch (Exception ex)
            {
                ((Form1)myHostReference).LogResults(ex.Message);
                Host.LogResults("----------error in GetLogItem------------");
                Host.HandleException((new System.Diagnostics.StackTrace(true)).GetFrame(0).GetFileName().ToString() + ":" + (new System.Diagnostics.StackTrace(true)).GetFrame(0).GetFileLineNumber().ToString(), System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex);
                //Host.LogResults("GetLogItem:" + index + Host.ItemsTextBox[index].Text);
                CompileErrors = FormatException(LogItemScriptPath, ex);
                return "-1";
            }
        }

        public string GetMeterItem(DataGridViewRow MeterItem)
        {
            CompileErrors = "";
            try
            {
                dynamic Result = "No Response Given";
                if (!Host.myUtils.IsNullOrEmpty(MeterItem.Cells["MeterScriptName"].Value))
                {
                    var FuncToRun = scope.GetVariable(MeterItem.Cells["MeterScriptName"].Value.ToString());
                    scope.SetVariable("Item", "No Response Given");         //initialize a global variable called "Item" (to allow differentation from null, which is a valid script result)
                    FuncToRun();                                            //LogItem script modifies this global variable.
                    Result = scope.GetVariable("Item");                     //Return the resulting value.
                }

                if (Result == null) Result = "-1";
                else
                {
                    if (Result.GetType() == typeof(String))
                    {
                        if (Result == "No Response Given") Result = "";
                    }
                    else
                    {
                        Result = Result.ToString();
                    }
                }
                return Result;
            }
            catch (Exception ex)
            {
                ((Form1)myHostReference).LogResults(ex.Message);
                Host.LogResults("----------error in GetLogItem------------");
                Host.HandleException((new System.Diagnostics.StackTrace(true)).GetFrame(0).GetFileName().ToString() + ":" + (new System.Diagnostics.StackTrace(true)).GetFrame(0).GetFileLineNumber().ToString(), System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex);
                CompileErrors = FormatException(MeterItemScriptPath, ex);
                return "-1";
            }
        }

        public string InitMeterItem(DataGridViewRow MeterItem)
        {
            CompileErrors = "";
            try
            {
                dynamic Result = "No Response Given";
                if (!Host.myUtils.IsNullOrEmpty(MeterItem.Cells["MeterScriptName"].Value))
                {
                    var FuncToRun = scope.GetVariable("Init" + MeterItem.Cells["MeterScriptName"].Value.ToString());
                    scope.SetVariable("Item", "No Response Given");         //initialize a global variable called "Item" (to allow differentation from null, which is a valid script result)
                    FuncToRun();                                            //LogItem script modifies this global variable.
                    Result = scope.GetVariable("Item");                     //Return the resulting value.
                }

                if (Result == null) Result = "-1";
                else
                {
                    if (Result.GetType() == typeof(String))
                    {
                        if (Result == "No Response Given") Result = "";
                    }
                    else
                    {
                        Result = Result.ToString();
                    }
                }
                return Result;
            }
            catch (Exception ex)
            {
                ((Form1)myHostReference).LogResults(ex.Message);
                Host.LogResults("----------error in Initializing Meter------------");
                Host.HandleException((new System.Diagnostics.StackTrace(true)).GetFrame(0).GetFileName().ToString() + ":" + (new System.Diagnostics.StackTrace(true)).GetFrame(0).GetFileLineNumber().ToString(), System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex);
                CompileErrors = FormatException(MeterItemScriptPath, ex);
                return "-1";
            }

        }

        string ScriptFileLogItems = "";
        public void LoadLogItemsFromScriptFile(DataGridView LogItems)
        {
            var sFile = File.ReadAllLines(ScriptFile);

            string ScriptCode;
            int itemnumber;
            int charnum;
            string FileLine;
            int i;
            string LogItemIndent = "";

            LogItems.Rows.Clear();

            itemnumber = 0;
            for (int lineindex = 0; lineindex < sFile.Length; lineindex++)
            {
                string ScriptName = "";
                string ItemName = "";
                string ItemFormula = "";

                //read one line at a time:
                FileLine = sFile[lineindex];

                string SpaceLine = FileLine.Replace("\t", " ").Replace("  ", " ");

                Match myMatch = Regex.Match(SpaceLine, @"def\s+(LogItem_\w*) *\(");
                if (myMatch.Success)
                {
                    //Save function name:
                    //LogItemScriptNames[itemnumber] = myMatch.Groups[1].Value;
                    ScriptName = myMatch.Groups[1].Value;

                    charnum = Strings.InStr(FileLine, "#");
                    if (charnum != 0)
                        FileLine = FileLine.Substring(charnum); // Retrieve comment which contains item names
                    FileLine = FileLine.Trim();

                    //Handle spaces in log item names:
                    ItemName = FileLine.Replace("    ", "\t");      //replace 4 spaces with a tab character
                    ItemName = ItemName.Replace("   ", "\t");       //replace 3 spaces with a tab character
                    ItemName = ItemName.Replace("  ", "\t");        //replace 2 spaces with a tab character

                    ScriptCode = "";
                    // we found an item:
                    for (lineindex++; lineindex < sFile.Length; lineindex++)
                    {
                        FileLine = sFile[lineindex];
                        if (FileLine.ToLower().StartsWith("def"))
                        {
                            lineindex--;
                            break;
                        }

                        if (!string.IsNullOrEmpty(FileLine))
                        {
                            // strip comments from lines.
                            i = Conversions.ToInteger(Strings.InStr(1, FileLine, "#"));
                            if (i != 0)
                                FileLine = Strings.Left(FileLine, i - 1);

                            FileLine = Host.StripTrailingSpaces(FileLine);
                            if (FileLine.Length > 0 && !FileLine.EndsWith(";"))
                            {
                                FileLine = FileLine + ";";          //append semicolon to all lines
                            }

                            if (FileLine.Trim().Length > 0 && LogItemIndent == "")
                            {
                                for (i = 0; i < FileLine.Length; i++)
                                {
                                    if (FileLine[i] != ' ' && FileLine[i] != '\t')
                                    {
                                        if (i > 0)
                                        {
                                            //valid indented code line:
                                            LogItemIndent = FileLine.Substring(0, i);
                                        }
                                        break;
                                    }
                                }
                            }

                            if (LogItemIndent != "" && FileLine.StartsWith(LogItemIndent))
                            {
                                //omit the initial function indent:
                                FileLine = FileLine.Substring(LogItemIndent.Length);
                            }

                            if (FileLine.Trim() != "global Item;")
                            {
                                ScriptCode = ScriptCode + FileLine + "   ";
                            }
                        }
                        ItemFormula = ScriptCode.TrimStart(new char[] { ' ' });
                        
                    }
                    if (ItemName != "" && ItemFormula != "")
                    {
                        var index =  LogItems.Rows.Add();
                        LogItems.Rows[index].Cells["ScriptName"].Value = ScriptName;
                        LogItems.Rows[index].Cells["Enabled"].Value = true;
                        LogItems.Rows[index].Cells["Item"].Value = ItemName;
                        LogItems.Rows[index].Cells["Formula"].Value = ItemFormula;
                        
                    }
                }
            }

            ScriptFileLogItems = "";
            for (i = 0; i < LogItems.Rows.Count; i++)
            {
                if ((bool)LogItems.Rows[i].Cells["Enabled"].FormattedValue == true)
                        ScriptFileLogItems += LogItems.Rows[i].Cells["ScriptName"].Value.ToString() + ":" + LogItems.Rows[i].Cells["Formula"].Value.ToString();
            }

            if (itemnumber == 1)
                // MsgBox ("script not found")
                return;
        }

        //string[] LogItemsText = new string[20];
        public void LoadLogItems(DataGridView LogItems)
        {
            string CodeToExecute;

            CompileErrors = "";

            CodeToExecute = "";
            for (int i = 0; i < LogItems.Rows.Count; i++)
            {
                if((bool)LogItems.Rows[i].Cells["Enabled"].FormattedValue == true && !Host.myUtils.IsNullOrEmpty(LogItems.Rows[i].Cells["ScriptName"].Value))
                    CodeToExecute += LogItems.Rows[i].Cells["ScriptName"].Value.ToString() + ":" + LogItems.Rows[i].Cells["Formula"].Value.ToString();
            }

            if (CodeToExecute == ScriptFileLogItems)
            {
                //Logitems match the script file, so just run the scripts from there.
                LogItemScriptPath = ScriptFile;
                return;
            }

            if (LogItemScriptPath != "" && LogItemScriptPath.EndsWith(".tmp.py"))
            {
                //remove the old temp script file:
                if (File.Exists(LogItemScriptPath))
                {
                    File.Delete(LogItemScriptPath);
                }
            }

            CodeToExecute = @"
#--------------------------
# IMPORTANT:  Note this TEMPORARY script file is only to aid in identifying errors.  Changes made will NOT be reflected anywhere.
# Corrections should be made to the original .py script file (preferred) or in the logitem view in hidsuite.
# TEMPORARY SCRIPT FILE.  Do not modify.
# TEMPORARY SCRIPT FILE.  Do not modify.
# TEMPORARY SCRIPT FILE.  Do not modify.
# TEMPORARY SCRIPT FILE.  Do not modify.
# TEMPORARY SCRIPT FILE.  Do not modify.
# TEMPORARY SCRIPT FILE.  Do not modify.
# TEMPORARY SCRIPT FILE.  Do not modify.
#--------------------------
";
            //Create a named function for each logitem:
            for (int i = 0; i < LogItems.Rows.Count; i++)
            {
                if ((bool)LogItems.Rows[i].Cells["Enabled"].FormattedValue != true || Host.myUtils.IsNullOrEmpty(LogItems.Rows[i].Cells["Formula"].Value))
                    continue;

                if (LogItems.Rows[i].Cells["Formula"].Value.ToString().Contains("global Item"))
                {
                    CodeToExecute += @"
def " + LogItems.Rows[i].Cells["ScriptName"].Value.ToString() + @"():
    " + LogItems.Rows[i].Cells["Formula"].Value.ToString().Replace("«", "\r\n    ") + @"
";
                }
                else
                {
                    CodeToExecute += @"
def " + LogItems.Rows[i].Cells["ScriptName"].Value.ToString() + @"():
    global Item
    " + LogItems.Rows[i].Cells["Formula"].Value.ToString().Replace("«", "\r\n    ") + @"
";
                }
            }

            if (LastLogCodeToExecute == CodeToExecute)
            {
                //Cached log script is unchanged.  No need to recompile.
                return;
            }

            LogItemsValid = false;
            LastLogCodeToExecute = CodeToExecute;

            LogItemScriptPath = System.IO.Path.GetTempFileName() + ".py";
            File.WriteAllText(LogItemScriptPath, CodeToExecute);

            try
            {

                source = engine.CreateScriptSourceFromString(CodeToExecute, SourceCodeKind.Statements);

                //Pass the host form to the scripting environment as a global:
                //scope.SetVariable("Host", myHostReference);           //already done at creation
                scope.SetVariable("Script", myHostReference);
                source.Execute(scope);                                  //not sure what this will actually do..

                source.Compile();
                LogItemsValid = true;
            }
            catch (Exception ex)
            {
                CompileErrors = FormatException(LogItemScriptPath, ex);
                //LogItemsValid = false;
            }
        }


        public void LoadMeterItems(DataGridView MeterItems)
        {
            string CodeToExecute;

            CompileErrors = "";

            // TODO This code is to allow caching of scripts
            // it shoudl be reworked once Load / Save are written
            //CodeToExecute = "";
            //foreach( DataGridViewRow row in MeterItems.Rows)
            //{
            //    if ((bool)LogItems.Rows[i].Cells["Enabled"].FormattedValue == true && !Host.myUtils.IsNullOrEmpty(LogItems.Rows[i].Cells["ScriptName"].Value))
            //        CodeToExecute += LogItems.Rows[i].Cells["ScriptName"].Value.ToString() + ":" + LogItems.Rows[i].Cells["Formula"].Value.ToString();
            //}

            //if (CodeToExecute == ScriptFileLogItems)
            //{
            //    //Logitems match the script file, so just run the scripts from there.
            //    LogItemScriptPath = ScriptFile;
            //    return;
            //}


            CodeToExecute = @"
#--------------------------
# IMPORTANT:  Note this TEMPORARY script file is only to aid in identifying errors.  Changes made will NOT be reflected anywhere.
# Corrections should be made to the original .py script file (preferred) or in the logitem view in hidsuite.
# TEMPORARY SCRIPT FILE.  Do not modify.
# TEMPORARY SCRIPT FILE.  Do not modify.
# TEMPORARY SCRIPT FILE.  Do not modify.
# TEMPORARY SCRIPT FILE.  Do not modify.
# TEMPORARY SCRIPT FILE.  Do not modify.
# TEMPORARY SCRIPT FILE.  Do not modify.
# TEMPORARY SCRIPT FILE.  Do not modify.
#--------------------------
";
            //Create a named function for each logitem:
            foreach (DataGridViewRow row in MeterItems.Rows)
            {
                if ((bool)row.Cells["MeterEnabled"].FormattedValue != true || Host.myUtils.IsNullOrEmpty(row.Cells["MeterFormula"].Value))
                    continue;

                if (row.Cells["MeterFormula"].Value.ToString().Contains("global Item"))
                {
                    CodeToExecute += @"
def " + row.Cells["MeterScriptName"].Value.ToString() + @"():
    " + row.Cells["MeterFormula"].Value.ToString().Replace("«", "\r\n    ") + @"
";
                }
                else
                {
                    CodeToExecute += @"
def " + row.Cells["MeterScriptName"].Value.ToString() + @"():
    global Item
    " + row.Cells["MeterFormula"].Value.ToString().Replace("«", "\r\n    ") + @"
";
                }

                if(!Host.myUtils.IsNullOrEmpty(row.Cells["MeterInitScript"]))
                {
                    if (row.Cells["MeterInitScript"].Value.ToString().Contains("global Item"))
                    {
                        CodeToExecute += @"
def Init" + row.Cells["MeterScriptName"].Value.ToString() + @"():
    " + row.Cells["MeterInitScript"].Value.ToString().Replace("«", "\r\n    ") + @"
";
                    }
                    else
                    {
                        CodeToExecute += @"
def Init" + row.Cells["MeterScriptName"].Value.ToString() + @"():
    global Item
    " + row.Cells["MeterInitScript"].Value.ToString().Replace("«", "\r\n    ") + @"
";
                    }

                }



            }

            if (LastMeterLogCodeToExecute == CodeToExecute)
            {
                //Cached log script is unchanged.  No need to recompile.
                return;
            }

            MeterItemsValid = false;
            LastMeterLogCodeToExecute = CodeToExecute;


            if (MeterItemScriptPath != "" && MeterItemScriptPath.EndsWith(".tmp.py"))
            {
                //remove the old temp script file:
                if (File.Exists(MeterItemScriptPath))
                {
                    File.Delete(MeterItemScriptPath);
                }
            }
            MeterItemScriptPath = System.IO.Path.GetTempFileName() + ".py";
            File.WriteAllText(MeterItemScriptPath, CodeToExecute);

            try
            {

                MeterSource = engine.CreateScriptSourceFromString(CodeToExecute, SourceCodeKind.Statements);

                //Pass the host form to the scripting environment as a global:
                //scope.SetVariable("Host", myHostReference);           //already done at creation
                scope.SetVariable("Script", myHostReference);
                MeterSource.Execute(scope);                                  //not sure what this will actually do..

                MeterSource.Compile();
                MeterItemsValid = true;
            }
            catch (Exception ex)
            {
                CompileErrors = FormatException(MeterItemScriptPath, ex);
                //LogItemsValid = false;
            }
        }



    } //Class

} //NAMESPACE

