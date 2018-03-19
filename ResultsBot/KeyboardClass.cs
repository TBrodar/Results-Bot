using MouseKeyboardLibrary;
using System;
using System.Windows.Forms;

namespace ResultsBot
{
    public static class KeyboardClass
    {
		public static IronPython.Runtime.List GetKeys()
        {
            IronPython.Runtime.List keys = new IronPython.Runtime.List();

            foreach(Keys s in Enum.GetValues(typeof(Keys)))
            {
                keys.Add(s.ToString());
            }

            return keys;
        }
		
		public static bool Write(string ASCIItext)
        {
			try
			{
				foreach (char c in ASCIItext.ToUpper())
				{
					KeyboardSimulator.KeyPress((Keys)c);
				}
			} catch (Exception)
			{
				string mesg = "Error: Exception occured while executing bool Write(string ASCIItext) function. Check if all characters are alphanumeric or if using KeyboardSimulator class works correctly.\nInfo: KeyDown function returned value false";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return false;
			}
			return true;
        }

        public static bool KeyDown(string c)
        {
			try { 
				foreach (Keys s in Enum.GetValues(typeof(Keys)))
				{
					if (s.ToString() == c)
					{
						KeyboardSimulator.KeyDown(s);
					}
				}
			} catch (Exception)
			{
				string mesg = "Error: Exception occured while executing bool KeyDown(string c) function.\nInfo: KeyDown function returned value false";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return false;
			}
			return true;
		}
        public static bool KeyUp(string c)
        {
			try {
				foreach (Keys s in Enum.GetValues(typeof(Keys)))
				{
					if (s.ToString() == c)
					{
						KeyboardSimulator.KeyUp(s);
					}
				}
			}
			catch (Exception)
			{
				string mesg = "Error: Exception occured while executing bool KeyUp(string c) function.\nInfo: KeyUp function returned value false";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return false;
			}
			return true;
		}

        public static bool KeyPress(string c)
        {
			try { 
				foreach (Keys s in Enum.GetValues(typeof(Keys)))
				{
					if (s.ToString() == c)
					{
						KeyboardSimulator.KeyPress(s);
						return true;
					}
				}
			}
			catch (Exception)
			{
				string mesg = "Error: Exception occured while executing bool KeyPress(string c) function.\nInfo: KeyPress function returned value false";
				MainWindow.MainWindowInstance.Dispatcher.BeginInvoke(new MainWindow.SetStatusDelegate(MainWindow.MainWindowInstance.SetStatus),
mesg);
				MainWindow.consoleWriter.WriteLine(mesg);
				return false;
			}
			return false;
		}

    }
}
