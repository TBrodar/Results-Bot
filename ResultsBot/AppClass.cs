using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ResultsBot
{
    public static class AppClass
    {

        //public static string GetErrorsString()
        //{
        //    return "";
        //}

        public static void  SetAliveState(bool IsAlive)
        {
            
        }

        public static bool IsAlive()
        {
            return true;
        }

        public static void SetPausedState(bool IsAlive)
        {

        }

        public static bool IsPaused()
        {
            return false;
        }
        public static void SetStatus(string Text)
        {

        }

        public static void ClearConsoleOutput()
        {
            return ;
        }

        [STAThread]
        public static bool SetClipboardText(string text)
        {
			try { 
            System.Windows.Clipboard.SetText(text);
			} catch (Exception e)
			{
                string mesg = "SetClipboardText(text) failed. Probably OS - C# internal error."
                throw new Exception(mesg + "\n\nInternal message:\n" + e.Message + "\n" + e.StackTrace);
                return false;
			}
			return true;
        }
        [STAThread]
        public static string GetClipboardText()
        {
            return System.Windows.Clipboard.GetText();
        }

    }
}
