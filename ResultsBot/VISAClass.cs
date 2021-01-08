using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using NationalInstruments.VisaNS;

namespace ResultsBot
{

    

    class VISAClass
    {
        private static MessageBasedSession mbSession;

        public static bool OpenSession(string resourceString)
        {
            try
            {
                mbSession = (MessageBasedSession)ResourceManager.GetLocalManager().Open(resourceString);
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("Resource selected must be a message-based session");
                return false;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                return false;
            }

            return true;
        }

        public static void CloseSession()
        { 
            mbSession.Dispose();
        }

        public static string Query_String(string TextToWrite)
        { 
            try
            { 
                string responseString = mbSession.Query(TextToWrite);
                responseString = InsertCommonEscapeSequences(responseString);
                return responseString;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                return "";
            } 
        }
        public static string Query_SpecificNumberOfCharacters(string TextToWrite, int NumberOfCharactersToRead)
        {
            try
            {
                string responseString = mbSession.Query(TextToWrite, NumberOfCharactersToRead);
                responseString = InsertCommonEscapeSequences(responseString);
                return responseString;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                return "";
            }
        }
        public static IronPython.Runtime.List Query_Bytes(IronPython.Runtime.List BytesToWrite)
        {
            byte[] BytesToWrite_pom = new byte[BytesToWrite.Count];

            for ( int i = 0; i < BytesToWrite.Count; i++)
            {
                BytesToWrite_pom[i] = Convert.ToByte((int)BytesToWrite[i]);
            }
            try
            { 
                byte[] responseBytes_pom = mbSession.Query(BytesToWrite_pom);
                IronPython.Runtime.List responseBytes = new IronPython.Runtime.List();
                for (int i = 0; i < responseBytes_pom.Length; i++){
                    responseBytes.append((int)responseBytes_pom[i]);
                }
                return responseBytes;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                return null;
            }
        }
        public static IronPython.Runtime.List Query_SpecificNumberOfBytes(IronPython.Runtime.List BytesToWrite, int NumberOfBytesToRead)
        {
            byte[] BytesToWrite_pom = new byte[BytesToWrite.Count];

            for (int i = 0; i < BytesToWrite.Count; i++)
            {
                BytesToWrite_pom[i] = Convert.ToByte((int)BytesToWrite[i]);
            }
            MessageBox.Show(string.Join(",", BytesToWrite_pom));
            //try
            //{
            byte[] responseBytes_pom = mbSession.Query(BytesToWrite_pom, NumberOfBytesToRead);
                IronPython.Runtime.List responseBytes = new IronPython.Runtime.List();
                for (int i = 0; i < responseBytes_pom.Length; i++)
                {
                    responseBytes.append((int)responseBytes_pom[i]);
                } 
                return responseBytes;

            //}
            //catch (Exception exp)
            //{
            //    MessageBox.Show(exp.Message);
            //    return null;
            //}
        }



        public static bool Write(string TextToWrite)
        {
            try
            { 
                mbSession.Write(TextToWrite);
                return true;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                return false;
            }
        }

        public static string Read()
        {
            try
            {
                string responseString = mbSession.ReadString();
                return responseString;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                return "";
            }
        }

        private static string ReplaceCommonEscapeSequences(string s)
        {
            return s.Replace("\\n", "\n").Replace("\\r", "\r");
        }

        private static string InsertCommonEscapeSequences(string s)
        {
            return s.Replace("\n", "\\n").Replace("\r", "\\r");
        } 
    }
}
