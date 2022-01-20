using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace Utility
{
    public class Utilities
    {
        public readonly string CHARSNUMS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        //public bool myIsNumeric(string item)
        //{
        //    int result;
        //    if(Int32.TryParse(item, out result))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //returns true if the specified string contains a numeric value.
        public bool myIsNumeric(string ObjectToTest)
        {
            if (ObjectToTest == null)
            {
                return false;

            }
            else
            {
                double OutValue;
                return double.TryParse(ObjectToTest.ToString().Trim(),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.CurrentCulture,
                    out OutValue);
            }
        }

        //converts a string input numerical value (hex or decimal) to an integer.
        //Returns 0 on failure to convert
        public int StringToInt(string stringToConvert)
        {
            bool success;
            return StringToInt(stringToConvert, out success);
        }

        //converts a string input numerical value (hex or decimal) to an integer.
        //Returns 0 on failure to convert
        public int StringToInt(string stringToConvert, out bool success)
        {
            int returnvalue;
            System.Globalization.NumberStyles style;
            style = System.Globalization.NumberStyles.Number;        //default style
            if (stringToConvert.Contains("0x"))
            {
                style = System.Globalization.NumberStyles.AllowHexSpecifier; //hex style
                stringToConvert = stringToConvert.Replace("0x", "");
            }

            success = int.TryParse(stringToConvert, style, System.Globalization.NumberFormatInfo.CurrentInfo, out returnvalue);
            if (!success) returnvalue = 0;
            return (returnvalue);
        }

        //converts a string input numerical value (hex with preceding "0x" or decimal) to an integer.
        //Returns 0 on failure to convert
        public uint StringToUint(string stringToConvert)
        {
            bool success;
            return StringToUint(stringToConvert, out success);
        }

        //converts a string input numerical value (hex with preceding "0x" or decimal) to an unsigned integer.
        //Returns 0 on failure to convert
        public uint StringToUint(string stringToConvert, out bool success)
        {
            uint returnvalue;
            System.Globalization.NumberStyles style;
            style = System.Globalization.NumberStyles.Number;        //default style
            if (stringToConvert.Contains("0x"))
            {
                style = System.Globalization.NumberStyles.AllowHexSpecifier; //hex style
                stringToConvert = stringToConvert.Replace("0x", "");
            }

            success = uint.TryParse(stringToConvert, style, System.Globalization.NumberFormatInfo.CurrentInfo, out returnvalue);
            if (!success) returnvalue = 0;
            return (returnvalue);
        }

        public byte[] IntToByteArray(int inputval)
        {
            byte[] returnval = new byte[4];
            returnval[0] = (byte)(inputval & 0xFF);
            returnval[1] = (byte)((inputval & 0xFF00) >> 8);
            returnval[2] = (byte)((inputval & 0xFF0000) >> 16);
            returnval[3] = (byte)((inputval & 0xFF000000) >> 24);
            return returnval;
        }

        public int ByteArrayToInt(byte[] byteArray, int index)
        {
            int returnval = 0;

            if (byteArray.Length < (4 + index))
            {
                return -1;     //FAIL
            }

            if (byteArray.Length > 0) {
                returnval = byteArray[index];
            }

            if (byteArray.Length > 1)
            {
                returnval += byteArray[index + 1] << 8;
            }
            if (byteArray.Length > 2)
            {
                returnval += byteArray[index + 2] << 16;
            }
            if (byteArray.Length > 3)
            {
                returnval += byteArray[index + 3] << 24;
            }
            return returnval;
        }

        public string ByteArrayToString(byte[] inputarray)
        {
            if (inputarray == null)
            {
                return ("");
            }

            string s = System.Text.ASCIIEncoding.ASCII.GetString(inputarray);
            return s;
        }

        public byte[] StringToByteArray(string inputstring)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(inputstring);
        }

        /// <summary>
        /// Convert a byte array to a hex string.
        /// </summary>
        /// <param name="bytes">byte array to convert</param>
        /// <param name="length">optional parameter for the number of bytes to get from the byte array.  Use null to get the length automatically.</param>
        /// <param name="ByteSeparatorText">Character string to use to separate between bytes</param>
        /// <returns></returns>
        public string ByteArrayToHexString(byte[] bytes, int? length = null, string ByteSeparatorText = "")
        {
            int len;
            string hexString = "";

            if (length == null) len = bytes.Length;
            else len = (int)length;
            if (ByteSeparatorText == null) ByteSeparatorText = "";

            for (int i = 0; i < len; i++)
            {
                if (i == 0) hexString = bytes[i].ToString("X2");        //no separator on first byte
                else hexString = hexString + ByteSeparatorText + bytes[i].ToString("X2");
            }
            return hexString;
        }

        public string FXD(string Input, int N)
        {
            if (Input.Length <= N)
            {
                return Input.PadLeft(N, '0');
            }
            else
            {
                return Input.Substring(Input.Length - N, N);
            }
        }

        public string Dec2Hex(int dec)
        {
            return dec.ToString("X2");
        }

        //Converts the specified integer to a hex string and returns the specified number of characters (nybbles).  This automatically adds leading 0's if needed or trims
        //characters from the most significant bytes where required to meet the NCharacters parameter.
        public string Dec2Hex(int dec, int NCharacters)
        {
            string result;
            result = dec.ToString("X2");

            return FXD(result, NCharacters);
        }

        public string Uint2Hex(uint dec)
        {
            return dec.ToString("X2");
        }

        public int Hex2Dec(string hexstring)
        {
            if (hexstring.Length > 8 || hexstring.Length <= 0)
            {
                throw new ArgumentException("hex must be at most 8 characters in length");
            }
            int newInt;
            if (!int.TryParse(hexstring, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out newInt))
            {
                throw new ArgumentException("Hex2Dec: TryParse Failed to convert to int: " + hexstring);
            }

            return newInt;
        }

        public uint Hex2Uint(string hexstring)
        {
            if (hexstring.Length > 8 || hexstring.Length <= 0)
            {
                throw new ArgumentException("hex must be at most 8 characters in length");
            }
            if (hexstring == "-1")
            {
                throw new ArgumentException("hexstring invalid value -1");
            }
            hexstring = hexstring.Replace("0x", "");
            uint newInt;
            if (!uint.TryParse(hexstring, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out newInt))
            {
                throw new ArgumentException("Hex2Uint: TryParse Failed");
            }

            return newInt;
        }

        public byte[] HexStringToByteArray(string hexstring)
        {
            byte[] bytes;
            if (hexstring.Length == 0)
            {
                return null;
            }

            try
            {
                //Ensure there is an even number of characters, and add a leading 0 if this is not the case:
                if (hexstring.Length % 2 != 0) hexstring = "0" + hexstring;

                bytes = new byte[hexstring.Length / 2];

                for (int i = 0; i < hexstring.Length / 2; i++)
                {
                    bytes[i] = HexToByte(hexstring.Substring(i * 2, 2));
                }
                return bytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine("HexStringToByteArray exception " + ex.Message);
                return null;        //Parse failed for some reason.
            }
        }

        /// <summary>
        /// Accepts a string of hexadecimal bytes and converts the bytes to their corresponding ASCII characters
        /// </summary>
        /// <param name="hexstring"></param>
        /// <returns></returns>
        public string HexStringToASCIIString(string hexstring)
        {
            return ByteArrayToString(HexStringToByteArray(hexstring));
        }

        /// <summary>
        /// Accepts an ASCII string and converts each character to its ASCII code displayed in hexadecimal form
        /// </summary>
        /// <param name="ASCIIString"></param>
        /// <returns></returns>
        public string ASCIIStringToHexString(string ASCIIString)
        {
            return ByteArrayToHexString(StringToByteArray(ASCIIString), null, null);
        }

        /// <summary>
        /// Converts 1 or 2 character string into equivalant byte value
        /// </summary>
        /// <param name="hex">1 or 2 character string</param>
        /// <returns>byte</returns>
        public byte HexToByte(string hex)
        {
            if (hex.Length > 2 || hex.Length <= 0)
            {
                throw new ArgumentException("hex must be 1 or 2 characters in length");
            }
            //byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            byte newByte;

            if (!byte.TryParse(hex, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out newByte))
            {

                throw new ArgumentException("HexToByte: TryParse Failed");
            }

            return newByte;
        }

        public int myMakeNumeric(string item)
        {
            int result;
            if (Int32.TryParse(item, out result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        //Simple Start / Stop Event:
        //System.Threading.ManualResetEvent ResetEvent = new ManualResetEvent(false);
        //ResetEvent.Reset();
        //result = ResetEvent.WaitOne(3000);       //Wait til ResetEvent.Set() is called (This is MUCH better than WaitForSingleObject)
        //ResetEvent.Set();

        //Array.Resize
        
        //Start a function as a thread:
        //System.Threading.Thread thread1;
        //thread1.threadstart(Function);

        //System.ComponentModel.BackgroundWorker bw = new System.ComponentModel.BackgroundWorker();
        //bw.DoWork += new System.ComponentModel.DoWorkEventHandler(Function);
        //private void Function(object sender, DoWorkEventArgs e)

        ///Great primer on threading:
        ///http://www.albahari.com/threading/


        //How to throw an exception:
        //throw new System.ArgumentException("Total Flash Size out of range", "TotalFlashSize");


        public void HandleException(string moduleName, Exception e)
        {
            // Purpose    : Provides a central mechanism for exception handling.
            //            : Displays a message box that describes the exception.

            // Accepts    : moduleName - the module where the exception occurred.
            //            : e - the exception

            string Message;
            string Caption;

            try
            {
                // Create an error message.
                Message = "Exception: " + e.Message + Environment.NewLine + "Module: " + moduleName + Environment.NewLine + "Method: " + e.TargetSite.Name;

                // Specify a caption.
                Caption = "Unexpected Exception";

                // Display the message in a message box.
                MessageBox.Show(Message, Caption, MessageBoxButtons.OK);
                Debug.Write(Message);
            }
            finally { }

        }

        public bool IsNullOrEmpty(object value)
        {
            if (value == null) return true;
            if (value.ToString() == "") return true;
            return false;
        }

        /// <summary>
        /// Replacement for VB InputBox, returns user input string.  Requires InputBoxDialog.cs
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="title"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string InputBox(string prompt, string title, string defaultValue)
        {
            InputBoxDialog ib = new InputBoxDialog();
            ib.FormPrompt = prompt;
            ib.FormCaption = title;
            ib.DefaultValue = defaultValue;
            ib.ShowDialog();
            string s = ib.InputResponse;
            ib.Close();
            return s;
        } // method: InputBox
    }
}
