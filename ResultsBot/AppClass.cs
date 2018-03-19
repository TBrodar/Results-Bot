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
        [STAThread]
        public static bool SetClipboardText(string text)
        {
			try { 
            System.Windows.Clipboard.SetText(text);
			} catch (Exception)
			{
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
